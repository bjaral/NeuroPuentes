from fastapi import FastAPI, File, UploadFile, HTTPException, Form
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
import base64
import os
import uuid
from typing import Optional, List # <--- Importar List
import logging
import json
from contextlib import asynccontextmanager # <--- Importar lifespan

from whisper_module import transcribe_audio, load_whisper_model 
from llama_module import generate_response, initialize_llama, PROMPT_TRAITS # <--- Importar PROMPT_TRAITS
from tts_module import text_to_speech, initialize_tts

# Configuración de logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# --- Modelos Globales ---
models = {}

# Almacén de contexto conversacional (memoria simple)
conversation_contexts = {}


# --- NUEVO: Lifespan para cargar modelos ---
@asynccontextmanager
async def lifespan(app: FastAPI):
    """Inicializa los modelos al arrancar el servidor"""
    logger.info("Inicializando modelos...")
    
    try:
        # Cargar Whisper
        logger.info("Cargando Whisper (small) en cuda...")
        import whisper
        # --- MEJORA: Modelo 'small' por defecto ---
        models["whisper_model"] = whisper.load_model("small", device="cuda")
        logger.info("Modelo Whisper 'small' cargado exitosamente")
        
        # Cargar LLaMA/Gemma
        logger.info("Cargando LLaMA/Gemma (Gemma-2B) en cuda...")
        models["llama_model"], models["llama_tokenizer"] = initialize_llama()
        logger.info("Modelo LLM cargado exitosamente")
        
        # Cargar TTS
        logger.info("Cargando Coqui TTS en cuda...")
        models["tts_model"] = initialize_tts(gpu=True) # Forzar GPU
        
        logger.info("Todos los modelos cargados exitosamente")
    except Exception as e:
        logger.error(f"Error fatal al cargar modelos: {str(e)}")
        raise
    
    yield
    
    logger.info("Limpiando y liberando modelos...")
    models.clear()
    conversation_contexts.clear()


app = FastAPI(
    title="IA Backend - Simulación Conversacional",
    lifespan=lifespan # <-- Registrar el nuevo lifespan
)

# CORS para permitir comunicación con .NET
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

TEMP_DIR = "temp_audio"
os.makedirs(TEMP_DIR, exist_ok=True)

class IAResponse(BaseModel):
    transcription: str
    response_text: str
    audio_base64: str
    session_id: str
    success: bool
    error: Optional[str] = None


@app.get("/")
async def root():
    return {
        "service": "IA Backend - Simulación Conversacional TEA",
        "status": "running",
        "endpoints": ["/process_audio", "/health", "/context-traits"]
    }

@app.get("/health")
async def health_check():
    """Verificar el estado del servicio y modelos"""
    return {
        "status": "healthy",
        "whisper_loaded": "whisper_model" in models,
        "llama_loaded": "llama_model" in models,
        "tts_loaded": "tts_model" in models
    }

# --- NUEVO ENDPOINT ---
@app.get("/context-traits")
async def get_context_traits():
    """
    Endpoint para que el frontend (.NET) pueda saber qué 
    rasgos (traits) están disponibles en la biblioteca de la IA.
    """
    return {"available_traits": list(PROMPT_TRAITS.keys())}


