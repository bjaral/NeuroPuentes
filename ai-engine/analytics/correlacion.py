"""
Análisis de Correlación: Características de Contextos vs Desempeño por Categoría
"""

import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
import psycopg2
import os

# Configuración de base de datos
DB_CONFIG = {
    'host': 'localhost',
    'port': '5432',
    'database': 'NeuroPuentes',
    'user': 'postgres',
    'password': '1234'
}

OUTPUT_DIR = "analytics/media/correlacion"
os.makedirs(OUTPUT_DIR, exist_ok=True)

def obtener_datos():
    """Obtiene datos de entrevistas con características y evaluaciones"""
    conn = psycopg2.connect(**DB_CONFIG)
    
    query = """
    SELECT 
        e._id as entrevista_id,
        car.nombre as caracteristica,
        ec.categoria,
        ec.score as score_categoria
    FROM Entrevistas e
    INNER JOIN Contextos c ON e.contexto_id = c._id
    INNER JOIN Eval_Entrevista ee ON e._id = ee.entrevista_id
    INNER JOIN Eval_Categoria ec ON ee._id = ec.eval_entrevista_id
    LEFT JOIN Caracts_Rel cr ON c._id = cr.contexto_id
    LEFT JOIN Caracteristicas car ON cr.caracteristica_id = car._id
    WHERE c.vigencia = TRUE
    """
    
    df = pd.read_sql(query, conn)
    conn.close()
    return df

def preparar_matrices(df):
    """Crea matrices de características (binarias) y scores"""
    print("\n   DETALLE: Preparando matriz de características...")
    caracteristicas = df.dropna(subset=['caracteristica']).copy()
    matriz_caract = caracteristicas.pivot_table(
        index='entrevista_id',
        columns='caracteristica',
        aggfunc='size',
        fill_value=0
    ).clip(upper=1)
    print(f"   - {len(matriz_caract)} entrevistas × {len(matriz_caract.columns)} características")
    print(f"   - Características detectadas: {', '.join(matriz_caract.columns[:5].tolist())}...")
    
    print("\n   DETALLE: Preparando matriz de scores...")
    matriz_scores = df[['entrevista_id', 'categoria', 'score_categoria']].drop_duplicates()
    matriz_scores = matriz_scores.pivot(
        index='entrevista_id',
        columns='categoria',
        values='score_categoria'
    )
    print(f"   - {len(matriz_scores)} entrevistas × {len(matriz_scores.columns)} categorías")
    print(f"   - Categorías: {', '.join(matriz_scores.columns.tolist())}")
    
    return matriz_caract, matriz_scores

def analizar_correlaciones(matriz_caract, matriz_scores):
    """Calcula correlaciones entre cada característica y cada categoría"""
    resultados = []
    total_pares = len(matriz_scores.columns) * len(matriz_caract.columns)
    print(f"\n   DETALLE: Analizando {total_pares} combinaciones posibles...")
    
    for categoria in matriz_scores.columns:
        for caracteristica in matriz_caract.columns:
            df_temp = pd.DataFrame({
                'x': matriz_caract[caracteristica],
                'y': matriz_scores[categoria]
            }).dropna()
            
            if len(df_temp) < 2:
                continue
            
            # Calcular estadísticos básicos
            mean_x = df_temp['x'].mean()
            mean_y = df_temp['y'].mean()
            std_x = df_temp['x'].std(ddof=1)
            std_y = df_temp['y'].std(ddof=1)
            
            # Covarianza manual
            cov = ((df_temp['x'] - mean_x) * (df_temp['y'] - mean_y)).sum() / (len(df_temp) - 1)
            
            # Correlaciones
            pearson = df_temp['x'].corr(df_temp['y'], method='pearson')
            spearman = df_temp['x'].corr(df_temp['y'], method='spearman')
            
            resultados.append({
                'categoria': categoria,
                'caracteristica': caracteristica,
                'n': len(df_temp),
                'mean_caract': mean_x,
                'mean_score': mean_y,
                'std_caract': std_x,
                'std_score': std_y,
                'cov': cov,
                'pearson': pearson,
                'spearman': spearman
            })
    
    df_result = pd.DataFrame(resultados)
    print(f"   - {len(df_result)} pares válidos analizados")
    print(f"   - Correlación Pearson promedio: {df_result['pearson'].mean():.3f}")
    print(f"   - Correlación más fuerte: {df_result['pearson'].abs().max():.3f}")
    
    return df_result

