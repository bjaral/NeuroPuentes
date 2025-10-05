import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { ScenariosService } from '../../services/scenarios.service';
import { Contexto } from '../../../../shared/models/contexto.model';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';

/**
 * Componente de configuración de contextos
 * Permite seleccionar casos de práctica para entrevistas
 * Aplica heurísticas de Nielsen para mejor usabilidad
 */
@Component({
  selector: 'app-scenario-config',
  standalone: true,
  imports: [
    CommonModule,
    ...MATERIAL_IMPORTS,
    RouterModule
  ],
  templateUrl: './scenario-config.component.html',
  styleUrls: ['./scenario-config.component.scss']
})
export class ScenarioConfigComponent implements OnInit {
  private contextosService = inject(ScenariosService);
  private router = inject(Router);

  // Heurística 1: Visibilidad del estado
  cargandoDatos = signal(true);
  contextos: Contexto[] = [];
  selectedContexto = signal<Contexto | null>(null);
  isRandomizing = signal(false);
  pulsingContextoId: number | null = null;

  // Heurística 6: Búsqueda para reconocer antes que recordar
  terminoBusqueda = signal('');
  contextosFiltrados = signal<Contexto[]>([]);

  ngOnInit(): void {
    this.loadContextos();
  }

  /**
   * Carga los contextos disponibles desde el servicio
   * Heurística 1: Feedback del estado de carga
   */
  private loadContextos(): void {
    this.cargandoDatos.set(true);

    this.contextosService.getContextos().subscribe({
      next: (contextos) => {
        this.contextos = contextos;
        this.contextosFiltrados.set(contextos);
        this.cargandoDatos.set(false);

        // Heurística 6: Preseleccionar el primero para facilitar inicio rápido
        if (contextos.length > 0) {
          this.selectedContexto.set(contextos[0]);
        }
      },
      error: (err) => {
        console.error('Error al cargar contextos:', err);
        this.cargandoDatos.set(false);
      }
    });
  }

  /**
   * Selecciona un contexto específico
   * Heurística 3: Control del usuario
   */
  selectContexto(contexto: Contexto): void {
    if (this.isRandomizing()) return;

    this.selectedContexto.set(contexto);
    this.addSelectionEffect(contexto._id!);
  }

  /**
   * Selecciona un contexto aleatorio
   * Heurística 7: Flexibilidad y eficiencia
   */
  selectRandom(): void {
    if (this.isRandomizing() || this.contextosFiltrados().length === 0) return;

    this.isRandomizing.set(true);

    this.contextosService.getContextoAleatorio().subscribe({
      next: (contexto) => {
        // Efecto visual de selección aleatoria
        let iterations = 0;
        const maxIterations = 8;
        const interval = setInterval(() => {
          const randomIndex = Math.floor(Math.random() * this.contextosFiltrados().length);
          const randomContexto = this.contextosFiltrados()[randomIndex];
          this.addSelectionEffect(randomContexto._id!);
          iterations++;

          if (iterations >= maxIterations) {
            clearInterval(interval);
            this.selectedContexto.set(contexto);
            this.isRandomizing.set(false);
          }
        }, 150);
      },
      error: (err) => {
        console.error('Error al seleccionar contexto aleatorio:', err);
        this.isRandomizing.set(false);
      }
    });
  }

  /**
   * Maneja la búsqueda de contextos
   * Heurística 6: Reconocer antes que recordar
   */
  onBusquedaChange(termino: string): void {
    this.terminoBusqueda.set(termino);
    
    this.contextosService.buscarContextos(termino).subscribe({
      next: (resultados) => {
        this.contextosFiltrados.set(resultados);

        // Si hay selección previa y ya no está en los filtrados, limpiar
        const currentSelection = this.selectedContexto();
        if (currentSelection && !resultados.find(c => c._id === currentSelection._id)) {
          this.selectedContexto.set(null);
        }
      }
    });
  }

  /**
   * Limpia la búsqueda
   * Heurística 3: Control y libertad
   */
  limpiarBusqueda(): void {
    this.terminoBusqueda.set('');
    this.contextosFiltrados.set(this.contextos);
  }

  /**
   * Inicia la simulación con el contexto seleccionado
   * Heurística 1: Feedback claro de acción
   */
  startSimulation(): void {
    const contexto = this.selectedContexto();
    if (!contexto) return;

    // Guardar el contexto seleccionado para usarlo en la simulación
    sessionStorage.setItem('selectedContexto', JSON.stringify(contexto));

    // Navegar a la simulación
    this.router.navigate(['/simulation']);
  }

  /**
   * Navega de vuelta al dashboard
   * Heurística 3: Control y libertad
   */
  volverDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  /**
   * Efecto visual de selección
   * Heurística 1: Feedback visual inmediato
   */
  private addSelectionEffect(contextoId: number): void {
    this.pulsingContextoId = contextoId;

    setTimeout(() => {
      this.pulsingContextoId = null;
    }, 600);
  }

  /**
   * Obtiene estadísticas de los contextos
   * Heurística 10: Información de ayuda
   */
  getStats(): string {
    const stats = this.contextosService.getEstadisticas();
    return `${stats.total} casos disponibles`;
  }

  /**
   * Verifica si hay resultados de búsqueda
   * Heurística 9: Feedback de estado vacío
   */
  hasResults(): boolean {
    return this.contextosFiltrados().length > 0;
  }

  /**
   * Obtiene mensaje para estado vacío
   * Heurística 9: Mensajes claros y constructivos
   */
  getEmptyMessage(): string {
    if (this.terminoBusqueda()) {
      return `No se encontraron casos que coincidan con "${this.terminoBusqueda()}"`;
    }
    return 'No hay casos disponibles';
  }

  /**
   * Extrae la edad del prompt_seed o nombre si está disponible
   */
  getEdadFromContexto(contexto: Contexto): string {
    const nombre = contexto.nombre || '';
    const match = nombre.match(/(\d+)\s*año/i);
    return match ? `${match[1]} años` : 'N/A';
  }

  /**
   * Obtiene tags del scope
   */
  getTagsFromScope(scope: string | undefined): string[] {
    if (!scope) return [];
    
    const scopeTags: { [key: string]: string[] } = {
      'evaluacion_inicial': ['Evaluación', 'Primera consulta'],
      'seguimiento': ['Seguimiento', 'Apoyo continuo'],
      'intervencion': ['Intervención', 'Apoyo intensivo'],
      'crisis': ['Crisis', 'Urgente'],
      'evaluacion_tardia': ['Diagnóstico tardío', 'Evaluación']
    };

    return scopeTags[scope] || [scope];
  }
}