@app.post("/process_audio", response_model=IAResponse)
async def process_audio(
    audio: UploadFile = File(...),
    
    # 1. ID estático (de un Ctx pre-hecho)
    context_id: Optional[str] = Form(None), 
    
    # 2. Lista de rasgos (para modo dinámico)
    #    (Este es el que usa tu .NET)
    context_traits: Optional[str] = Form(None), # Recibe un string separado por comas
    
    session_id: Optional[str] = Form(None) 
):
    """
    Endpoint principal: procesa audio y genera respuesta.
    Acepta un 'context_id' (estático) O 
    un 'context_traits' (lista de rasgos dinámica separada por comas).
    """
    global conversation_contexts

    if not session_id:
        session_id = str(uuid.uuid4())
        logger.info(f"Nueva sesión de IA iniciada: {session_id}")
    
    temp_input_path = None
    temp_output_path = None
    
    try:
        # --- Lógica de Contexto ---
        traits_list: Optional[List[str]] = None
        if context_traits:
            # El backend .NET envía un string separado por comas
            traits_list = [trait.strip() for trait in context_traits.split(',')]
            logger.info(f"Usando contexto dinámico (Traits): {traits_list}")
        elif context_id:
            logger.info(f"Usando contexto estático (ID): {context_id}")
        else:
            logger.warning("No se proporcionó context_id ni context_traits. Usando default.")
            context_id = "padre_hijo_8_anios"

        # Guardar audio
        if not audio.content_type or not audio.content_type.startswith("audio"):
            raise HTTPException(status_code=400, detail="El archivo debe ser de tipo audio")
        
        file_extension = audio.filename.split(".")[-1]
        temp_input_path = os.path.join(TEMP_DIR, f"{uuid.uuid4()}.{file_extension}")
        with open(temp_input_path, "wb") as f:
            f.write(await audio.read())
        
        # PASO 1: Transcribir
        logger.info(f"[{session_id}] Transcribiendo audio...")
        transcription = transcribe_audio(models["whisper_model"], temp_input_path)
        logger.info(f"[{session_id}] Transcripción: {transcription}")
        
        if not transcription or transcription.strip() == "":
            raise HTTPException(status_code=400, detail="No se pudo transcribir el audio.")
        
        # PASO 2: Contexto
        if session_id not in conversation_contexts:
            conversation_contexts[session_id] = []
        conversation_contexts[session_id].append({"role": "student", "content": transcription})
        
        # PASO 3: Generar respuesta
        logger.info(f"[{session_id}] Generando respuesta con LLaMA...")
        response_text = generate_response(
            models["llama_model"],
            models["llama_tokenizer"],
            transcription,
            conversation_contexts[session_id],
            context_id=context_id,
            context_traits=traits_list 
        )
        logger.info(f"[{session_id}] Respuesta generada: {response_text}")
        
        conversation_contexts[session_id].append({"role": "parent", "content": response_text})
        if len(conversation_contexts[session_id]) > 20:
            conversation_contexts[session_id] = conversation_contexts[session_id][-20:]
        
        # PASO 4: TTS
        logger.info(f"[{session_id}] Generando audio de respuesta...")
        temp_output_path = os.path.join(TEMP_DIR, f"{uuid.uuid4()}_response.wav")
        text_to_speech(models["tts_model"], response_text, temp_output_path)
        
        # PASO 5: Base64
        with open(temp_output_path, "rb") as audio_file:
            audio_bytes = audio_file.read()
            audio_base64 = base64.b64encode(audio_bytes).decode("utf-8")
        
        logger.info(f"[{session_id}] Procesamiento completado exitosamente")
        
        return IAResponse(
            transcription=transcription,
            response_text=response_text,
            audio_base64=audio_base64,
            session_id=session_id,
            success=True
        )
    
    except HTTPException as he:
        raise he
    except Exception as e:
        logger.error(f"Error en process_audio: {str(e)}")
        return IAResponse(
            transcription="", response_text="", audio_base64="",
            session_id=session_id or "", success=False, error=str(e)
        )
    
    finally:
        # Limpieza
        if temp_input_path and os.path.exists(temp_input_path):
            os.remove(temp_input_path)
        if temp_output_path and os.path.exists(temp_output_path):
            os.remove(temp_output_path)

@app.post("/reset_session/{session_id}")
async def reset_session(session_id: str):
    """Reinicia el contexto conversacional de una sesión"""
    global conversation_contexts
    if session_id in conversation_contexts:
        del conversation_contexts[session_id]
        return {"message": f"Sesión {session_id} reiniciada"}
    return {"message": "Sesión no encontrada"}

@app.get("/session_context/{session_id}")
async def get_session_context(session_id: str):
    """Obtiene el historial de conversación de una sesión"""
    if session_id in conversation_contexts:
        return {"session_id": session_id, "context": conversation_contexts[session_id]}
    return {"session_id": session_id, "context": []}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=5001)