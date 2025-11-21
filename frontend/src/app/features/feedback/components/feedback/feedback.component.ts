import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { EvalCategoria, EvalEntrevista } from '../../../../shared/models/eval.model';
import { FeedbackEntrevista } from '../../../../shared/models/feedback.model';

/**
 * Interface para datos históricos de comparativa
 */
interface DatoHistorico {
  fecha: string;
  score: number;
}

/**
 * Componente de resultados de evaluación
 * Muestra feedback inmediato después de completar una entrevista
 * Aplica heurísticas de Nielsen para visualización clara de resultados
 */
@Component({
  selector: 'app-feedback',
  imports: [MATERIAL_IMPORTS, RouterModule, CommonModule],
  templateUrl: './feedback.component.html',
  styleUrl: './feedback.component.scss'
})
export class FeedbackComponent implements OnInit {

  constructor(private router: Router) {}

  // Heurística 1: Visibilidad del estado
  cargandoDatos = signal(true);

  // Datos de evaluación actual
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

  // Datos históricos para comparativa
  datosHistoricos: DatoHistorico[] = [
    { fecha: '01/08', score: 80 },
    { fecha: '05/08', score: 90 },
    { fecha: '10/08', score: 82 },
    { fecha: '15/08', score: 86 } // Score actual
  ];

  ngOnInit(): void {
    this.cargarDatos();
  }

  /**
   * Simula carga de datos de evaluación
   * Heurística 1: Visibilidad del estado
   */
  private cargarDatos(): void {
    setTimeout(() => {
      this.cargandoDatos.set(false);
    }, 600);
  }

  /**
   * Navega para repetir la entrevista
   * Heurística 3: Control y libertad
   */
  repetirEntrevista(): void {
    this.router.navigate(['/scenarios']);
  }

  /**
   * Navega al dashboard
   * Heurística 3: Control y libertad
   */
  volverDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  /**
   * Navega al historial
   * Heurística 7: Flexibilidad
   */
  verHistorial(): void {
    this.router.navigate(['/history']);
  }

  /**
   * Color según el puntaje
   * Heurística 2: Correspondencia con el mundo real
   */
  getScoreColor(score: number | undefined): string {
    if (!score) return '';
    if (score >= 85) return 'success';
    if (score >= 70) return 'accent';
    return 'warn';
  }

  /**
   * Tooltip según el puntaje
   * Heurística 6: Reconocer antes que recordar
   */
  tooltipScore(score: number): string {
    if (score >= 85) return 'Excelente desempeño';
    if (score >= 70) return 'Buen desempeño';
    return 'Hay oportunidades de mejora';
  }

  /**
   * Analiza tendencia de puntajes
   * Heurística 2: Información significativa
   */
  getTendencia(): 'mejorando' | 'estable' | 'bajando' {
    if (this.datosHistoricos.length < 2) return 'estable';
    
    const scoreActual = this.datosHistoricos[this.datosHistoricos.length - 1].score;
    const scoreAnterior = this.datosHistoricos[this.datosHistoricos.length - 2].score;
    
    if (scoreActual > scoreAnterior + 3) return 'mejorando';
    if (scoreActual < scoreAnterior - 3) return 'bajando';
    return 'estable';
  }

  /**
   * Mensaje de tendencia
   * Heurística 10: Ayuda y documentación
   */
  getMensajeTendencia(): string {
    const tendencia = this.getTendencia();
    
    if (tendencia === 'mejorando') {
      return '¡Vas en ascenso! Sigue practicando';
    } else if (tendencia === 'bajando') {
      return 'Revisa las áreas de mejora sugeridas';
    } else {
      return 'Te mantienes consistente';
    }
  }

  /**
   * Icono de tendencia
   * Heurística 2: Visualización significativa
   */
  getIconoTendencia(): string {
    const tendencia = this.getTendencia();
    
    if (tendencia === 'mejorando') return 'trending_up';
    if (tendencia === 'bajando') return 'trending_down';
    return 'trending_flat';
  }
}