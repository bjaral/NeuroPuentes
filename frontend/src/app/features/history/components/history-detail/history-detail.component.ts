import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { HistoryDetailService } from '../../services/history-detail.service';
import { Entrevista } from '../../../../shared/models/entrevista.model';
import { Dialogo } from '../../../../shared/models/dialogo.model';
import { 
  Contexto,
  getEdadFromPromptSeed,
  getTagsFromPromptSeed,
  scopeToTags
} from '../../../../shared/models/contexto.model';

/**
 * Componente para visualizar el detalle completo de una entrevista.
 * Muestra: fecha, duración, contexto (título, descripción, tags) y transcripción desplegable.
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
    private router: Router,
    private historyDetailService: HistoryDetailService
  ) {}

  // Heurística 1: Visibilidad del estado
  cargandoDatos = signal(true);
  
  // NUEVO: Control de expansión de transcripción
  // Heurística 8: Diseño minimalista - La transcripción inicia colapsada
  transcripcionExpandida = false;

  // Datos de la entrevista
  interviewId = '';
  entrevista: Entrevista | null = null;
  contexto: Contexto | null = null;
  fechaEntrevista = '';
  duracionEntrevista = '';

  // Transcripción
  transcript: Dialogo[] = [];

  ngOnInit(): void {
    this.interviewId = this.route.snapshot.paramMap.get('id') || '';
    if (this.interviewId) {
      this.cargarDatos();
    }
  }

  /**
   * Carga los datos de la entrevista desde el backend
   * Heurística 1: Visibilidad del estado del sistema
   */
  private cargarDatos(): void {
    this.cargandoDatos.set(true);

    const entrevistaId = parseInt(this.interviewId);
    
    this.historyDetailService.getDetalleCompleto(entrevistaId).subscribe({
      next: ({ entrevista, dialogos, contexto }) => {
        this.entrevista = entrevista;
        this.contexto = contexto;
        this.transcript = dialogos;

        // Formatear fecha y duración
        if (entrevista?.fecha_creacion) {
          this.fechaEntrevista = this.formatearFecha(entrevista.fecha_creacion);
        }

        if (entrevista?.duracion_min) {
          this.duracionEntrevista = `${entrevista.duracion_min} min`;
        }

        this.cargandoDatos.set(false);
      },
      error: (error) => {
        console.error('Error al cargar datos de la entrevista:', error);
        this.cargandoDatos.set(false);
      }
    });
  }

  /**
   * NUEVO: Expande o colapsa la transcripción
   * Heurística 3: Control y libertad del usuario
   * Heurística 8: Diseño minimalista - Reduce saturación visual
   */
  toggleTranscripcion(): void {
    this.transcripcionExpandida = !this.transcripcionExpandida;
  }

  /**
   * Formatea una fecha ISO a formato legible
   */
  private formatearFecha(fechaISO: string): string {
    const fecha = new Date(fechaISO);
    const opciones: Intl.DateTimeFormatOptions = { 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    };
    return fecha.toLocaleDateString('es-ES', opciones);
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
    if (this.contexto) {
      // Guardar el contexto en sessionStorage para usarlo en la simulación
      sessionStorage.setItem('selectedContexto', JSON.stringify(this.contexto));
      this.router.navigate(['/simulation']);
    } else {
      // Si no hay contexto, ir a scenarios para que seleccione uno
      this.router.navigate(['/scenarios']);
    }
  }

  /**
   * Descarga el reporte en formato HTML
   * Heurística 7: Flexibilidad y eficiencia
   */
  descargarReporte(): void {
    if (!this.entrevista || !this.contexto) {
      console.error('No hay datos para descargar');
      return;
    }

    // Crear contenido del reporte
    const contenido = this.generarContenidoReporte();
    
    // Crear un blob con el contenido HTML
    const blob = new Blob([contenido], { type: 'text/html;charset=utf-8' });
    
    // Crear un enlace de descarga
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `Entrevista_${this.entrevista._id}_${new Date().toISOString().split('T')[0]}.html`;
    
    // Trigger de descarga
    document.body.appendChild(link);
    link.click();
    
    // Limpieza
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
    
    console.log('Reporte descargado exitosamente');
  }

  /**
   * Genera el contenido HTML del reporte para descarga
   */
  private generarContenidoReporte(): string {
    const tags = this.getAllTags().join(', ');
    const edad = this.getEdadFromContexto();
    
    let transcripcionHTML = '';
    this.transcript.forEach((dialogo) => {
      const emisor = dialogo.sender?.toLowerCase() === 'user' ? 'Estudiante' : 'Paciente (IA)';
      const clase = dialogo.sender?.toLowerCase() === 'user' ? 'user' : 'ai';
      transcripcionHTML += `
        <div class="message ${clase}">
          <strong>${emisor}:</strong>
          <p>${dialogo.texto}</p>
        </div>
      `;
    });

    return `
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Reporte de Entrevista - ${this.entrevista?._id}</title>
  <style>
    * {
      margin: 0;
      padding: 0;
      box-sizing: border-box;
    }
    body {
      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
      line-height: 1.6;
      color: #333;
      max-width: 900px;
      margin: 0 auto;
      padding: 2rem;
      background: #f5f5f5;
    }
    .header {
      background: linear-gradient(135deg, #1565C0, #0D47A1);
      color: white;
      padding: 2rem;
      border-radius: 12px;
      margin-bottom: 2rem;
    }
    h1 {
      font-size: 2rem;
      margin-bottom: 0.5rem;
    }
    .meta {
      font-size: 0.9rem;
      opacity: 0.9;
    }
    .section {
      background: white;
      padding: 1.5rem;
      border-radius: 12px;
      margin-bottom: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }
    h2 {
      color: #1565C0;
      margin-bottom: 1rem;
      padding-bottom: 0.5rem;
      border-bottom: 2px solid #1565C0;
    }
    .info-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1rem;
      margin-bottom: 1rem;
    }
    .info-item {
      padding: 0.75rem;
      background: #f5f5f5;
      border-radius: 8px;
    }
    .info-label {
      font-weight: 600;
      color: #1565C0;
      font-size: 0.85rem;
      text-transform: uppercase;
    }
    .info-value {
      font-size: 1.1rem;
      margin-top: 0.25rem;
    }
    .tags {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
      margin-top: 1rem;
    }
    .tag {
      background: rgba(21, 101, 192, 0.1);
      color: #1565C0;
      padding: 0.35rem 0.75rem;
      border-radius: 12px;
      font-size: 0.85rem;
      font-weight: 600;
    }
    .messages {
      display: flex;
      flex-direction: column;
      gap: 1rem;
      max-height: 600px;
      overflow-y: auto;
    }
    .message {
      padding: 1rem;
      border-radius: 12px;
      max-width: 85%;
    }
    .message.user {
      align-self: flex-end;
      background: linear-gradient(135deg, #1565C0, #0D47A1);
      color: white;
      margin-left: auto;
    }
    .message.ai {
      align-self: flex-start;
      background: #f5f5f5;
      color: #333;
    }
    .message strong {
      display: block;
      font-size: 0.75rem;
      text-transform: uppercase;
      margin-bottom: 0.5rem;
      opacity: 0.8;
    }
    .message p {
      margin: 0;
    }
    .footer {
      text-align: center;
      padding: 2rem;
      color: #666;
      font-size: 0.9rem;
    }
    @media print {
      body {
        background: white;
      }
      .section {
        box-shadow: none;
        border: 1px solid #ddd;
      }
    }
  </style>
</head>
<body>
  <div class="header">
    <h1>REPORTE DE ENTREVISTA</h1>
    <div class="meta">
      ID: ${this.entrevista?._id} | 
      Generado: ${new Date().toLocaleDateString('es-ES', { 
        year: 'numeric', 
        month: 'long', 
        day: 'numeric', 
        hour: '2-digit', 
        minute: '2-digit' 
      })}
    </div>
  </div>
  
  <div class="section">
    <h2>INFORMACIÓN GENERAL</h2>
    <div class="info-grid">
      <div class="info-item">
        <div class="info-label">Fecha</div>
        <div class="info-value">${this.fechaEntrevista}</div>
      </div>
      <div class="info-item">
        <div class="info-label">Duración</div>
        <div class="info-value">${this.duracionEntrevista}</div>
      </div>
      <div class="info-item">
        <div class="info-label">Número de Turnos</div>
        <div class="info-value">${this.transcript.length}</div>
      </div>
    </div>
  </div>
  
  <div class="section">
    <h2>CONTEXTO DE LA ENTREVISTA</h2>
    <h3 style="color: #0D47A1; margin-bottom: 0.5rem;">${this.contexto?.nombre}</h3>
    <p style="margin-bottom: 1rem;">${this.contexto?.descripcion}</p>
    ${edad !== 'N/A' ? `<p><strong>Edad del paciente:</strong> ${edad}</p>` : ''}
    ${tags ? `
      <div>
        <strong>Características:</strong>
        <div class="tags">
          ${this.getAllTags().map(tag => `<span class="tag">${tag}</span>`).join('')}
        </div>
      </div>
    ` : ''}
  </div>
  
  <div class="section">
    <h2>TRANSCRIPCIÓN COMPLETA</h2>
    <div class="messages">
      ${transcripcionHTML || '<p style="text-align: center; color: #666;">No hay transcripción disponible.</p>'}
    </div>
  </div>
  
  <div class="footer">
    <p><strong>NeuroPuentes</strong> - Sistema de Práctica de Entrevistas</p>
    <p style="margin-top: 0.5rem;">Este reporte es confidencial y debe ser tratado según las políticas de privacidad.</p>
  </div>
</body>
</html>
    `;
  }

  /**
   * Obtiene la edad del contexto usando el helper
   */
  getEdadFromContexto(): string {
    return this.contexto ? getEdadFromPromptSeed(this.contexto) : 'N/A';
  }

  /**
   * Obtiene todos los tags combinados (scope + promptSeed)
   */
  getAllTags(): string[] {
    if (!this.contexto) return [];
    
    const scopeTags = scopeToTags(this.contexto.scope);
    const promptTags = getTagsFromPromptSeed(this.contexto);
    return [...scopeTags, ...promptTags];
  }
}