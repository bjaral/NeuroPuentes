import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { Entrevista } from '../../../../shared/models/entrevista.model';
import { EvalEntrevista } from '../../../../shared/models/eval.model';
import { FormsModule } from '@angular/forms';

/**
 * Interface combinada para mostrar entrevista con su evaluación
 */
interface EntrevistaConEval {
  entrevista: Entrevista;
  evaluacion: EvalEntrevista;
}

/**
 * Componente de historial de entrevistas
 * Aplica heurísticas de Nielsen enfocándose en búsqueda y navegación clara
 */
@Component({
  selector: 'app-history',
  imports: [...MATERIAL_IMPORTS, CommonModule, FormsModule],
  templateUrl: './history.component.html',
  styleUrl: './history.component.scss'
})
export class HistoryComponent implements OnInit {

  constructor(private router: Router) {}

  // Heurística 1: Visibilidad del estado
  cargandoDatos = signal(true);
  
  // Heurística 6: Búsqueda para reconocer antes que recordar
  terminoBusqueda = signal('');

  // Datos de entrevistas
  private entrevistasOriginales = signal<EntrevistaConEval[]>([]);

  // Heurística 7: Eficiencia - Computed para filtrado reactivo
  entrevistasFiltradas = computed(() => {
    const termino = this.terminoBusqueda().toLowerCase();
    if (!termino) {
      return this.entrevistasOriginales();
    }

    return this.entrevistasOriginales().filter(item => {
      const contexto = item.entrevista.contexto_snapshot?.toLowerCase() || '';
      const titulo = item.entrevista.titulo?.toLowerCase() || '';
      const fecha = item.entrevista.fecha_creacion?.toLowerCase() || '';
      
      return contexto.includes(termino) || 
             titulo.includes(termino) || 
             fecha.includes(termino);
    });
  });

  ngOnInit(): void {
    this.cargarEntrevistas();
  }

  /**
   * Carga las entrevistas del usuario
   * Heurística 1: Visibilidad del estado
   */
  private cargarEntrevistas(): void {
    // Simulación de datos - en producción vendría del servicio
    setTimeout(() => {
      const mockData: EntrevistaConEval[] = [
        {
          entrevista: {
            _id: 1,
            usuario_id: 1,
            contexto_id: 1,
            titulo: 'Entrevista con madre de niño de 8 años',
            descripcion: 'Evaluación inicial',
            duracion_min: 32,
            numero_turnos: 12,
            fecha_creacion: '2024-03-15',
            fecha_cierre: '2024-03-15',
            contexto_snapshot: 'Madre preocupada por el comportamiento de su hijo en la escuela. Menciona dificultades de socialización y posible diagnóstico de autismo.'
          },
          evaluacion: {
            _id: 1,
            entrevista_id: 1,
            score_final: 86,
            comentario_general: 'Buen manejo de empatía'
          }
        },
        {
          entrevista: {
            _id: 2,
            usuario_id: 1,
            contexto_id: 2,
            titulo: 'Entrevista inicial con padre',
            descripcion: 'Primera sesión',
            duracion_min: 28,
            numero_turnos: 10,
            fecha_creacion: '2024-03-10',
            fecha_cierre: '2024-03-10',
            contexto_snapshot: 'Padre busca estrategias para ayudar a su hija de 6 años con dificultades de atención en clase.'
          },
          evaluacion: {
            _id: 2,
            entrevista_id: 2,
            score_final: 78,
            comentario_general: 'Mejorar preguntas abiertas'
          }
        },
        {
          entrevista: {
            _id: 3,
            usuario_id: 1,
            contexto_id: 3,
            titulo: 'Seguimiento caso TEA',
            descripcion: 'Segunda sesión',
            duracion_min: 45,
            numero_turnos: 18,
            fecha_creacion: '2024-03-05',
            fecha_cierre: '2024-03-05',
            contexto_snapshot: 'Seguimiento del caso anterior. La madre reporta mejoras en la comunicación del niño después de implementar las estrategias sugeridas.'
          },
          evaluacion: {
            _id: 3,
            entrevista_id: 3,
            score_final: 92,
            comentario_general: 'Excelente manejo del seguimiento'
          }
        }
      ];

      this.entrevistasOriginales.set(mockData);
      this.cargandoDatos.set(false);
    }, 600);
  }

  /**
   * Maneja el cambio en el input de búsqueda
   * Heurística 7: Eficiencia de uso
   */
  onBusquedaChange(termino: string): void {
    this.terminoBusqueda.set(termino);
  }

  /**
   * Limpia la búsqueda
   * Heurística 3: Control y libertad
   */
  limpiarBusqueda(): void {
    this.terminoBusqueda.set('');
  }

  /**
   * Navega al detalle de la entrevista
   * Heurística 3: Control del usuario
   */
  verDetalle(entrevistaId: number): void {
    this.router.navigate(['/history', entrevistaId]);
  }

  /**
   * Formatea la fecha para mostrar
   * Heurística 2: Correspondencia con el mundo real
   */
  formatearFecha(fecha: string | undefined): string {
    if (!fecha) return 'Sin fecha';
    const date = new Date(fecha);
    return date.toLocaleDateString('es-CL', { 
      day: '2-digit', 
      month: '2-digit', 
      year: '2-digit' 
    });
  }

  /**
   * Trunca el texto del contexto para preview
   * Heurística 8: Diseño minimalista
   */
  truncarContexto(contexto: string | undefined, maxLength: number = 120): string {
    if (!contexto) return 'Sin descripción disponible';
    if (contexto.length <= maxLength) return contexto;
    return contexto.substring(0, maxLength) + '...';
  }

  /**
   * Obtiene la clase CSS según el puntaje
   * Heurística 2: Correspondencia visual con significado
   */
  getScoreClass(score: number | undefined): string {
    if (!score) return '';
    if (score >= 85) return 'score-excellent';
    if (score >= 70) return 'score-good';
    return 'score-needs-improvement';
  }
}