"""
Script de Análisis de Evaluaciones - NeuroPuentes
Analiza el desempeño de un usuario específico en sus entrevistas
"""

import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import psycopg2 
from datetime import datetime
import os
from dotenv import load_dotenv

# Cargar variables de entorno
load_dotenv()

# =====================================================================
# CONFIGURACIÓN
# =====================================================================
DB_USER = "postgres"
DB_PASS = "admin"   # Cambiar según tu contraseña
DB_HOST = "localhost"
DB_PORT = "5432"
DB_NAME = "neuropuentes"

USUARIO_ID = 3
UMBRAL_APROBACION = 60
OUTPUT_DIR = "analytics/media/clasificacion"

# Crear directorio de salida
os.makedirs(OUTPUT_DIR, exist_ok=True)

# =====================================================================
# FUNCIONES AUXILIARES
# =====================================================================

def conectar_bd():
    """
    Intenta conectarse a la base de datos usando psycopg2,
    la misma librería usada en tu primer script.
    """
    try:
        # Intenta crear la conexión directamente con psycopg2
        conn = psycopg2.connect(**DB_CONFIG)
        conn.close() # Cierra la conexión de prueba
        print("Conexión exitosa a la base de datos\n")
        return DB_CONFIG # Devuelve la configuración para usarla en pandas
    except Exception as e:
        print(f"Error de conexión: {e}")
        print("\nUsando datos de ejemplo (CSV)...")
        return None

def cargar_datos_bd(db_config, usuario_id):
    # Se usa pd.read_sql y se le pasa el diccionario de conexión a través de psycopg2
    conn = psycopg2.connect(**db_config)
        
    # stats_usuario: obtenemos estadísticas del usuario
    query_stats = f"""
    SELECT _id, usuario_id, fecha_corte, total_entrevistas, tiempo_total_min, score_promedio
    FROM stats_usuario
    WHERE usuario_id = {usuario_id}
    ORDER BY fecha_corte DESC
    LIMIT 1
    """
    df_stats = pd.read_sql(query_stats, conn)

    df_eval = pd.read_sql("SELECT _id, entrevista_id, score_final FROM eval_entrevista ORDER BY _id", conn)

    conn.close()
    return df_eval, df_stats

def clasificar_entrevistas(df_eval, umbral=60):
    df_eval['clasificacion'] = df_eval['score_final'].apply(lambda x: 'Aprobado' if x >= umbral else 'Necesita mejora')
    return df_eval

def obtener_estado_general(score_promedio, umbral=60):
    return 'Va aprobando' if score_promedio >= umbral else 'Necesita mejorar'

def mostrar_resumen_entrevistas(df_eval):
    print("="*70)
    print("RESUMEN POR ENTREVISTA")
    print("="*70)
    for idx, row in df_eval.iterrows():
        print(f"Entrevista ID: {row['entrevista_id']} | Score: {row['score_final']:.2f} | {row['clasificacion']}")
    print("="*70 + "\n")

def mostrar_resumen_usuario(df_stats, estado_general):
    print("="*70)
    print("RESUMEN GENERAL DEL USUARIO")
    print("="*70)
    if len(df_stats) == 0:
        print("No hay estadísticas disponibles para este usuario")
        return
    stats = df_stats.iloc[0]
    print(f"Usuario ID: {stats['usuario_id']}")
    print(f"Total de entrevistas: {stats['total_entrevistas']}")
    print(f"Tiempo total invertido: {stats['tiempo_total_min']:.2f} minutos")
    print(f"Score promedio: {stats['score_promedio']:.2f}")
    print(f"Estado general: {estado_general}")
    print(f"Última actualización: {stats['fecha_corte']}")
    print("="*70 + "\n")

def generar_estadisticas_adicionales(df_eval):
    print("="*70)
    print("ESTADÍSTICAS ADICIONALES")
    print("="*70)
    total = len(df_eval)
    aprobadas = len(df_eval[df_eval['clasificacion'] == 'Aprobado'])
    reprobadas = total - aprobadas
    tasa_aprobacion = (aprobadas / total * 100) if total > 0 else 0
    print(f"Total de entrevistas: {total}")
    print(f"Entrevistas aprobadas: {aprobadas}")
    print(f"Entrevistas que necesitan mejora: {reprobadas}")
    print(f"Tasa de aprobación: {tasa_aprobacion:.1f}%")
    print(f"Score más alto: {df_eval['score_final'].max():.2f}")
    print(f"Score más bajo: {df_eval['score_final'].min():.2f}")
    print(f"Desviación estándar: {df_eval['score_final'].std():.2f}")
    print("="*70 + "\n")

