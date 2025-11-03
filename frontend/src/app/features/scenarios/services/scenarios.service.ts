import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { Contexto, scopeToString } from '../../../shared/models/contexto.model';
import { Caracteristica, CaractsRel } from '../../../shared/models/caracteristica.model';
import { environment } from '../../../../environments/environment';

@Injectable({ 
  providedIn: 'root' 
})
export class ScenariosService {

  private apiUrl = environment.apiUrl || 'http://localhost:5299';
  private contextosUrl = `${this.apiUrl}/api/Contextos`;
  private caracteristicasUrl = `${this.apiUrl}/api/Caracteristicas`;
  private caractsRelUrl = `${this.apiUrl}/api/CaractsRel`;

  constructor(private http: HttpClient) { }

  // ============================================
  // MÉTODOS PARA CONTEXTOS
  // ============================================

  /**
   * Obtiene todos los contextos vigentes desde el backend
   */
  getContextos(): Observable<Contexto[]> {
    return this.http.get<any[]>(`${this.contextosUrl}/Vigentes`).pipe(
      map(data => this.mapContextosFromBackend(data)),
      catchError(error => {
        console.error('Error al obtener contextos:', error);
        return of([]);
      })
    );
  }

  /**
   * Obtiene todos los contextos (incluyendo no vigentes)
   */
  getAllContextos(): Observable<Contexto[]> {
    return this.http.get<any[]>(this.contextosUrl).pipe(
      map(data => this.mapContextosFromBackend(data)),
      catchError(error => {
        console.error('Error al obtener todos los contextos:', error);
        return of([]);
      })
    );
  }

  /**
   * Obtiene un contexto por ID
   */
  getContextoById(id: number): Observable<Contexto | null> {
    return this.http.get<any>(`${this.contextosUrl}/${id}`).pipe(
      map(data => this.mapContextoFromBackend(data)),
      catchError(error => {
        console.error(`Error al obtener contexto ${id}:`, error);
        return of(null);
      })
    );
  }

  /**
   * Obtiene un contexto aleatorio
   */
  getContextoAleatorio(): Observable<Contexto> {
    return this.getContextos().pipe(
      map(contextos => {
        if (contextos.length === 0) {
          throw new Error('No hay contextos disponibles');
        }
        const randomIndex = Math.floor(Math.random() * contextos.length);
        return contextos[randomIndex];
      })
    );
  }

  /**
   * Busca contextos por término
   */
  buscarContextos(termino: string): Observable<Contexto[]> {
    return this.getContextos().pipe(
      map(contextos => {
        if (!termino.trim()) return contextos;
        
        const term = termino.toLowerCase();
        return contextos.filter(c => 
          c.nombre?.toLowerCase().includes(term) ||
          c.descripcion?.toLowerCase().includes(term) ||
          c.scope?.toLowerCase().includes(term)
        );
      })
    );
  }

  /**
   * Filtra contextos por scope
   */
  getContextosPorScope(scope: string): Observable<Contexto[]> {
    return this.getContextos().pipe(
      map(contextos => contextos.filter(c => 
        scopeToString(c.scope).toLowerCase() === scope.toLowerCase()
      ))
    );
  }

  /**
   * Obtiene el contexto asociado a una entrevista
   */
  getContextoPorEntrevistaId(entrevistaId: number): Observable<Contexto | null> {
    return this.http.get<any>(`${this.contextosUrl}/entrevista/${entrevistaId}`).pipe(
      map(data => this.mapContextoFromBackend(data)),
      catchError(error => {
        console.error(`Error al obtener contexto de entrevista ${entrevistaId}:`, error);
        return of(null);
      })
    );
  }

  /**
   * Crea un nuevo contexto
   */
  crearContexto(contexto: Contexto): Observable<number> {
    const dto = this.mapContextoToBackend(contexto);
    return this.http.post<number>(this.contextosUrl, dto).pipe(
      catchError(error => {
        console.error('Error al crear contexto:', error);
        throw error;
      })
    );
  }

  /**
   * Actualiza un contexto existente
   */
  actualizarContexto(id: number, contexto: Contexto): Observable<void> {
    const dto = this.mapContextoToBackend(contexto);
    return this.http.put<void>(`${this.contextosUrl}/${id}`, dto).pipe(
      catchError(error => {
        console.error(`Error al actualizar contexto ${id}:`, error);
        throw error;
      })
    );
  }

  /**
   * Elimina un contexto
   */
  eliminarContexto(id: number): Observable<void> {
    return this.http.delete<void>(`${this.contextosUrl}/${id}`).pipe(
      catchError(error => {
        console.error(`Error al eliminar contexto ${id}:`, error);
        throw error;
      })
    );
  }

  /**
   * Obtiene estadísticas de los contextos
   */
  getEstadisticas(): Observable<{ total: number; porScope: { [key: string]: number } }> {
    return this.getContextos().pipe(
      map(contextos => {
        const porScope: { [key: string]: number } = {};
        
        contextos.forEach(c => {
          const scope = scopeToString(c.scope) || 'sin_categoria';
          porScope[scope] = (porScope[scope] || 0) + 1;
        });

        return {
          total: contextos.length,
          porScope
        };
      })
    );
  }

  // ============================================
  // MÉTODOS PARA CARACTERÍSTICAS
  // ============================================

  /**
   * Obtiene todas las características vigentes
   */
  getCaracteristicas(): Observable<Caracteristica[]> {
    return this.http.get<any[]>(`${this.caracteristicasUrl}/Vigentes`).pipe(
      map(data => this.mapCaracteristicasFromBackend(data)),
      catchError(error => {
        console.error('Error al obtener características:', error);
        return of([]);
      })
    );
  }

