import whisper
import torch
import logging
from typing import Optional

logger = logging.getLogger(__name__)

def transcribe_audio(model: whisper.Whisper, audio_path: str, language: str = "es") -> str:
    """
    Transcribe un archivo de audio usando Whisper
    
    Args:
        model: Modelo de Whisper cargado
        audio_path: Ruta al archivo de audio
        language: Idioma del audio (por defecto español)
    
    Returns:
        Texto transcrito
    """
    try:
        # Verificar disponibilidad de GPU
        device = "cuda" if torch.cuda.is_available() else "cpu"
        logger.info(f"Usando dispositivo: {device}")
        
        # Transcribir audio
        # fp16=False para CPU, fp16=True para GPU (más rápido)
        result = model.transcribe(
            audio_path,
            language=language,
            fp16=torch.cuda.is_available(),
            task="transcribe",
            verbose=False
        )
        
        transcription = result["text"].strip()
        
        # Validar que la transcripción no esté vacía
        if not transcription:
            logger.warning("La transcripción está vacía")
            return ""
        
        logger.info(f"Transcripción exitosa: {len(transcription)} caracteres")
        return transcription
    
    except Exception as e:
        logger.error(f"Error en transcripción: {str(e)}")
        raise Exception(f"Error al transcribir audio: {str(e)}")

def load_whisper_model(model_size: str = "base") -> whisper.Whisper:
    """
    Carga el modelo de Whisper
    
    Args:
        model_size: Tamaño del modelo (tiny, base, small, medium, large)
                   - tiny: más rápido, menos preciso
                   - base: balance recomendado para producción
                   - small: mejor precisión, más lento
                   - medium/large: máxima precisión, requiere GPU potente
    
    Returns:
        Modelo de Whisper cargado
    """
    try:
        device = "cuda" if torch.cuda.is_available() else "cpu"
        logger.info(f"Cargando Whisper modelo '{model_size}' en {device}...")
        
        model = whisper.load_model(model_size, device=device)
        
        logger.info(f"Modelo Whisper '{model_size}' cargado exitosamente")
        return model
    
    except Exception as e:
        logger.error(f"Error al cargar Whisper: {str(e)}")
        raise Exception(f"No se pudo cargar el modelo Whisper: {str(e)}")

def get_audio_info(audio_path: str) -> dict:
    """
    Obtiene información sobre el archivo de audio
    
    Args:
        audio_path: Ruta al archivo de audio
    
    Returns:
        Diccionario con información del audio
    """
    try:
        import soundfile as sf
        
        data, samplerate = sf.read(audio_path)
        duration = len(data) / samplerate
        
        return {
            "duration_seconds": duration,
            "sample_rate": samplerate,
            "channels": data.shape[1] if len(data.shape) > 1 else 1,
            "samples": len(data)
        }
    
    except Exception as e:
        logger.warning(f"No se pudo obtener información del audio: {str(e)}")
        return {}