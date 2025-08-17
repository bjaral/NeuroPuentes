// angular core
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

// angular material
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatGridListModule } from '@angular/material/grid-list';



interface Entrevista {
  fecha: string;
  puntaje: number;
  estado: 'Completada' | 'Pendiente' | 'En Progreso';
}

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonModule, 
    RouterModule,
    MatCardModule, 
    MatButtonModule, 
    MatIconModule, 
    MatGridListModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
    entrevistasRealizadas = 12;
    puntajePromedio = "85%";
    ultimoFeedback = "Buen Inicio";
    
  entrevistas: Entrevista[] = [
    { fecha: '2025-08-01', puntaje: 85, estado: 'Completada' },
    { fecha: '2025-08-10', puntaje: 90, estado: 'Completada' },
    { fecha: '2025-08-15', puntaje: 70, estado: 'Pendiente' },
  ];

  progresoGeneral = 76;
  feedbackReciente = '¡Buen progreso en tus últimas entrevistas!';
  tips = [
    'Practica respuestas más concisas.',
    'Trabaja en tu lenguaje corporal.',
    'Revisa conceptos técnicos clave.'
  ];

  nuevaEntrevista() {
    alert('Funcionalidad para crear nueva entrevista 🚀');
  }

  simularEntrevista() {
    alert('Funcionalidad para simular entrevista 🎤');
  }
}

