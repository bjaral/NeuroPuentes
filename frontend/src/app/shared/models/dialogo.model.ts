export interface Dialogo {
  _id?: number;
  entrevista_id?: number;
  turno?: number;
  sender?: string;
  texto?: string;
  texto_procesado?: string;
  timestamp?: string;
  audio_url?: string | null;
}