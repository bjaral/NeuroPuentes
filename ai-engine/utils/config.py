import os
from dotenv import load_dotenv
from pathlib import Path

# Cargar variables de entorno
load_dotenv()

class Config:
    """Configuración centralizada del microservicio"""
    
    # Puerto del servicio
    PORT = int(os.getenv("PORT", 5001))
    
    # Modelos
    WHISPER_MODEL = os.getenv("WHISPER_MODEL", "base")
    LLAMA_MODEL = os.getenv("LLAMA_MODEL", "meta-llama/Llama-3.2-1B-Instruct")
    TTS_MODEL = os.getenv("TTS_MODEL", "tts_models/es/css10/vits")
    
    # Parámetros de generación
    MAX_TOKENS = int(os.getenv("MAX_TOKENS", 150))
    TEMPERATURE = float(os.getenv("TEMPERATURE", 0.8))
    TOP_P = float(os.getenv("TOP_P", 0.9))
    
    # HuggingFace token
    HF_TOKEN = os.getenv("HF_TOKEN")
    
    # Directorios
    TEMP_DIR = os.getenv("TEMP_DIR", "temp_audio")
    LOG_DIR = os.getenv("LOG_DIR", "logs")
    
    # Base de datos
    DATABASE_URL = os.getenv("DATABASE_URL")
    
    # Debug
    DEBUG = os.getenv("DEBUG", "True").lower() == "true"
    
    @classmethod
    def ensure_directories(cls):
        """Crea los directorios necesarios si no existen"""
        Path(cls.TEMP_DIR).mkdir(exist_ok=True)
        Path(cls.LOG_DIR).mkdir(exist_ok=True)
    
    @classmethod
    def validate(cls):
        """Valida la configuración"""
        if not cls.HF_TOKEN:
            print("⚠️  ADVERTENCIA: HF_TOKEN no está configurado. Algunos modelos pueden no cargar.")
        
        if cls.DEBUG:
            print("🐛 Modo DEBUG activado")
        
        print(f"✅ Configuración cargada:")
        print(f"   - Whisper: {cls.WHISPER_MODEL}")
        print(f"   - LLaMA: {cls.LLAMA_MODEL}")
        print(f"   - TTS: {cls.TTS_MODEL}")
        print(f"   - Puerto: {cls.PORT}")

# Configuración de HuggingFace
if Config.HF_TOKEN:
    os.environ["HUGGINGFACE_TOKEN"] = Config.HF_TOKEN