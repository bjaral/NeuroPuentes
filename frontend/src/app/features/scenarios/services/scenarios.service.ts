import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Contexto } from '../../../shared/models/contexto.model';

export interface Scenario {
  id: number;
  title: string;
  edad: number;
  contexto: string;
  vistaPrevia: string;
  icono?: string;
  tags?: string[];
}

@Injectable({ 
  providedIn: 'root' 
})
export class ScenariosService {

  private apiUrl = `http://localhost:5299/api/Contextos`;
private contextosEjemplo: Contexto[] = [
    {
      _id: 1,
      nombre: 'Niño de 4 años - Primeras señales',
      descripcion: 'Padres preocupados por desarrollo social y comunicación del niño',
      scope: 'evaluacion_inicial',
      origen: 'sistema',
      prompt_seed: 'Padre: "Mi hijo no nos mira a los ojos cuando le hablamos". No responde cuando lo llaman por su nombre. Prefiere jugar solo con sus carritos.',
      vigencia: true,
      fecha_creacion: '2024-01-15'
    },
    {
      _id: 2,
      nombre: 'Niña de 6 años - Diagnóstico reciente',
      descripcion: 'Familia adaptándose al diagnóstico de TEA y buscando apoyo',
      scope: 'seguimiento',
      origen: 'sistema',
      prompt_seed: 'Madre: "Nos acaban de confirmar que tiene autismo". Dificultades en el colegio con ruidos fuertes. Muy inteligente pero evita juegos grupales.',
      vigencia: true,
      fecha_creacion: '2024-01-20'
    },
    {
      _id: 3,
      nombre: 'Adolescente de 14 años - Transición',
      descripcion: 'Desafíos de la adolescencia con autismo',
      scope: 'intervencion',
      origen: 'sistema',
      prompt_seed: 'Padre: "Desde que empezó la secundaria está muy ansioso". Cambios físicos de la pubertad lo angustian. Dificultades para hacer amigos.',
      vigencia: true,
      fecha_creacion: '2024-02-01'
    },
    {
      _id: 4,
      nombre: 'Niño de 8 años - Comportamientos desafiantes',
      descripcion: 'Crisis conductuales que afectan la dinámica familiar',
      scope: 'crisis',
      origen: 'sistema',
      prompt_seed: 'Madre: "Últimamente tiene rabietas muy intensas". Se golpea la cabeza cuando está frustrado. Hermanos menores tienen miedo de sus crisis.',
      vigencia: true,
      fecha_creacion: '2024-02-10'
    },
    {
      _id: 5,
      nombre: 'Niña de 10 años - Perfil femenino',
      descripcion: 'Diagnóstico tardío en niña con enmascaramiento social',
      scope: 'evaluacion_tardia',
      origen: 'sistema',
      prompt_seed: 'Madre: "Siempre pensé que era solo muy tímida". Imita comportamientos de sus compañeras. Llega agotada del colegio, colapsa en casa.',
      vigencia: true,
      fecha_creacion: '2024-02-15'
    }
  ];

  constructor(private http: HttpClient) { }

  /**
   * Obtiene todos los contextos vigentes
   */
  getContextos(): Observable<Contexto[]> {
    // En producción, usar la API real:
    // return this.http.get<Contexto[]>(`${this.apiUrl}?vigencia=true`);
    
    // Desarrollo: devolver datos mock
    return new Observable(observer => {
      setTimeout(() => {
        observer.next(this.contextosEjemplo.filter(c => c.vigencia));
        observer.complete();
      }, 400);
    });
  }

  /**
   * Obtiene un contexto por ID
   */
  getContextoById(id: number): Observable<Contexto | undefined> {
    // En producción:
    // return this.http.get<Contexto>(`${this.apiUrl}/${id}`);
    
    // Desarrollo:
    return new Observable(observer => {
      const contexto = this.contextosEjemplo.find(c => c._id === id);
      observer.next(contexto);
      observer.complete();
    });
  }

  /**
   * Obtiene un contexto aleatorio
   */
  getContextoAleatorio(): Observable<Contexto> {
    return this.getContextos().pipe(
      map(contextos => {
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
      map(contextos => contextos.filter(c => c.scope === scope))
    );
  }

  /**
   * Crea un nuevo contexto (solo para usuarios autorizados)
   */
  crearContexto(contexto: Contexto): Observable<Contexto> {
    return this.http.post<Contexto>(this.apiUrl, contexto);
  }

  /**
   * Actualiza un contexto existente
   */
  actualizarContexto(id: number, contexto: Partial<Contexto>): Observable<Contexto> {
    return this.http.put<Contexto>(`${this.apiUrl}/${id}`, contexto);
  }

  /**
   * Desactiva un contexto (soft delete)
   */
  desactivarContexto(id: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}`, { vigencia: false });
  }

  /**
   * Obtiene estadísticas de los contextos
   */
  getEstadisticas(): { total: number; porScope: { [key: string]: number } } {
    const contextos = this.contextosEjemplo.filter(c => c.vigencia);
    const porScope: { [key: string]: number } = {};
    
    contextos.forEach(c => {
      const scope = c.scope || 'sin_categoria';
      porScope[scope] = (porScope[scope] || 0) + 1;
    });

    return {
      total: contextos.length,
      porScope
    };
  }
}