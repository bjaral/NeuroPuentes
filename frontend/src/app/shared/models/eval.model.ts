export interface EvalEntrevista {
  _id?: number;
  entrevista_id?: number;
  score_final?: number;
  comentario_general?: string | null;
}


export interface EvalCategoria {
  _id?: number;
  eval_entrevista_id?: number;
  categoria?: string;
  score?: number;
}