export interface FeedbackEntrevista {
  _id: number;
  entrevista_id: number;
  tipo: string;
  mensaje: string;
  categoria?: string | null;
  fecha: string;
}

export interface FeedbackUsuario {
  _id: number;
  usuario_id: number;
  tipo: string;
  mensaje: string;
  categoria?: string | null;
  fecha: string;
}
