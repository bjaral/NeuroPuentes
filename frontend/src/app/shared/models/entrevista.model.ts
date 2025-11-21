export interface Entrevista {
  _id?: number;
  usuario_id?: number;
  contexto_id?: number;
  titulo?: string;
  descripcion?: string;
  duracion_min?: number;
  numero_turnos?: number;
  fecha_creacion?: string;
  fecha_cierre?: string | null;
  contexto_snapshot?: string;
}
