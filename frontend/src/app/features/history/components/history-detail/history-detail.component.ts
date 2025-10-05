import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { FeedbackEntrevista } from '../../../../shared/models/feedback.model';
import { Dialogo } from '../../../../shared/models/dialogo.model';
import { EvalCategoria, EvalEntrevista } from '../../../../shared/models/eval.model';

/**
 * Componente para visualizar el detalle completo de una entrevista.
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
    private router: Router
  ) {}

  // Heurística 1: Visibilidad del estado
  cargandoDatos = signal(true);

  // Datos de la entrevista
  interviewId = '';
  interviewTitle = 'Entrevista con madre de niño de 8 años';
  interviewDescription = 'Simulación de entrevista inicial para evaluar comunicación empática y preguntas abiertas.';
  tags = ['Empatía', 'Autismo', 'Entrevista inicial'];
  fechaEntrevista = '15 de Marzo, 2024';
  duracionEntrevista = '32 min';

  // Transcripción
  transcript: Dialogo[] = [
    { sender: 'user', texto: 'Buenos días, ¿cómo se encuentra hoy?' },
    { sender: 'ai', texto: 'Buenos días, me siento un poco preocupada por mi hijo.' },
    { sender: 'user', texto: 'Entiendo, ¿qué es lo que más le preocupa?' },
    { sender: 'ai', texto: 'Su desempeño en la escuela y cómo se relaciona con otros niños.' },
  ];

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

  ngOnInit(): void {
    this.interviewId = this.route.snapshot.paramMap.get('id') || '';
    this.cargarDatos();
  }

  /**
   * Simula carga de datos
   * Heurística 1: Visibilidad del estado del sistema
   */
  private cargarDatos(): void {
    // Simulación de carga
    setTimeout(() => {
      this.cargandoDatos.set(false);
    }, 600);
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
    this.router.navigate(['/scenarios']);
  }

  /**
   * Descarga el reporte
   * Heurística 7: Flexibilidad y eficiencia
   */
  descargarReporte(): void {
    console.log('Descargando reporte...');
    // Implementación real aquí
  }

  /**
   * Tooltip dinámico según el score
   * Heurística 6: Reconocer antes que recordar
   */
  tooltipScore(score: number | undefined): string {
    if (score === undefined) return 'Puntaje no disponible';
    if (score >= 85) return 'Excelente desempeño';
    if (score >= 70) return 'Buen desempeño';
    return 'Hay oportunidades de mejora';
  }

  /**
   * Color según el puntaje
   * Heurística 2: Correspondencia con el mundo real
   */
  getScoreColor(score: number | undefined): string {
    if (score === undefined) return 'warn';
    if (score >= 85) return 'success';
    if (score >= 70) return 'accent';
    return 'warn';
  }
}