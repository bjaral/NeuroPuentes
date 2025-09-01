import { Component, Input } from '@angular/core';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { CommonModule } from '@angular/common';

interface TranscriptLine {
  sender: 'user' | 'ai';
  text: string;
}

interface EvaluationCategory {
  category: string;
  score: number;
}

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

  transcript: TranscriptLine[] = [
    { sender: 'user', text: 'Buenos días, ¿cómo se encuentra hoy?' },
    {
      sender: 'ai',
      text: 'Buenos días, me siento un poco preocupada por mi hijo.',
    },
    { sender: 'user', text: 'Entiendo, ¿qué es lo que más le preocupa?' },
    {
      sender: 'ai',
      text: 'Su desempeño en la escuela y cómo se relaciona con otros niños.',
    },
  ];

  evaluation: EvaluationCategory[] = [
    { category: 'Inicio', score: 85 },
    { category: 'Preguntas', score: 78 },
    { category: 'Empatía', score: 92 },
    { category: 'Cierre', score: 88 },
  ];

  finalScore = 86;

}