def consolidar_datos(df_eval, df_stats):
    if len(df_stats) > 0:
        stats = df_stats.iloc[0]
        df_eval['total_entrevistas_usuario'] = stats['total_entrevistas']
        df_eval['tiempo_total_min'] = stats['tiempo_total_min']
        df_eval['score_promedio_usuario'] = stats['score_promedio']
        df_eval['estado_general'] = obtener_estado_general(stats['score_promedio'])
    else:
        df_eval['total_entrevistas_usuario'] = len(df_eval)
        df_eval['tiempo_total_min'] = 0
        df_eval['score_promedio_usuario'] = df_eval['score_final'].mean()
        df_eval['estado_general'] = obtener_estado_general(df_eval['score_final'].mean())
    return df_eval

def exportar_csv(df_consolidado, usuario_id):
    timestamp = datetime.now().strftime('%Y%m%d_%H%M%S')
    filename = f'{OUTPUT_DIR}/analisis_usuario_{usuario_id}_{timestamp}.csv'
    df_consolidado.to_csv(filename, index=False)
    print(f"Datos exportados a: {filename}")
    return filename

def generar_graficos(df_eval, usuario_id):
    sns.set_style('whitegrid')
    
    # Gráfico 1: Distribución de scores
    plt.figure(figsize=(10, 6))
    plt.hist(df_eval['score_final'], bins=10, edgecolor='black', alpha=0.7, color='steelblue')
    plt.axvline(UMBRAL_APROBACION, color='red', linestyle='--', linewidth=2, label=f'Umbral ({UMBRAL_APROBACION})')
    plt.title(f'Distribución de Scores - Usuario {usuario_id}')
    plt.xlabel('Score Final')
    plt.ylabel('Frecuencia')
    plt.legend()
    plt.grid(True, alpha=0.3)
    plt.tight_layout()
    plt.savefig(f'{OUTPUT_DIR}/01_distribucion_scores.png', dpi=200)
    plt.close()
    
    # Gráfico 2: Clasificación de entrevistas
    plt.figure(figsize=(8, 6))
    clasificacion_counts = df_eval['clasificacion'].value_counts()
    colors = ['#2ecc71', '#e74c3c']  # Verde para aprobado, rojo para necesita mejora
    plt.pie(clasificacion_counts.values, labels=clasificacion_counts.index, autopct='%1.1f%%', 
            colors=colors, startangle=90)
    plt.title(f'Clasificación de Entrevistas - Usuario {usuario_id}')
    plt.tight_layout()
    plt.savefig(f'{OUTPUT_DIR}/02_clasificacion_entrevistas.png', dpi=200)
    plt.close()
    
    # Gráfico 3: Scores por entrevista (ordenadas)
    plt.figure(figsize=(12, 6))
    df_sorted = df_eval.sort_values('score_final')
    plt.bar(range(len(df_sorted)), df_sorted['score_final'], 
            color=['#2ecc71' if x >= UMBRAL_APROBACION else '#e74c3c' for x in df_sorted['score_final']])
    plt.axhline(y=UMBRAL_APROBACION, color='red', linestyle='--', linewidth=2, label=f'Umbral ({UMBRAL_APROBACION})')
    plt.title(f'Scores por Entrevista (Ordenadas) - Usuario {usuario_id}')
    plt.xlabel('Entrevista (Ordenada por Score)')
    plt.ylabel('Score Final')
    plt.legend()
    plt.grid(True, alpha=0.3, axis='y')
    plt.tight_layout()
    plt.savefig(f'{OUTPUT_DIR}/03_scores_entrevistas.png', dpi=200)
    plt.close()

# =====================================================================
# MAIN
# =====================================================================
def main():
    print("="*70)
    print("ANÁLISIS DE EVALUACIONES - NEUROPUENTES")
    print("="*70)
    print(f"Analizando usuario ID: {USUARIO_ID}")
    print(f"Umbral de aprobación: {UMBRAL_APROBACION}")
    print("="*70 + "\n")
    
    engine = conectar_bd()
    if engine:
        df_eval, df_stats = cargar_datos_bd(engine, USUARIO_ID)
    else:
        print("No hay engine de BD. Cargar CSV manualmente aquí...")
        return
    
    if df_eval is None or len(df_eval) == 0:
        print("No se encontraron datos para analizar")
        return
    
    df_eval = clasificar_entrevistas(df_eval, UMBRAL_APROBACION)
    
    score_promedio = df_stats.iloc[0]['score_promedio'] if len(df_stats) > 0 else df_eval['score_final'].mean()
    estado_general = obtener_estado_general(score_promedio, UMBRAL_APROBACION)
    
    mostrar_resumen_entrevistas(df_eval)
    mostrar_resumen_usuario(df_stats, estado_general)
    generar_estadisticas_adicionales(df_eval)
    
    df_consolidado = consolidar_datos(df_eval, df_stats)
    exportar_csv(df_consolidado, USUARIO_ID)
    generar_graficos(df_eval, USUARIO_ID)
    
    print("="*70)
    print(f"Gráficos y datos exportados a: {OUTPUT_DIR}/")
    print("="*70)
    print("\nAnálisis completado exitosamente")
    print("="*70 + "\n")

if __name__ == "__main__":
    try:
        main()
    except Exception as e:
        print(f"Error: {e}")