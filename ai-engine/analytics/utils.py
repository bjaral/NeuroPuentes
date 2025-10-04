import pandas as pd
from sqlalchemy import create_engine

# Configuración de conexión
DB_USER = "postgres"
DB_PASS = "tu_password"
DB_HOST = "localhost"
DB_PORT = "5432"
DB_NAME = "neuro_puentes_db"

# Crear engine global
engine = create_engine(f"postgresql+psycopg2://{DB_USER}:{DB_PASS}@{DB_HOST}:{DB_PORT}/{DB_NAME}")

def get_dataframe(query: str) -> pd.DataFrame:
    """
    Ejecuta un query SQL y lo devuelve como DataFrame de pandas.
    """
    return pd.read_sql(query, engine)
