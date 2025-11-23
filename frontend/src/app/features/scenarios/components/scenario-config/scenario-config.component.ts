import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { ScenariosService } from '../../services/scenarios.service';
import { TokenService } from '../../../../core/services/token.service';
import { Entrevista } from '../../../../shared/models/entrevista.model';
import {
  Contexto,
  scopeToTags,
  getEdadFromPromptSeed,
  getTagsFromPromptSeed
} from '../../../../shared/models/contexto.model';
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

  // Estado de estadísticas
  estadisticas = signal<{ total: number; porScope: { [key: string]: number } }>({
    total: 0,
    porScope: {}
  });

  ngOnInit(): void {
    this.loadContextos();
    this.loadEstadisticas();
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
   * Carga las estadísticas de contextos
   */
  private loadEstadisticas(): void {
    this.contextosService.getEstadisticas().subscribe({
      next: (stats) => {
        this.estadisticas.set(stats);
      },
      error: (err) => {
        console.error('Error al cargar estadísticas:', err);
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
    this.addSelectionEffect(contexto.id!);
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
          this.addSelectionEffect(randomContexto.id!);
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
        if (currentSelection && !resultados.find(c => c.id === currentSelection.id)) {
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

    // 1. Guardar el contexto seleccionado para usarlo en la simulación
    // Se usa sessionStorage
    sessionStorage.setItem('selectedContexto', JSON.stringify(contexto));

    // 2. Navegar a la simulación
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
    const stats = this.estadisticas();
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
   * Extrae la edad del promptSeed o nombre usando el helper
   */
  getEdadFromContexto(contexto: Contexto): string {
    return getEdadFromPromptSeed(contexto);
  }

  /**
   * Obtiene tags del scope usando el helper
   */
  getTagsFromScope(scope: string | undefined): string[] {
    return scopeToTags(scope);
  }

  /**
   * Obtiene tags dinámicos del promptSeed (todos excepto edad)
   */
  getTagsFromPromptSeed(contexto: Contexto): string[] {
    return getTagsFromPromptSeed(contexto);
  }

  /**
   * Obtiene todos los tags combinados (scope + promptSeed)
   */
  getAllTags(contexto: Contexto): string[] {
    const scopeTags = this.getTagsFromScope(contexto.scope);
    const promptTags = this.getTagsFromPromptSeed(contexto);
    return [...scopeTags, ...promptTags];
  }

  /**
   * NUEVO: Formatea el promptSeed para mostrarlo de forma legible
   * Convierte el JSON en texto amigable
   * Heurística 2: Correspondencia con el mundo real
   */
  getPromptSeedFormatted(contexto: Contexto): string {
    if (!contexto.promptSeed) return '';

    try {
      // Intentar parsear como JSON
      const parsed = JSON.parse(contexto.promptSeed);
      
      // Crear una descripción legible desde el JSON
      const parts: string[] = [];

      // Edad
      if (parsed.edad) {
        parts.push(`Paciente de ${parsed.edad} años`);
      }

      // Severidad
      if (parsed.severidad) {
        const severidadMap: { [key: string]: string } = {
          'leve': 'Caso de complejidad leve',
          'moderado': 'Caso de complejidad moderada',
          'moderada': 'Caso de complejidad moderada',
          'severo': 'Caso de complejidad alta',
          'severa': 'Caso de complejidad alta'
        };
        const severidad = severidadMap[parsed.severidad.toLowerCase()] || `Severidad: ${parsed.severidad}`;
        parts.push(severidad);
      }

      // Comorbilidad
      if (parsed.comorbilidad) {
        const comorbilidades = Array.isArray(parsed.comorbilidad) 
          ? parsed.comorbilidad.join(', ') 
          : parsed.comorbilidad;
        parts.push(`Con comorbilidad: ${comorbilidades}`);
      }

      // Foco
      if (parsed.foco) {
        const focos = Array.isArray(parsed.foco) 
          ? parsed.foco.join(', ') 
          : parsed.foco;
        parts.push(`Foco evaluativo: ${focos}`);
      }

      // Tipo
      if (parsed.tipo) {
        const tipoMap: { [key: string]: string } = {
          'inicial': 'Evaluación inicial',
          'seguimiento': 'Evaluación de seguimiento',
          'control': 'Control de evolución',
          'derivacion': 'Evaluación para derivación',
          'derivación': 'Evaluación para derivación'
        };
        const tipo = tipoMap[parsed.tipo.toLowerCase()] || `Tipo: ${parsed.tipo}`;
        parts.push(tipo);
      }

      // Contexto
      if (parsed.contexto) {
        const contextos = Array.isArray(parsed.contexto) 
          ? parsed.contexto.join(', ') 
          : parsed.contexto;
        parts.push(`Contexto: ${contextos}`);
      }

      // Si no hay partes formateadas, devolver descripción genérica
      if (parts.length === 0) {
        return 'Caso de práctica clínica';
      }

      return parts.join(' • ');

    } catch (e) {
      // Si no es JSON válido, devolver el texto tal cual
      return contexto.promptSeed;
    }
  }
}