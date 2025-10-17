import os
import pandas as pd
from sqlalchemy import create_engine, text
from dotenv import load_dotenv

# Cargar variables desde el archivo .env
load_dotenv()

# Configuración de conexión
DB_USER = os.getenv("DB_USER")
DB_PASS = os.getenv("DB_PASS")
DB_HOST = os.getenv("DB_HOST")
DB_PORT = os.getenv("DB_PORT")
DB_NAME = os.getenv("DB_NAME")

# Validación básica
if not all([DB_USER, DB_PASS, DB_HOST, DB_PORT, DB_NAME]):
    raise ValueError("Faltan variables de entorno en el archivo .env")

# Crear engine global
DATABASE_URL = f"postgresql+psycopg2://{DB_USER}:{DB_PASS}@{DB_HOST}:{DB_PORT}/{DB_NAME}"
engine = create_engine(DATABASE_URL, pool_pre_ping=True)

def get_dataframe(query: str) -> pd.DataFrame:
    """
    Ejecuta un query SQL y lo devuelve como DataFrame de pandas.
    Usa el patrón correcto de SQLAlchemy 2.0+
    """
    try:
        # Usar contexto de conexión para ejecutar la query
        with engine.connect() as connection:
            df = pd.read_sql_query(text(query), connection)
            return df
    except Exception as e:
        print(f"Error ejecutando la query: {e}")
        return pd.DataFrame()

def test_connection():
    """
    Prueba la conexión a la base de datos.
    """
    try:
        with engine.connect() as connection:
            result = connection.execute(text("SELECT 1"))
            print("✓ Conexión exitosa a la base de datos")
            return True
    except Exception as e:
        print(f"✗ Error de conexión: {e}")
        return False