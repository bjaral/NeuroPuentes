import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';

export interface IAResponse {
  transcription: string;
  response_text: string;
  audio_base64: string;
  session_id: string; // Este será el entrevistaId
  success: boolean;
  error: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class SimulationService {

  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl || 'http://localhost:5299';
  
  private entrevistasUrl = `${this.apiUrl}/api/Entrevistas`;
  private dialogosUrl = `${this.apiUrl}/api/Dialogos`;

  /**
   * Inicia la entrevista y crea el primer diálogo.
   * Llama a: POST /api/Entrevistas/iniciar-con-ia
   */
  iniciarEntrevista(formData: FormData): Observable<IAResponse | null> {
    return this.http.post<IAResponse>(`${this.entrevistasUrl}/iniciar-con-ia`, formData).pipe(
      catchError(error => {
        console.error('Error en /iniciar-con-ia:', error);
        return of(null);
      })
    );
  }

  /**
   * Envía el audio del usuario para continuar la conversación.
   * Llama a: POST /api/Dialogos/continuar-con-ia
   */
  continuarDialogo(formData: FormData): Observable<IAResponse | null> {
    return this.http.post<IAResponse>(`${this.dialogosUrl}/continuar-con-ia`, formData).pipe(
      catchError(error => {
        console.error('Error en /continuar-con-ia:', error);
        return of(null);
      })
    );
  }
}