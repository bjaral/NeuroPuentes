import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin, of } from 'rxjs';
import { map, catchError, switchMap } from 'rxjs/operators';
import { Entrevista } from '../../../shared/models/entrevista.model';
import { Dialogo } from '../../../shared/models/dialogo.model';
import { Contexto } from '../../../shared/models/contexto.model';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class HistoryDetailService {
  private apiUrl = environment.apiUrl || 'http://localhost:5299';
  private entrevistasUrl = `${this.apiUrl}/api/Entrevistas`;
  private dialogosUrl = `${this.apiUrl}/api/Dialogos`;
  private contextosUrl = `${this.apiUrl}/api/Contextos`;

  constructor(private http: HttpClient) { }

  /**
   * Obtiene una entrevista por ID
   */
  getEntrevistaById(id: number): Observable<Entrevista | null> {
    return this.http.get<any>(`${this.entrevistasUrl}/${id}`).pipe(
      map(data => this.mapEntrevistaFromBackend(data)),
      catchError(error => {
        console.error(`Error al obtener entrevista ${id}:`, error);
        return of(null);
      })
    );
  }

  /**
   * Obtiene los diálogos de una entrevista
   */
  getDialogosByEntrevistaId(entrevistaId: number): Observable<Dialogo[]> {
    return this.http.get<any[]>(`${this.dialogosUrl}/entrevista/${entrevistaId}`).pipe(
      map(data => this.mapDialogosFromBackend(data)),
      catchError(error => {
        console.error(`Error al obtener diálogos de entrevista ${entrevistaId}:`, error);
        return of([]);
      })
    );
  }

  /**
   * Obtiene el contexto asociado a una entrevista
   */
  getContextoByEntrevistaId(entrevistaId: number): Observable<Contexto | null> {
    return this.http.get<any>(`${this.contextosUrl}/entrevista/${entrevistaId}`).pipe(
      map(data => this.mapContextoFromBackend(data)),
      catchError(error => {
        console.error(`Error al obtener contexto de entrevista ${entrevistaId}:`, error);
        return of(null);
      })
    );
  }

  /**
   * Obtiene los detalles completos de una entrevista (entrevista + diálogos + contexto)
   */
  getDetalleCompleto(entrevistaId: number): Observable<{
    entrevista: Entrevista | null;
    dialogos: Dialogo[];
    contexto: Contexto | null;
  }> {
    return forkJoin({
      entrevista: this.getEntrevistaById(entrevistaId),
      dialogos: this.getDialogosByEntrevistaId(entrevistaId),
      contexto: this.getContextoByEntrevistaId(entrevistaId)
    });
  }

  /**
   * Obtiene todas las entrevistas de un usuario
   */
  getEntrevistasByUsuarioId(usuarioId: number): Observable<Entrevista[]> {
    return this.http.get<any[]>(`${this.entrevistasUrl}/usuario/${usuarioId}`).pipe(
      map(data => this.mapEntrevistasFromBackend(data)),
      catchError(error => {
        console.error(`Error al obtener entrevistas del usuario ${usuarioId}:`, error);
        return of([]);
      })
    );
  }

  // ============================================
  // MÉTODOS DE MAPEO (Backend camelCase <-> Frontend snake_case)
  // ============================================

  private mapEntrevistaFromBackend(data: any): Entrevista {
    return {
      _id: data.id,
      usuario_id: data.usuarioId,
      contexto_id: data.contextoId,
      titulo: data.titulo,
      descripcion: data.descripcion,
      duracion_min: data.duracionMin,
      numero_turnos: data.numeroTurnos,
      fecha_creacion: data.fechaCreacion,
      fecha_cierre: data.fechaCierre,
      contexto_snapshot: data.contextoSnapshot
    };
  }

  private mapEntrevistasFromBackend(data: any[]): Entrevista[] {
    return data.map(item => this.mapEntrevistaFromBackend(item));
  }

  private mapDialogoFromBackend(data: any): Dialogo {
    return {
      _id: data.id,
      entrevista_id: data.entrevistaId,
      turno: data.turno,
      sender: data.sender,
      texto: data.texto,
      texto_procesado: data.textoProcesado,
      timestamp: data.timestamp,
      audio_url: data.audioUrl
    };
  }

  private mapDialogosFromBackend(data: any[]): Dialogo[] {
    return data.map(item => this.mapDialogoFromBackend(item)).sort((a, b) => (a.turno || 0) - (b.turno || 0));
  }

  private mapContextoFromBackend(data: any): Contexto {
    return {
      id: data.id,
      nombre: data.nombre,
      descripcion: data.descripcion,
      scope: data.scope,
      creadoPor: data.creadoPor,
      origen: data.origen,
      promptSeed: data.promptSeed,
      vigencia: data.vigencia,
      fechaCreacion: data.fechaCreacion
    };
  }
}