def visualizar_matriz_correlacion(correlaciones, top_n=15):
    """Heatmap de correlaciones"""
    matriz = correlaciones.pivot(index='caracteristica', columns='categoria', values='pearson')
    
    if len(matriz) > top_n:
        top_features = matriz.abs().max(axis=1).nlargest(top_n).index
        matriz = matriz.loc[top_features]
    
    plt.figure(figsize=(12, 8))
    im = plt.imshow(matriz.values, vmin=-1, vmax=1, aspect='auto', cmap='RdBu_r')
    
    for i in range(len(matriz.index)):
        for j in range(len(matriz.columns)):
            val = matriz.iloc[i, j]
            if not np.isnan(val):
                color = 'black' if abs(val) < 0.5 else 'white'
                plt.text(j, i, f'{val:.2f}', ha='center', va='center', 
                        fontsize=8, color=color)
    
    plt.xticks(range(len(matriz.columns)), matriz.columns, rotation=45, ha='right')
    plt.yticks(range(len(matriz.index)), matriz.index)
    plt.title('Correlación (Pearson): Características vs Categorías de Desempeño', pad=20)
    plt.colorbar(im, label='Correlación')
    plt.tight_layout()
    plt.savefig(f'{OUTPUT_DIR}/01_matriz_correlacion.png', dpi=200, bbox_inches='tight')
    plt.close()

def visualizar_top_correlaciones(correlaciones, top_n=10):
    """Barras horizontales con top correlaciones positivas y negativas"""
    fig, axes = plt.subplots(1, 2, figsize=(14, 6))
    
    for idx, (data, titulo) in enumerate([
        (correlaciones.nlargest(top_n, 'pearson'), 'Positivas'),
        (correlaciones.nsmallest(top_n, 'pearson'), 'Negativas')
    ]):
        axes[idx].barh(range(len(data)), data['pearson'])
        axes[idx].set_yticks(range(len(data)))
        axes[idx].set_yticklabels(
            [f"{row['caracteristica'][:20]}\nvs {row['categoria']}" 
             for _, row in data.iterrows()], 
            fontsize=8
        )
        axes[idx].set_xlabel('Correlación de Pearson')
        axes[idx].set_title(f'Top {top_n} Correlaciones {titulo}')
        axes[idx].grid(axis='x', alpha=0.3)
    
    plt.tight_layout()
    plt.savefig(f'{OUTPUT_DIR}/02_top_correlaciones.png', dpi=200, bbox_inches='tight')
    plt.close()

def visualizar_dispersiones(matriz_caract, matriz_scores, correlaciones, n=6):
    """Diagramas de dispersión para las correlaciones más fuertes"""
    top = correlaciones.assign(abs_pearson=lambda x: x['pearson'].abs()).nlargest(n, 'abs_pearson')
    
    fig, axes = plt.subplots(2, 3, figsize=(15, 10))
    axes = axes.flatten()
    
    for idx, (_, row) in enumerate(top.iterrows()):
        if idx >= n:
            break
        
        df_temp = pd.DataFrame({
            'x': matriz_caract[row['caracteristica']],
            'y': matriz_scores[row['categoria']]
        }).dropna()
        
        x_jitter = df_temp['x'] + np.random.normal(0, 0.02, len(df_temp))
        
        axes[idx].scatter(x_jitter, df_temp['y'], alpha=0.6)
        axes[idx].set_xlabel(row['caracteristica'][:30], fontsize=9)
        axes[idx].set_ylabel(f"Score {row['categoria']}", fontsize=9)
        axes[idx].set_title(f"r = {row['pearson']:.3f}", fontsize=10)
        axes[idx].grid(True, alpha=0.3)
        axes[idx].set_xticks([0, 1])
        axes[idx].set_xticklabels(['No', 'Sí'])
    
    plt.suptitle('Diagramas de Dispersión: Características vs Desempeño', fontsize=14)
    plt.tight_layout()
    plt.savefig(f'{OUTPUT_DIR}/03_dispersiones.png', dpi=200, bbox_inches='tight')
    plt.close()

