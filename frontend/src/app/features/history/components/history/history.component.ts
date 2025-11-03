import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { Entrevista } from '../../../../shared/models/entrevista.model';
import { EvalEntrevista } from '../../../../shared/models/eval.model';
import { FormsModule } from '@angular/forms';
import { HistoryService } from '../../services/history.service';
import { AuthService } from '../../../../core/services/auth.service';
import { TokenService } from '../../../../core/services/token.service';

/**
 * Interface combinada para mostrar entrevista con su evaluación
 */
interface EntrevistaConEval {
  entrevista: Entrevista;
  evaluacion?: EvalEntrevista;
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

  constructor(private router: Router, private authService: AuthService, private tokenService: TokenService, private historyService: HistoryService) { }

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
    this.cargandoDatos.set(true);

    try {
      const usuarioId = this.tokenService.getNameIdentifier();

      if (!usuarioId) {
        console.error('ID de usuario no encontrado');
        this.cargandoDatos.set(false);
        this.entrevistasOriginales.set([]);
        return;
      }

      this.historyService.getEntrevistasUsuario(usuarioId).subscribe({
        next: (data) => {
          if (data && Array.isArray(data)) {
            const entrevistasConEval: EntrevistaConEval[] = data.map(e => ({
              entrevista: {
                _id: e.id,
                usuario_id: e.usuarioId,
                contexto_id: e.contextoId,
                titulo: e.titulo || 'Sin título',
                descripcion: e.descripcion,
                duracion_min: e.duracionMin,
                numero_turnos: e.numeroTurnos,
                fecha_creacion: this.formatearFecha(e.fechaCreacion),
              }
            }));
            this.entrevistasOriginales.set(entrevistasConEval);
          } else {
            console.error('Datos de entrevistas inválidos');
            this.entrevistasOriginales.set([]);
          }
          this.cargandoDatos.set(false);
        },
        error: (err) => {
          console.error('Error cargando entrevistas: ', err);
          this.entrevistasOriginales.set([]);
          this.cargandoDatos.set(false);
        }
      });
    } catch (error) {
      console.error('Error cargando entrevistas:', error);
      this.entrevistasOriginales.set([]);
      this.cargandoDatos.set(false);
    }
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
    this.router.navigate(['/history-detail', entrevistaId]);
  }

  /**
   * Formatea la fecha para mostrar
   * Heurística 2: Correspondencia con el mundo real
   */
  private formatearFecha(fecha: string): string {
    if (!fecha) return 'Sin fecha';
    const date = new Date(fecha);
    return date.toLocaleDateString('es-CL', {
      year: 'numeric', month: '2-digit', day: '2-digit'
    });
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