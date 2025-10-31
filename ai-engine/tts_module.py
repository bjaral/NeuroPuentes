from TTS.api import TTS
import torch
import logging
import os

logger = logging.getLogger(__name__)

def initialize_tts(model_name: str = "tts_models/es/css10/vits") -> TTS:
    """
    Inicializa el modelo de Text-to-Speech
    
    Args:
        model_name: Nombre del modelo TTS
                   Modelos en español recomendados:
                   - "tts_models/es/css10/vits" (español, voz natural)
                   - "tts_models/es/mai/tacotron2-DDC" (alternativa)
                   - "tts_models/multilingual/multi-dataset/your_tts" (multiidioma)
    
    Returns:
        Modelo TTS inicializado
    """
    try:
        device = "cuda" if torch.cuda.is_available() else "cpu"
        logger.info(f"Inicializando TTS en dispositivo: {device}")
        
        # Inicializar modelo
        tts = TTS(model_name=model_name, progress_bar=False, gpu=(device == "cuda"))
        
        logger.info(f"Modelo TTS '{model_name}' cargado exitosamente")
        return tts
    
    except Exception as e:
        logger.error(f"Error al cargar TTS: {str(e)}")
        raise Exception(f"No se pudo cargar el modelo TTS: {str(e)}")

def text_to_speech(
    tts_model: TTS,
    text: str,
    output_path: str,
    emotion: str = "neutral"
) -> str:
    """
    Convierte texto a audio usando Coqui TTS
    
    Args:
        tts_model: Modelo TTS inicializado
        text: Texto a convertir
        output_path: Ruta donde guardar el audio
        emotion: Emoción a transmitir (si el modelo lo soporta)
    
    Returns:
        Ruta al archivo de audio generado
    """
    try:
        # Validar texto
        if not text or len(text.strip()) == 0:
            raise ValueError("El texto está vacío")
        
        # Limpiar y preparar texto
        text = text.strip()
        
        # Limitar longitud para evitar audios muy largos
        if len(text) > 500:
            logger.warning("Texto muy largo, truncando a 500 caracteres")
            text = text[:500] + "..."
        
        logger.info(f"Generando audio para: {text[:50]}...")
        
        # Generar audio
        tts_model.tts_to_file(
            text=text,
            file_path=output_path,
            emotion=emotion if hasattr(tts_model, 'emotion') else None
        )
        
        # Verificar que el archivo se creó
        if not os.path.exists(output_path):
            raise Exception("No se generó el archivo de audio")
        
        file_size = os.path.getsize(output_path)
        logger.info(f"Audio generado: {output_path} ({file_size} bytes)")
        
        return output_path
    
    except Exception as e:
        logger.error(f"Error al generar audio: {str(e)}")
        raise Exception(f"No se pudo generar el audio: {str(e)}")

def adjust_speech_rate(audio_path: str, rate: float = 1.0):
    """
    Ajusta la velocidad del audio (opcional, requiere procesamiento adicional)
    
    Args:
        audio_path: Ruta al archivo de audio
        rate: Factor de velocidad (1.0 = normal, 0.8 = más lento, 1.2 = más rápido)
    """
    try:
        import soundfile as sf
        from scipy import signal
        
        data, samplerate = sf.read(audio_path)
        
        # Cambiar velocidad sin cambiar tono
        if rate != 1.0:
            # Remuestrear
            number_of_samples = round(len(data) / rate)
            data_resampled = signal.resample(data, number_of_samples)
            
            # Guardar
            sf.write(audio_path, data_resampled, samplerate)
            logger.info(f"Velocidad ajustada a {rate}x")
    
    except Exception as e:
        logger.warning(f"No se pudo ajustar la velocidad del audio: {str(e)}")

def get_available_voices() -> list:
    """
    Lista las voces disponibles en Coqui TTS
    
    Returns:
        Lista de modelos TTS disponibles
    """
    try:
        tts = TTS()
        models = tts.list_models()
        
        # Filtrar modelos en español
        spanish_models = [m for m in models if "/es/" in m or "spanish" in m.lower()]
        
        return spanish_models
    
    except Exception as e:
        logger.error(f"Error al listar voces: {str(e)}")
        return []