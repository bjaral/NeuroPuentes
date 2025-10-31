from fastapi import FastAPI, File, UploadFile, HTTPException, Form
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
import base64
import os
import uuid
from typing import Optional
import logging
# 'json' ya no es necesario aquí
# import json 

from whisper_module import transcribe_audio
from llama_module import generate_response, initialize_llama
from tts_module import text_to_speech, initialize_tts

# Configuración de logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

app = FastAPI(title="IA Backend - Simulación Conversacional")

# CORS (sin cambios)
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

TEMP_DIR = "temp_audio"
os.makedirs(TEMP_DIR, exist_ok=True)

# Modelos globales (sin cambios)
whisper_model = None
llama_model = None
llama_tokenizer = None
tts_model = None

# --- ARREGLO 1 (Error de 'NameError') ---
# Esta variable es global
conversation_contexts = {}

class IAResponse(BaseModel):
    transcription: str
    response_text: str
    audio_base64: str
    session_id: str
    success: bool
    error: Optional[str] = None

@app.on_event("startup")
async def startup_event():
    """Inicializa los modelos al arrancar el servidor"""
    global whisper_model, llama_model, llama_tokenizer, tts_model
    logger.info("Inicializando modelos...")
    try:
        logger.info("Cargando Whisper (small)...")
        import whisper
        whisper_model = whisper.load_model("small")
        
        logger.info("Cargando LLaMA (1B)...")
        llama_model, llama_tokenizer = initialize_llama()
        
        logger.info("Cargando Coqui TTS...")
        tts_model = initialize_tts()
        
        logger.info("Todos los modelos cargados exitosamente")
    except Exception as e:
        logger.error(f"Error al cargar modelos: {str(e)}")
        raise

@app.get("/")
async def root():
    return {"service": "IA Backend - Simulación Conversacional TEA", "status": "running"}

@app.get("/health")
async def health_check():
    return {
        "status": "healthy",
        "whisper_loaded": whisper_model is not None,
        "llama_loaded": llama_model is not None,
        "tts_loaded": tts_model is not None
    }

# --- ENDPOINT MODIFICADO ---
@app.post("/process_audio", response_model=IAResponse)
async def process_audio(
    audio: UploadFile = File(...),
    
    # Opción 1: ID estático
    context_id: Optional[str] = Form("padre_hijo_8_anios"), 
    
    # --- ARREGLO 2 (JSON vs Comas) ---
    # Ahora acepta un string simple separado por comas
    context_traits: Optional[str] = Form(None), 
    
    session_id: Optional[str] = Form(None) 
):
    """
    Endpoint principal: procesa audio y genera respuesta.
    Acepta un 'context_id' (estático) O 
    un 'context_traits' (string separado por comas para modo dinámico).
    """
    
    # --- ARREGLO 1 (continuación) ---
    # Le decimos a esta función que use la variable global
    global conversation_contexts

    if not session_id:
        session_id = str(uuid.uuid4())
    
    temp_input_path = None
    temp_output_path = None
    
    try:
        # --- ARREGLO 2 (continuación) ---
        traits_list = None
        if context_traits:
            # Ahora procesamos el string separado por comas
            # ej: "rol_padre, hijo_8_anios, emocion_cansado"
            traits_list = [trait.strip() for trait in context_traits.split(',')]
            logger.info(f"Usando contexto dinámico (Traits): {traits_list}")
        else:
            logger.info(f"Usando contexto estático (ID): {context_id}")
        
        # Guardar audio
        if not audio.content_type or not audio.content_type.startswith("audio"):
            raise HTTPException(status_code=400, detail="El archivo debe ser de tipo audio")
        
        file_extension = audio.filename.split(".")[-1]
        temp_input_path = os.path.join(TEMP_DIR, f"{uuid.uuid4()}.{file_extension}")
        with open(temp_input_path, "wb") as f:
            f.write(await audio.read())
        
        # PASO 1: Transcribir
        logger.info("Transcribiendo audio...")
        transcription = transcribe_audio(whisper_model, temp_input_path)
        logger.info(f"Transcripción: {transcription}")
        
        if not transcription or transcription.strip() == "":
            raise HTTPException(status_code=400, detail="No se pudo transcribir el audio.")
        
        # PASO 2: Contexto
        if session_id not in conversation_contexts:
            conversation_contexts[session_id] = []
        conversation_contexts[session_id].append({"role": "student", "content": transcription})
        
        # PASO 3: Generar respuesta
        logger.info("Generando respuesta con LLaMA...")
        response_text = generate_response(
            llama_model,
            llama_tokenizer,
            transcription,
            conversation_contexts[session_id],
            context_id=context_id,
            context_traits=traits_list 
        )
        logger.info(f"Respuesta generada: {response_text}")
        
        conversation_contexts[session_id].append({"role": "parent", "content": response_text})
        if len(conversation_contexts[session_id]) > 20:
            conversation_contexts[session_id] = conversation_contexts[session_id][-20:]
        
        # PASO 4: TTS
        logger.info("Generando audio de respuesta...")
        temp_output_path = os.path.join(TEMP_DIR, f"{uuid.uuid4()}_response.wav")
        text_to_speech(tts_model, response_text, temp_output_path)
        
        # PASO 5: Base64
        with open(temp_output_path, "rb") as audio_file:
            audio_bytes = audio_file.read()
            audio_base64 = base64.b64encode(audio_bytes).decode("utf-8")
        
        logger.info("Procesamiento completado exitosamente")
        
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
    # --- ARREGLO 1 (continuación) ---
    global conversation_contexts
    if session_id in conversation_contexts:
        del conversation_contexts[session_id]
        return {"message": f"Sesión {session_id} reiniciada"}
    return {"message": "Sesión no encontrada"}

@app.get("/session_context/{session_id}")
async def get_session_context(session_id: str):
    # --- ARREGLO 1 (continuación) ---
    # (Técnicamente no es necesario aquí porque solo lee, pero es buena práctica)
    global conversation_contexts
    if session_id in conversation_contexts:
        return {"session_id": session_id, "context": conversation_contexts[session_id]}
    return {"session_id": session_id, "context": []}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=5001)