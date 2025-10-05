// angular core
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';

interface Entrevista {
  fecha: string;
  puntaje: number;
  estado: 'Completada' | 'Pendiente' | 'En Progreso';
  id?: number;
}

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonModule,
    RouterModule, ...MATERIAL_IMPORTS],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  entrevistasRealizadas = 12;
  puntajePromedio = "85%";
  ultimoFeedback = "Buen Inicio";
  errorCargando = false;
  cargandoDatos = true; // Nuevo: estado de carga

  ngOnInit() {
    this.cargarEntrevistas();
  }

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
    // Mejorado: feedback más específico
    console.log('Redirigiendo a nueva entrevista...');
  }

  simularEntrevista() {
    // Mejorado: feedback más específico
    console.log('Iniciando simulación...');
  }

  tooltipEstado(estado: string): string {
    switch (estado.toLowerCase()) {
      case 'pendiente':
        return 'Entrevista creada pero aún no iniciada';
      case 'completada':
        return 'Entrevista finalizada correctamente';
      case 'en progreso':
        return 'Entrevista en curso';
      default:
        return 'Estado desconocido';
    }
  }

  // Manejo de errores y estados
  async cargarEntrevistas() {
    this.cargandoDatos = true;
    this.errorCargando = false;
    
    try {
      // Simular carga asíncrona
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      const exito = true; // Simular éxito/fallo
      
      if (!exito) {
        throw new Error('Error al cargar datos del servidor');
      }

      // Validar datos antes de asignar
      if (this.entrevistas && Array.isArray(this.entrevistas)) {
        this.errorCargando = false;
      }

    } catch (error) {
      console.error('Error cargando entrevistas:', error);
      this.errorCargando = true;
      this.entrevistas = [];
    } finally {
      this.cargandoDatos = false;
    }
  }

  // Función para reintentar carga
  reintentarCarga() {
    this.cargarEntrevistas();
  }
}