  /**
   * Obtiene todas las características (incluyendo no vigentes)
   */
  getAllCaracteristicas(): Observable<Caracteristica[]> {
    return this.http.get<any[]>(this.caracteristicasUrl).pipe(
      map(data => this.mapCaracteristicasFromBackend(data)),
      catchError(error => {
        console.error('Error al obtener todas las características:', error);
        return of([]);
      })
    );
  }

  /**
   * Obtiene una característica por ID
   */
  getCaracteristicaById(id: number): Observable<Caracteristica | null> {
    return this.http.get<any>(`${this.caracteristicasUrl}/${id}`).pipe(
      map(data => this.mapCaracteristicaFromBackend(data)),
      catchError(error => {
        console.error(`Error al obtener característica ${id}:`, error);
        return of(null);
      })
    );
  }

  /**
   * Obtiene características asociadas a un contexto
   */
  getCaracteristicasPorContexto(contextoId: number): Observable<Caracteristica[]> {
    return this.http.get<any[]>(`${this.caracteristicasUrl}/contexto/${contextoId}`).pipe(
      map(data => this.mapCaracteristicasFromBackend(data)),
      catchError(error => {
        console.error(`Error al obtener características del contexto ${contextoId}:`, error);
        return of([]);
      })
    );
  }

  /**
   * Crea una nueva característica
   */
  crearCaracteristica(caracteristica: Caracteristica): Observable<number> {
    const dto = this.mapCaracteristicaToBackend(caracteristica);
    return this.http.post<number>(this.caracteristicasUrl, dto).pipe(
      catchError(error => {
        console.error('Error al crear característica:', error);
        throw error;
      })
    );
  }

  /**
   * Actualiza una característica
   */
  actualizarCaracteristica(id: number, caracteristica: Caracteristica): Observable<void> {
    const dto = this.mapCaracteristicaToBackend(caracteristica);
    return this.http.put<void>(`${this.caracteristicasUrl}/${id}`, dto).pipe(
      catchError(error => {
        console.error(`Error al actualizar característica ${id}:`, error);
        throw error;
      })
    );
  }

  /**
   * Elimina una característica
   */
  eliminarCaracteristica(id: number): Observable<void> {
    return this.http.delete<void>(`${this.caracteristicasUrl}/${id}`).pipe(
      catchError(error => {
        console.error(`Error al eliminar característica ${id}:`, error);
        throw error;
      })
    );
  }

  // ============================================
  // MÉTODOS PARA RELACIONES CARACTERÍSTICAS-CONTEXTO
  // ============================================

  /**
   * Obtiene todas las relaciones
   */
  getCaractsRel(): Observable<CaractsRel[]> {
    return this.http.get<any[]>(this.caractsRelUrl).pipe(
      map(data => this.mapCaractsRelFromBackend(data)),
      catchError(error => {
        console.error('Error al obtener relaciones características-contexto:', error);
        return of([]);
      })
    );
  }

  /**
   * Crea una relación característica-contexto
   */
  crearCaractsRel(relacion: CaractsRel): Observable<void> {
    const dto = this.mapCaractsRelToBackend(relacion);
    return this.http.post<void>(this.caractsRelUrl, dto).pipe(
      catchError(error => {
        console.error('Error al crear relación:', error);
        throw error;
      })
    );
  }

  /**
   * Elimina una relación característica-contexto
   */
  eliminarCaractsRel(caracteristicaId: number, contextoId: number): Observable<void> {
    return this.http.delete<void>(`${this.caractsRelUrl}/${caracteristicaId}/${contextoId}`).pipe(
      catchError(error => {
        console.error('Error al eliminar relación:', error);
        throw error;
      })
    );
  }

  // ============================================
  // MÉTODOS DE MAPEO (Backend camelCase <-> Frontend camelCase)
  // ============================================

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

  private mapContextosFromBackend(data: any[]): Contexto[] {
    return data.map(item => this.mapContextoFromBackend(item));
  }

  private mapContextoToBackend(contexto: Contexto): any {
    return {
      nombre: contexto.nombre,
      descripcion: contexto.descripcion,
      scope: contexto.scope,
      creadoPor: contexto.creadoPor,
      origen: contexto.origen,
      promptSeed: contexto.promptSeed,
      vigencia: contexto.vigencia
    };
  }

  private mapCaracteristicaFromBackend(data: any): Caracteristica {
    return {
      id: data.id,
      nombre: data.nombre,
      descripcion: data.descripcion,
      grupo: data.grupo,
      vigencia: data.vigencia
    };
  }

  private mapCaracteristicasFromBackend(data: any[]): Caracteristica[] {
    return data.map(item => this.mapCaracteristicaFromBackend(item));
  }

  private mapCaracteristicaToBackend(caracteristica: Caracteristica): any {
    return {
      nombre: caracteristica.nombre,
      descripcion: caracteristica.descripcion,
      grupo: caracteristica.grupo,
      vigencia: caracteristica.vigencia
    };
  }

  private mapCaractsRelFromBackend(data: any[]): CaractsRel[] {
    return data.map(item => ({
      caracteristicaId: item.caracteristicaId,
      contextoId: item.contextoId
    }));
  }

  private mapCaractsRelToBackend(relacion: CaractsRel): any {
    return {
      caracteristicaId: relacion.caracteristicaId,
      contextoId: relacion.contextoId
    };
  }
}