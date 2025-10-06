import os
import pandas as pd
from sqlalchemy import create_engine
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
engine = create_engine(f"postgresql+psycopg2://{DB_USER}:{DB_PASS}@{DB_HOST}:{DB_PORT}/{DB_NAME}")

def get_dataframe(query: str) -> pd.DataFrame:
    """
    Ejecuta un query SQL y lo devuelve como DataFrame de pandas.
    """
    try:
        return pd.read_sql(query, engine)
    except Exception as e:
        print(f"Error ejecutando la query: {e}")
        return pd.DataFrame()
