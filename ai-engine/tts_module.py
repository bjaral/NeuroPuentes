from TTS.api import TTS
import torch
import logging
import os
import re # <-- Para reemplazar números

logger = logging.getLogger(__name__)

def num_to_words(text):
    """
    Convierte dígitos en un string a palabras.
    Ej: "8 es complicado" -> "ocho es complicado"
    """
    # Simple reemplazo para los números que fallaban
    text = re.sub(r'\b8\b', 'ocho', text)
    text = re.sub(r'\b4\b', 'cuatro', text)
    text = re.sub(r'\b2\b', 'dos', text)
    text = re.sub(r'\b3\b', 'tres', text)
    text = re.sub(r'\b5\b', 'cinco', text)
    return text

def initialize_tts(model_name: str = "tts_models/es/css10/vits", gpu: bool = False) -> TTS:
    """
    Inicializa el modelo de Text-to-Speech
    
    Args:
        model_name: Nombre del modelo TTS
        gpu: (Opcional) Si se debe usar GPU
    
    Returns:
        Modelo TTS inicializado
    """
    try:
        # --- ARREGLO de DeprecationWarning ---
        # `gpu=True` está obsoleto, ahora usamos .to(device)
        device = "cuda" if gpu and torch.cuda.is_available() else "cpu"
        logger.info(f"Inicializando TTS en dispositivo: {device}")
        
        # 1. Cargar modelo (sin el argumento 'gpu')
        tts = TTS(model_name=model_name, progress_bar=False)
        
        # 2. Mover el modelo al dispositivo (GPU o CPU)
        tts.to(device)
        
        logger.info(f"Modelo TTS '{model_name}' cargado exitosamente")
        return tts
    
    except Exception as e:
        logger.error(f"Error al cargar TTS: {str(e)}")
        raise Exception(f"No se pudo cargar el modelo TTS: {str(e)}")

def text_to_speech(
    tts_model: TTS,
    text: str,
    output_path: str,
    emotion: str = "neutral" # (Este argumento se mantiene)
) -> str:
    """
    Convierte texto a audio usando Coqui TTS
    """
    try:
        # Validar texto
        if not text or len(text.strip()) == 0:
            raise ValueError("El texto está vacío")
        
        # Limpiar y preparar texto
        text = text.strip()
        
        # --- NUEVA MEJORA ---
        # Reemplazar dígitos por palabras para que TTS no falle
        text = num_to_words(text)
        
        if len(text) > 500:
            logger.warning("Texto muy largo, truncando a 500 caracteres")
            text = text[:500] + "..."
        
        logger.info(f"Generando audio para: {text[:50]}...")
        
        # Generar audio
        tts_model.tts_to_file(
            text=text,
            file_path=output_path,
            # (El parámetro 'emotion' no existe en este modelo VITS,
            # pero lo dejamos por si se cambia a un modelo que sí lo soporte)
        )
        
        if not os.path.exists(output_path):
            raise Exception("No se generó el archivo de audio")
        
        file_size = os.path.getsize(output_path)
        logger.info(f"Audio generado: {output_path} ({file_size} bytes)")
        
        return output_path
    
    except Exception as e:
        logger.error(f"Error al generar audio: {str(e)}")
        raise Exception(f"No se pudo generar el audio: {str(e)}")

# (Las otras funciones de 'tts_module.py' no necesitan cambios)
def get_available_voices() -> list:
    """
    Lista las voces disponibles en Coqui TTS
    """
    try:
        tts = TTS()
        models = tts.list_models()
        spanish_models = [m for m in models if "/es/" in m or "spanish" in m.lower()]
        return spanish_models
    except Exception as e:
        logger.error(f"Error al listar voces: {str(e)}")
        return []