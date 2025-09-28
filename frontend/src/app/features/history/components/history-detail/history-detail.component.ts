import { Component, Input } from '@angular/core';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { CommonModule } from '@angular/common';
import { FeedbackEntrevista } from '../../../../shared/models/feedback.model';
import { Dialogo } from '../../../../shared/models/dialogo.model';
import { EvalCategoria, EvalEntrevista } from '../../../../shared/models/eval.model';

@Component({
  selector: 'app-history-detail',
  imports: [...MATERIAL_IMPORTS, CommonModule],
  templateUrl: './history-detail.component.html',
  styleUrl: './history-detail.component.scss'
})
export class HistoryDetailComponent {

  interviewTitle = 'Entrevista con madre de niño de 8 años';
  interviewDescription =
    'Simulación de entrevista inicial para evaluar comunicación empática y preguntas abiertas.';
  tags = ['Empatía', 'Autismo', 'Entrevista inicial'];

  tips: string[] = [
    'Mantén contacto visual y valida emociones explícitamente.',
    'Usa preguntas abiertas para explorar más detalles.',
    'Evita interrupciones largas, permite que el otro se exprese.',
  ];

  transcript: Dialogo[] = [
    { sender: 'user', texto: 'Buenos días, ¿cómo se encuentra hoy?' },
    {
      sender: 'ai',
      texto: 'Buenos días, me siento un poco preocupada por mi hijo.',
    },
    { sender: 'user', texto: 'Entiendo, ¿qué es lo que más le preocupa?' },
    {
      sender: 'ai',
      texto: 'Su desempeño en la escuela y cómo se relaciona con otros niños.',
    },
  ];

  // Usando las interfaces correctas
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

  // Nuevo: Feedback separado por tipo
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
}