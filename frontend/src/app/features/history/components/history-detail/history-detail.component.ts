import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { HistoryDetailService } from '../../services/history-detail.service';
import { Entrevista } from '../../../../shared/models/entrevista.model';
import { Dialogo } from '../../../../shared/models/dialogo.model';
import { 
  Contexto,
  getEdadFromPromptSeed,
  getTagsFromPromptSeed,
  scopeToTags
} from '../../../../shared/models/contexto.model';

/**
 * Componente para visualizar el detalle completo de una entrevista.
 * Muestra: fecha, duración, contexto (título, descripción, tags) y transcripción completa.
 * Aplica las heurísticas de Nielsen enfocándose en usabilidad y experiencia agradable.
 */
@Component({
  selector: 'app-history-detail',
  imports: [...MATERIAL_IMPORTS, CommonModule],
  templateUrl: './history-detail.component.html',
  styleUrl: './history-detail.component.scss'
})
export class HistoryDetailComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private historyDetailService: HistoryDetailService
  ) {}

  // Heurística 1: Visibilidad del estado
  cargandoDatos = signal(true);

  // Datos de la entrevista
  interviewId = '';
  entrevista: Entrevista | null = null;
  contexto: Contexto | null = null;
  fechaEntrevista = '';
  duracionEntrevista = '';

  // Transcripción
  transcript: Dialogo[] = [];

  /* ============================================
   * DATOS COMENTADOS - No se usarán actualmente
   * ============================================
  
  // Evaluación
  evalEntrevista: EvalEntrevista = {
    _id: 1,
    entrevista_id: 1,
    score_final: 86,
    comentario_general: 'Buen desempeño general con áreas de mejora identificadas'
  };

  evalCategorias: EvalCategoria[] = [
    { _id: 1, eval_entrevista_id: 1, categoria: 'Inicio', score: 85 },
    { _id: 2, eval_entrevista_id: 1, categoria: 'Preguntas', score: 78 },
    { _id: 3, eval_entrevista_id: 1, categoria: 'Empatía', score: 92 },
    { _id: 4, eval_entrevista_id: 1, categoria: 'Cierre', score: 88 },
  ];

  positiveFeedback: FeedbackEntrevista[] = [
    {
      _id: 1,
      entrevista_id: 1,
      tipo: 'positivo',
      mensaje: 'Buen manejo de preguntas abiertas',
      categoria: 'Preguntas',
      fecha: '2024-08-15'
    },
    {
      _id: 2,
      entrevista_id: 1,
      tipo: 'positivo',
      mensaje: 'Cierre claro y respetuoso',
      categoria: 'Cierre',
      fecha: '2024-08-15'
    }
  ];

  improvementFeedback: FeedbackEntrevista[] = [
    {
      _id: 3,
      entrevista_id: 1,
      tipo: 'mejora',
      mensaje: 'Mejorar la validación emocional del padre',
      categoria: 'Empatía',
      fecha: '2024-08-15'
    }
  ];

  // Heurística 10: Ayuda y documentación
  tips: string[] = [
    'Mantén contacto visual y valida emociones explícitamente.',
    'Usa preguntas abiertas para explorar más detalles.',
    'Evita interrupciones largas, permite que el otro se exprese.',
  ];
  
  ============================================ */

  ngOnInit(): void {
    this.interviewId = this.route.snapshot.paramMap.get('id') || '';
    if (this.interviewId) {
      this.cargarDatos();
    }
  }

  /**
   * Carga los datos de la entrevista desde el backend
   * Heurística 1: Visibilidad del estado del sistema
   */
  private cargarDatos(): void {
    this.cargandoDatos.set(true);

    const entrevistaId = parseInt(this.interviewId);
    
    this.historyDetailService.getDetalleCompleto(entrevistaId).subscribe({
      next: ({ entrevista, dialogos, contexto }) => {
        this.entrevista = entrevista;
        this.contexto = contexto;
        this.transcript = dialogos;

        // Formatear fecha y duración
        if (entrevista?.fecha_creacion) {
          this.fechaEntrevista = this.formatearFecha(entrevista.fecha_creacion);
        }

        if (entrevista?.duracion_min) {
          this.duracionEntrevista = `${entrevista.duracion_min} min`;
        }

        this.cargandoDatos.set(false);
      },
      error: (error) => {
        console.error('Error al cargar datos de la entrevista:', error);
        this.cargandoDatos.set(false);
      }
    });
  }

  /**
   * Formatea una fecha ISO a formato legible
   */
  private formatearFecha(fechaISO: string): string {
    const fecha = new Date(fechaISO);
    const opciones: Intl.DateTimeFormatOptions = { 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    };
    return fecha.toLocaleDateString('es-ES', opciones);
  }

  /**
   * Navega de vuelta al historial
   * Heurística 3: Control y libertad del usuario
   */
  volverAlHistorial(): void {
    this.router.navigate(['/history']);
  }

  /**
   * Repite la entrevista con el mismo escenario
   * Heurística 3: Control del usuario
   */
  repetirEntrevista(): void {
    if (this.contexto) {
      // Guardar el contexto en sessionStorage para usarlo en la simulación
      sessionStorage.setItem('selectedContexto', JSON.stringify(this.contexto));
      this.router.navigate(['/simulation']);
    } else {
      // Si no hay contexto, ir a scenarios para que seleccione uno
      this.router.navigate(['/scenarios']);
    }
  }

  /**
   * Descarga el reporte en formato PDF
   * Heurística 7: Flexibilidad y eficiencia
   */
  descargarReporte(): void {
    if (!this.entrevista || !this.contexto) {
      console.error('No hay datos para descargar');
      return;
    }

    // Crear contenido del reporte
    const contenido = this.generarContenidoReporte();
    
    // Crear un blob con el contenido HTML
    const blob = new Blob([contenido], { type: 'text/html;charset=utf-8' });
    
    // Crear un enlace de descarga
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `Entrevista_${this.entrevista._id}_${new Date().toISOString().split('T')[0]}.html`;
    
    // Trigger de descarga
    document.body.appendChild(link);
    link.click();
    
    // Limpieza
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
    
    console.log('Reporte descargado exitosamente');
  }

  /**
   * Genera el contenido HTML del reporte para descarga
   */
  private generarContenidoReporte(): string {
    const tags = this.getAllTags().join(', ');
    const edad = this.getEdadFromContexto();
    
    let transcripcionTexto = '';
    this.transcript.forEach((dialogo, index) => {
      const emisor = dialogo.sender?.toLowerCase() === 'user' ? 'Estudiante' : 'Paciente (IA)';
      transcripcionTexto += `\n${emisor}: ${dialogo.texto}\n`;
    });

    return `
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <title>Reporte de Entrevista - ${this.entrevista?._id}</title>
</head>
<body>
  <h1>REPORTE DE ENTREVISTA</h1>
  <p>ID: ${this.entrevista?._id}</p>
  <p>Generado: ${new Date().toLocaleDateString('es-ES', { year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit' })}</p>
  
  <hr>
  
  <h2>INFORMACIÓN GENERAL</h2>
  <p><strong>Fecha:</strong> ${this.fechaEntrevista}</p>
  <p><strong>Duración:</strong> ${this.duracionEntrevista}</p>
  <p><strong>Número de turnos:</strong> ${this.transcript.length}</p>
  
  <hr>
  
  <h2>CONTEXTO DE LA ENTREVISTA</h2>
  <p><strong>Caso:</strong> ${this.contexto?.nombre}</p>
  <p><strong>Descripción:</strong> ${this.contexto?.descripcion}</p>
  ${edad !== 'N/A' ? `<p><strong>Edad:</strong> ${edad}</p>` : ''}
  ${tags ? `<p><strong>Características:</strong> ${tags}</p>` : ''}
  
  <hr>
  
  <h2>TRANSCRIPCIÓN COMPLETA</h2>
  <pre>${transcripcionTexto || 'No hay transcripción disponible.'}</pre>
  
  <hr>
  
  <p><em>NeuroPuentes - Sistema de Práctica de Entrevistas</em></p>
  <p><em>Este reporte es confidencial y debe ser tratado según las políticas de privacidad.</em></p>
</body>
</html>
    `;
  }

  /**
   * Obtiene la edad del contexto usando el helper
   */
  getEdadFromContexto(): string {
    return this.contexto ? getEdadFromPromptSeed(this.contexto) : 'N/A';
  }

  /**
   * Obtiene todos los tags combinados (scope + promptSeed)
   */
  getAllTags(): string[] {
    if (!this.contexto) return [];
    
    const scopeTags = scopeToTags(this.contexto.scope);
    const promptTags = getTagsFromPromptSeed(this.contexto);
    return [...scopeTags, ...promptTags];
  }

  /* ============================================
   * MÉTODOS COMENTADOS - No se usan actualmente
   * ============================================

  /**
   * Tooltip dinámico según el score
   * Heurística 6: Reconocer antes que recordar
   *
  tooltipScore(score: number | undefined): string {
    if (score === undefined) return 'Puntaje no disponible';
    if (score >= 85) return 'Excelente desempeño';
    if (score >= 70) return 'Buen desempeño';
    return 'Hay oportunidades de mejora';
  }

  /**
   * Color según el puntaje
   * Heurística 2: Correspondencia con el mundo real
   *
  getScoreColor(score: number | undefined): string {
    if (score === undefined) return 'warn';
    if (score >= 85) return 'success';
    if (score >= 70) return 'accent';
    return 'warn';
  }
  
  ============================================ */
}