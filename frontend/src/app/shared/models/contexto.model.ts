export interface Contexto {
  _id?: number;
  nombre?: string;
  descripcion?: string;
  scope?: string;
  creado_por?: number | null;
  origen?: string;
  prompt_seed?: string | null;
  vigencia?: boolean;
  fecha_creacion?: string;
}
