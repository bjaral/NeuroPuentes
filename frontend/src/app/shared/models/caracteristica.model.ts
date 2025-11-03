export interface Caracteristica {
  id?: number;
  nombre?: string;
  descripcion?: string;
  grupo?: string;
  vigencia?: boolean;
}

export interface CaractsRel {
  caracteristicaId?: number;
  contextoId?: number;
}