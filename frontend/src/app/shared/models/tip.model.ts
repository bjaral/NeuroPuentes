export interface Tip {
  _id: number;
  usuario_id: number;
  entrevista_id?: number | null;
  titulo?: string | null;
  contenido: string;
  categoria: string;
  fecha: string;
  usado: boolean;
  vigencia: boolean;
}