def main():
    print("=" * 70)
    print("ANÁLISIS DE CORRELACIÓN: Características de Contextos vs Desempeño")
    print("=" * 70)
    
    print("\n[1/5] Obteniendo datos...")
    df = obtener_datos()
    
    if df.empty:
        print("No hay datos disponibles.")
        return
    
    print(f"   {len(df)} registros | {df['entrevista_id'].nunique()} entrevistas")
    
    print("\n[2/5] Preparando matrices...")
    matriz_caract, matriz_scores = preparar_matrices(df)
    print(f"   Características: {matriz_caract.shape} | Scores: {matriz_scores.shape}")
    
    print("\n[3/5] Calculando correlaciones...")
    correlaciones = analizar_correlaciones(matriz_caract, matriz_scores)
    
    print(f"\n   ESTADÍSTICAS GENERALES:")
    print(f"   - Total de pares analizados: {len(correlaciones)}")
    print(f"   - Rango Pearson: [{correlaciones['pearson'].min():.3f}, {correlaciones['pearson'].max():.3f}]")
    print(f"   - Media Pearson: {correlaciones['pearson'].mean():.3f}")
    print(f"   - Desviación estándar: {correlaciones['pearson'].std():.3f}")
    
    print(f"\n   TOP 5 CORRELACIONES POSITIVAS:")
    top5_pos = correlaciones.nlargest(5, 'pearson')
    for i, (_, row) in enumerate(top5_pos.iterrows(), 1):
        print(f"   {i}. {row['caracteristica'][:35]:35} vs {row['categoria']:15} | r={row['pearson']:6.3f} (n={row['n']:3})")
    
    print(f"\n   TOP 5 CORRELACIONES NEGATIVAS:")
    top5_neg = correlaciones.nsmallest(5, 'pearson')
    for i, (_, row) in enumerate(top5_neg.iterrows(), 1):
        print(f"   {i}. {row['caracteristica'][:35]:35} vs {row['categoria']:15} | r={row['pearson']:6.3f} (n={row['n']:3})")
    
    print("\n[4/5] Generando visualizaciones...")
    print("   - Creando matriz de correlación (heatmap)...")
    visualizar_matriz_correlacion(correlaciones)
    print("   - Creando gráfico de barras (top positivas/negativas)...")
    visualizar_top_correlaciones(correlaciones)
    print("   - Creando diagramas de dispersión (top 6)...")
    visualizar_dispersiones(matriz_caract, matriz_scores, correlaciones)
    print("   ✓ 3 gráficos guardados")
    
    print("\n[5/5] Guardando resultados...")
    output_file = f'{OUTPUT_DIR}/correlaciones_completas.csv'
    correlaciones.sort_values('pearson', ascending=False).to_csv(output_file, index=False)
    print(f"   ✓ CSV guardado: {output_file}")
    print(f"   - Columnas: {', '.join(correlaciones.columns.tolist())}")
    print(f"   - Total filas: {len(correlaciones)}")
    
    print("\n" + "=" * 70)
    print("ANÁLISIS COMPLETADO")
    print("=" * 70)
    print(f"\nArchivos generados en: {OUTPUT_DIR}/")
    print("  - 01_matriz_correlacion.png")
    print("  - 02_top_correlaciones.png")
    print("  - 03_dispersiones.png")
    print("  - correlaciones_completas.csv")
    
    print("\nDISTRIBUCIÓN DE CORRELACIONES:")
    abs_corr = correlaciones['pearson'].abs()
    total = len(correlaciones)
    fuerte = (abs_corr > 0.7).sum()
    moderada = ((abs_corr > 0.4) & (abs_corr <= 0.7)).sum()
    debil = (abs_corr <= 0.4).sum()
    
    print(f"  Fuertes    (|r| > 0.70): {fuerte:3} ({fuerte/total*100:5.1f}%)")
    print(f"  Moderadas  (|r| 0.4-0.7): {moderada:3} ({moderada/total*100:5.1f}%)")
    print(f"  Débiles    (|r| < 0.40): {debil:3} ({debil/total*100:5.1f}%)")
    print(f"  {'─'*50}")
    print(f"  Total:                   {total:3} (100.0%)")
    
    print("\nINTERPRETACIÓN:")
    print("  • r cercano a +1: La característica está asociada con mayor desempeño")
    print("  • r cercano a -1: La característica está asociada con menor desempeño")
    print("  • r cercano a 0:  No hay relación lineal aparente")
    print("=" * 70)

if __name__ == "__main__":
    try:
        main()
    except Exception as e:
        print(f"\nError: {e}")
        import traceback
        traceback.print_exc()