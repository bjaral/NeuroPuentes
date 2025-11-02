import soundfile as sf
import numpy as np
from scipy import signal
import logging

logger = logging.getLogger(__name__)

def validate_audio_file(file_path: str) -> dict:
    """
    Valida un archivo de audio y retorna información sobre él
    
    Args:
        file_path: Ruta al archivo de audio
    
    Returns:
        Diccionario con información del audio o None si es inválido
    """
    try:
        data, samplerate = sf.read(file_path)
        
        duration = len(data) / samplerate
        channels = data.shape[1] if len(data.shape) > 1 else 1
        
        # Validaciones básicas
        if duration < 0.5:
            logger.warning("Audio demasiado corto (< 0.5s)")
            return None
        
        if duration > 180:  # 3 minutos
            logger.warning("Audio demasiado largo (> 3 minutos)")
            return None
        
        info = {
            "valid": True,
            "duration": duration,
            "sample_rate": samplerate,
            "channels": channels,
            "samples": len(data),
            "file_path": file_path
        }
        
        logger.info(f"Audio validado: {duration:.2f}s, {samplerate}Hz, {channels} canal(es)")
        return info
    
    except Exception as e:
        logger.error(f"Error al validar audio: {str(e)}")
        return None

def normalize_audio(audio_data: np.ndarray) -> np.ndarray:
    """
    Normaliza el volumen del audio
    
    Args:
        audio_data: Datos del audio como numpy array
    
    Returns:
        Audio normalizado
    """
    try:
        # Normalizar a rango [-1, 1]
        max_val = np.abs(audio_data).max()
        if max_val > 0:
            normalized = audio_data / max_val
        else:
            normalized = audio_data
        
        return normalized
    
    except Exception as e:
        logger.error(f"Error al normalizar audio: {str(e)}")
        return audio_data

def reduce_noise(audio_data: np.ndarray, sample_rate: int) -> np.ndarray:
    """
    Reduce ruido del audio (filtro básico)
    
    Args:
        audio_data: Datos del audio
        sample_rate: Frecuencia de muestreo
    
    Returns:
        Audio con ruido reducido
    """
    try:
        # Filtro pasa-bajos para eliminar ruido de alta frecuencia
        nyquist = sample_rate / 2
        cutoff = 3000  # Hz
        
        # Diseñar filtro Butterworth
        b, a = signal.butter(5, cutoff / nyquist, btype='low')
        
        # Aplicar filtro
        filtered = signal.filtfilt(b, a, audio_data)
        
        return filtered
    
    except Exception as e:
        logger.error(f"Error al reducir ruido: {str(e)}")
        return audio_data

def convert_to_mono(audio_data: np.ndarray) -> np.ndarray:
    """
    Convierte audio estéreo a mono
    
    Args:
        audio_data: Datos del audio
    
    Returns:
        Audio en mono
    """
    try:
        if len(audio_data.shape) > 1:
            # Promediar canales
            mono = audio_data.mean(axis=1)
            logger.info("Audio convertido de estéreo a mono")
            return mono
        return audio_data
    
    except Exception as e:
        logger.error(f"Error al convertir a mono: {str(e)}")
        return audio_data

def resample_audio(
    audio_data: np.ndarray, 
    original_rate: int, 
    target_rate: int = 16000
) -> np.ndarray:
    """
    Remuestrea audio a una frecuencia objetivo
    
    Args:
        audio_data: Datos del audio
        original_rate: Frecuencia original
        target_rate: Frecuencia objetivo (16kHz es óptimo para Whisper)
    
    Returns:
        Audio remuestreado
    """
    try:
        if original_rate == target_rate:
            return audio_data
        
        # Calcular número de muestras
        num_samples = int(len(audio_data) * target_rate / original_rate)
        
        # Remuestrear
        resampled = signal.resample(audio_data, num_samples)
        
        logger.info(f"Audio remuestreado de {original_rate}Hz a {target_rate}Hz")
        return resampled
    
    except Exception as e:
        logger.error(f"Error al remuestrear: {str(e)}")
        return audio_data

def preprocess_audio(file_path: str, output_path: str = None) -> str:
    """
    Preprocesa audio completo: normalización, reducción de ruido, mono, remuestreo
    
    Args:
        file_path: Ruta del audio original
        output_path: Ruta para guardar audio procesado (opcional)
    
    Returns:
        Ruta del audio procesado
    """
    try:
        # Leer audio
        data, samplerate = sf.read(file_path)
        
        # Convertir a mono
        data = convert_to_mono(data)
        
        # Normalizar
        data = normalize_audio(data)
        
        # Reducir ruido (opcional, puede ser lento)
        # data = reduce_noise(data, samplerate)
        
        # Remuestrear a 16kHz (óptimo para Whisper)
        data = resample_audio(data, samplerate, 16000)
        
        # Guardar
        if output_path is None:
            output_path = file_path.replace(".wav", "_processed.wav")
        
        sf.write(output_path, data, 16000)
        
        logger.info(f"Audio preprocesado guardado en: {output_path}")
        return output_path
    
    except Exception as e:
        logger.error(f"Error al preprocesar audio: {str(e)}")
        return file_path  # Retornar original si falla