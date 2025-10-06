import requests
import pandas as pd
from sklearn.linear_model import LinearRegression
from sklearn.metrics import r2_score
from datetime import datetime, timedelta
from dateutil.relativedelta import relativedelta

BACKEND_URL = "http://localhost:5299/api/StatsUsuario/resumen"
HEADERS = {"accept": "application/json"}

def get_stats(usuario_id, fecha_inicio, fecha_fin):
    params = {
        "usuarioId": usuario_id,
        "fechaInicio": fecha_inicio.strftime("%Y-%m-%d"),
        "fechaFin": fecha_fin.strftime("%Y-%m-%d")
    }
    try:
        resp = requests.get(BACKEND_URL, params=params, headers=HEADERS)
        resp.raise_for_status()
        return resp.json()
    except requests.exceptions.HTTPError as e:
        if resp.status_code == 404:
            # No hay datos en este rango de fechas
            return None
        else:
            # Otros errores se relanzan
            raise

def recolectar_stats_mensuales(usuario_id, fecha_inicio, fecha_fin):
    current = fecha_inicio
    df_list = []

    while current <= fecha_fin:
        # fin del mes actual
        next_month = (current.replace(day=28) + timedelta(days=4)).replace(day=1)
        fin_mes = min(next_month - timedelta(days=1), fecha_fin)

        print(f"📅 Procesando mes {current.strftime('%Y-%m')}...")

        stats = get_stats(usuario_id, current, fin_mes)
        if stats and stats.get('totalEntrevistas', 0) > 0:
            df_list.append(stats)

        # Avanzar al siguiente mes
        current = next_month

    if not df_list:
        print("⚠️ No se encontraron datos en el rango de fechas proporcionado.")
        return pd.DataFrame()  # DataFrame vacío si no hay datos

    return pd.DataFrame(df_list)

# --- Uso ---
usuario_id = 3
fecha_inicio = datetime(2024, 1, 1)
fecha_fin = datetime(2025, 10, 31)

df = recolectar_stats_mensuales(usuario_id, fecha_inicio, fecha_fin)
print(df)
