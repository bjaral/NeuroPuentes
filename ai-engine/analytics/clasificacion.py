"""
Script de Análisis de Evaluaciones - NeuroPuentes
Analiza el desempeño de un usuario específico en sus entrevistas
"""

import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
from sqlalchemy import create_engine
from datetime import datetime

# Colores en consola (opcional)
try:
    from colorama import init, Fore, Style
    init()
    USAR_COLORES = True
except ImportError:
    USAR_COLORES = False
    print("Tip: Instala colorama para colores en consola: pip install colorama\n")

# =====================================================================
# CONFIGURACIÓN
# =====================================================================
DB_USER = "postgres"
DB_PASS = "admin"   # Cambiar según la contraseña de cada uno
DB_HOST = "localhost"
DB_PORT = "5432"
DB_NAME = "neuropuentes"

USUARIO_ID = 3
UMBRAL_APROBACION = 60

# =====================================================================
# FUNCIONES AUXILIARES
# =====================================================================

def color_texto(texto, color='verde'):
    if not USAR_COLORES:
        return texto
    colores = {'verde': Fore.GREEN, 'rojo': Fore.RED, 'amarillo': Fore.YELLOW, 'reset': Style.RESET_ALL}
    return f"{colores.get(color, '')}{texto}{colores['reset']}"

def conectar_bd():
    try:
        engine = create_engine(
            f"postgresql+psycopg2://{DB_USER}:{DB_PASS}@{DB_HOST}:{DB_PORT}/{DB_NAME}"
        )
        pd.read_sql("SELECT 1", engine)  # Test conexión
        print(color_texto("Conexión exitosa a la base de datos\n", 'verde'))
        return engine
    except Exception as e:
        print(color_texto(f"Error de conexión: {e}", 'rojo'))
        print("\nUsando datos de ejemplo (CSV)...")
        return None

def cargar_datos_bd(engine, usuario_id):
    # stats_usuario: obtenemos estadísticas del usuario
    query_stats = f"""
    SELECT _id, usuario_id, fecha_corte, total_entrevistas, tiempo_total_min, score_promedio
    FROM stats_usuario
    WHERE usuario_id = {usuario_id}
    ORDER BY fecha_corte DESC
    LIMIT 1
    """
    df_stats = pd.read_sql(query_stats, engine)
    
    # eval_entrevista: obtenemos todas las entrevistas
    df_eval = pd.read_sql("SELECT _id, entrevista_id, score_final FROM eval_entrevista ORDER BY _id", engine)
    
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
        color = 'verde' if row['clasificacion'] == 'Aprobado' else 'rojo'
        print(f"Entrevista ID: {row['entrevista_id']} | Score: {row['score_final']:.2f} | {color_texto(row['clasificacion'], color)}")
    print("="*70 + "\n")

def mostrar_resumen_usuario(df_stats, estado_general):
    print("="*70)
    print("RESUMEN GENERAL DEL USUARIO")
    print("="*70)
    if len(df_stats) == 0:
        print(color_texto("No hay estadísticas disponibles para este usuario", 'amarillo'))
        return
    stats = df_stats.iloc[0]
    print(f"Usuario ID: {stats['usuario_id']}")
    print(f"Total de entrevistas: {stats['total_entrevistas']}")
    print(f"Tiempo total invertido: {stats['tiempo_total_min']:.2f} minutos")
    print(f"Score promedio: {stats['score_promedio']:.2f}")
    color = 'verde' if estado_general == 'Va aprobando' else 'rojo'
    print(f"Estado general: {color_texto(estado_general, color)}")
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
    print(f"Entrevistas aprobadas: {color_texto(str(aprobadas), 'verde')}")
    print(f"Entrevistas que necesitan mejora: {color_texto(str(reprobadas), 'rojo')}")
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
    filename = f'analisis_usuario_{usuario_id}_{timestamp}.csv'
    df_consolidado.to_csv(filename, index=False)
    print(color_texto(f"Datos exportados a: {filename}", 'verde'))
    return filename

def generar_graficos(df_eval, usuario_id):
    sns.set_style('whitegrid')
    plt.figure(figsize=(10,6))
    plt.hist(df_eval['score_final'], bins=10, edgecolor='black', alpha=0.7)
    plt.axvline(UMBRAL_APROBACION, color='red', linestyle='--', linewidth=2, label=f'Umbral ({UMBRAL_APROBACION})')
    plt.title(f'Distribución de Scores - Usuario {usuario_id}')
    plt.xlabel('Score Final')
    plt.ylabel('Frecuencia')
    plt.legend()
    plt.show()

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
        print(color_texto("No se encontraron datos para analizar", 'rojo'))
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
    
    print(color_texto("\nAnálisis completado exitosamente", 'verde'))
    print("="*70 + "\n")

if __name__ == "__main__":
    main()
