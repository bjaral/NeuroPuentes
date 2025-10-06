# === regresion_multiple.py ===
import requests
import pandas as pd
import numpy as np
from datetime import datetime, timedelta
from dateutil.relativedelta import relativedelta

# --- Configuración Backend ---
BACKEND_URL = "http://localhost:5299/api/StatsUsuario/resumen"
HEADERS = {"accept": "application/json"}

# --- Función para obtener stats de un rango de fechas ---
def get_stats(usuario_id, fecha_inicio, fecha_fin):
    params = {
        "usuarioId": usuario_id,
        "fechaInicio": fecha_inicio.strftime("%Y-%m-%d"),
        "fechaFin": fecha_fin.strftime("%Y-%m-%d")
    }
    try:
        resp = requests.get(BACKEND_URL, params=params, headers=HEADERS)
        if resp.status_code == 404:
            # No hay datos en este rango → retornar None
            return None
        resp.raise_for_status()
        return resp.json()
    except requests.RequestException as e:
        print(f"⚠️ Error al consultar {fecha_inicio} - {fecha_fin}: {e}")
        return None

# --- Función para recolectar stats mensuales ---
def recolectar_stats_mensuales(usuario_id, fecha_inicio, fecha_fin):
    current = fecha_inicio
    df_list = []

    while current <= fecha_fin:
        next_month = (current.replace(day=28) + timedelta(days=4)).replace(day=1)
        fin_mes = min(next_month - timedelta(days=1), fecha_fin)

        stats = get_stats(usuario_id, current, fin_mes)
        if stats and stats.get("totalEntrevistas", 0) > 0:
            df_list.append(stats)

        current = next_month

    if not df_list:
        print("No se encontraron datos para este usuario en el rango indicado.")
        return pd.DataFrame()

    return pd.DataFrame(df_list)

# --- Función de regresión múltiple ---
def regresion_multiple(df, features, target):
    # Matriz de diseño X con columna de 1s para intercepto
    X = np.column_stack([np.ones(len(df))] + [df[f].values for f in features])
    y = df[target].values

    # Resolver mínimos cuadrados
    beta, residuals, rank, s = np.linalg.lstsq(X, y, rcond=None)
    intercept = beta[0]
    coefs = beta[1:]

    # Predicciones y R²
    y_hat = X @ beta
    ss_res = np.sum((y - y_hat)**2)
    ss_tot = np.sum((y - np.mean(y))**2)
    r2 = 1 - ss_res / ss_tot if ss_tot != 0 else 0.0

    print(f"=== REGRESIÓN MÚLTIPLE: {target} ~ {', '.join(features)} ===")
    print(f"Intercepto: {intercept:.6f}")
    for f, c in zip(features, coefs):
        print(f"Coeficiente {f}: {c:.6f}")
    print(f"R²: {r2:.6f}")
    return intercept, coefs, r2

# --- Uso del script ---
if __name__ == "__main__":
    usuario_id = 3
    fecha_inicio = datetime(2024, 1, 1)
    fecha_fin = datetime(2025, 12, 31)

    print("📅 Recolectando datos mensuales...")
    df = recolectar_stats_mensuales(usuario_id, fecha_inicio, fecha_fin)
    if df.empty:
        exit(0)

    print("📊 Datos recolectados:")
    print(df)

    # --- Elegir features y variable objetivo ---
    features = [
        "totalEntrevistas",
        "tiempoTotalMin",
        "numTurnosPromedio",
        "feedbackFortalezas",
        "feedbackDebilidades",
        "contextosDificiles",
        "contextosFaciles"
    ]
    target = "scorePromedio"

    # Ejecutar regresión múltiple
    regresion_multiple(df, features, target)
