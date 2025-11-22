import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';

export interface CalificacionUsuario {
  id?: number;
  usuarioId: number;
  calificacion: number;
  mensaje: string;
  fecha?: string;
}

@Injectable({
  providedIn: 'root'
})
export class RatingService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/CalificacionUsuario`;

  /**
   * Obtiene todas las calificaciones de un usuario
   */
  getCalificacionesByUsuario(usuarioId: number): Observable<CalificacionUsuario[]> {
    return this.http.get<CalificacionUsuario[]>(`${this.apiUrl}/usuario/${usuarioId}`);
  }

  /**
   * Verifica si el usuario ya calificó en el mes actual
   */
  haCalificadoEsteMes(usuarioId: number): Observable<boolean> {
    return this.getCalificacionesByUsuario(usuarioId).pipe(
      map(calificaciones => {
        if (calificaciones.length === 0) return false;

        const now = new Date();
        const mesActual = now.getMonth();
        const anioActual = now.getFullYear();

        return calificaciones.some(cal => {
          if (!cal.fecha) return false;
          const fechaCal = new Date(cal.fecha);
          return fechaCal.getMonth() === mesActual && 
                 fechaCal.getFullYear() === anioActual;
        });
      })
    );
  }

  /**
   * Crea una nueva calificación
   */
  crearCalificacion(calificacion: CalificacionUsuario): Observable<CalificacionUsuario> {
    return this.http.post<CalificacionUsuario>(this.apiUrl, calificacion);
  }

  /**
   * Obtiene la última calificación del usuario
   */
  getUltimaCalificacion(usuarioId: number): Observable<CalificacionUsuario | null> {
    return this.getCalificacionesByUsuario(usuarioId).pipe(
      map(calificaciones => {
        if (calificaciones.length === 0) return null;
        // Ordenar por fecha descendente y retornar la más reciente
        return calificaciones.sort((a, b) => {
          if (!a.fecha || !b.fecha) return 0;
          return new Date(b.fecha).getTime() - new Date(a.fecha).getTime();
        })[0];
      })
    );
  }
}