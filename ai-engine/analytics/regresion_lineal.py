"""
Regresión Lineal: Análisis de evolución de scores del usuario
"""

import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from sklearn.linear_model import LinearRegression
from sklearn.metrics import r2_score, mean_squared_error, mean_absolute_error
import psycopg2
import os
from dotenv import load_dotenv

# Cargar variables de entorno
load_dotenv()

# Configuración desde .env
DB_CONFIG = {
    'host': os.getenv('DB_HOST'),
    'port': os.getenv('DB_PORT'),
    'database': os.getenv('DB_NAME'),
    'user': os.getenv('DB_USER'),
    'password': os.getenv('DB_PASS')
}
USUARIO_ID = 3
OUTPUT_DIR = "analytics/media/regresion_lineal"

# Crear directorio de salida
os.makedirs(OUTPUT_DIR, exist_ok=True)

def main():
    print("="*60)
    print("REGRESIÓN LINEAL - Usuario ID:", USUARIO_ID)
    print("="*60)
    
    # Conectar y cargar datos
    conn = psycopg2.connect(**DB_CONFIG)
    query = f"""
        SELECT fecha_corte, score_promedio 
        FROM stats_usuario 
        WHERE usuario_id = {USUARIO_ID} 
        ORDER BY fecha_corte
    """
    df = pd.read_sql(query, conn)
    conn.close()
    
    if df.empty:
        print("No hay datos disponibles")
        return
    
    # Preparar datos
    df['fecha_corte'] = pd.to_datetime(df['fecha_corte'])
    df['dias'] = (df['fecha_corte'] - df['fecha_corte'].min()).dt.days
    X = df[['dias']].values
    y = df['score_promedio'].values
    
    print(f"Registros: {len(df)}")
    print(f"Score inicial: {y[0]:.2f}% → Score final: {y[-1]:.2f}%\n")
    
    # Ajustar modelo
    modelo = LinearRegression()
    modelo.fit(X, y)
    y_pred = modelo.predict(X)
    
    beta0, beta1 = modelo.intercept_, modelo.coef_[0]
    r2 = r2_score(y, y_pred)
    mse = mean_squared_error(y, y_pred)
    
    print("MODELO")
    print(f"  β₀ (intercepto): {beta0:.3f}%")
    print(f"  β₁ (pendiente):  {beta1:.4f}% por día")
    print(f"  Mejora mensual:  {beta1 * 30:.2f}%")
    print(f"\nMÉTRICAS")
    print(f"  R²:   {r2:.4f}")
    print(f"  MSE:  {mse:.4f}")
    print(f"  RMSE: {np.sqrt(mse):.4f}\n")
    
    # Gráfico 1: Dispersión
    plt.figure(figsize=(10, 5))
    plt.scatter(df['dias'], y, s=80, alpha=0.6, c='steelblue')
    plt.title('Score vs Tiempo')
    plt.xlabel('Días desde inicio')
    plt.ylabel('Score (%)')
    plt.grid(True, alpha=0.3)
    plt.tight_layout()
    plt.savefig(f'{OUTPUT_DIR}/01_dispersion.png', dpi=200)
    plt.close()
    
    # Gráfico 2: Regresión
    plt.figure(figsize=(10, 5))
    plt.scatter(df['dias'], y, s=80, alpha=0.6, c='steelblue', label='Datos')
    plt.plot(df['dias'], y_pred, 'r-', linewidth=2, label='Regresión')
    plt.title(f'Score = {beta0:.2f} + {beta1:.4f} × Días')
    plt.xlabel('Días desde inicio')
    plt.ylabel('Score (%)')
    plt.legend()
    plt.grid(True, alpha=0.3)
    plt.tight_layout()
    plt.savefig(f'{OUTPUT_DIR}/02_regresion.png', dpi=200)
    plt.close()
    
    # Gráfico 3: Residuales
    residuales = y - y_pred
    fig, axes = plt.subplots(1, 2, figsize=(12, 4))
    
    axes[0].scatter(df['dias'], residuales, s=60, alpha=0.6, c='coral')
    axes[0].axhline(0, color='black', linestyle='--')
    axes[0].set_title('Residuales vs Días')
    axes[0].set_xlabel('Días')
    axes[0].set_ylabel('Residual')
    axes[0].grid(True, alpha=0.3)
    
    axes[1].hist(residuales, bins=10, color='coral', alpha=0.7, edgecolor='black')
    axes[1].axvline(0, color='black', linestyle='--')
    axes[1].set_title('Distribución de Residuales')
    axes[1].set_xlabel('Residual')
    axes[1].set_ylabel('Frecuencia')
    axes[1].grid(True, alpha=0.3, axis='y')
    
    plt.tight_layout()
    plt.savefig(f'{OUTPUT_DIR}/03_residuales.png', dpi=200)
    plt.close()
    
    print("="*60)
    print(f"Gráficos guardados en: {OUTPUT_DIR}/")
    print("="*60)

if __name__ == "__main__":
    try:
        main()
    except Exception as e:
        print(f"Error: {e}")