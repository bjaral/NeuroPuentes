export interface Usuario {
  _id: number;
  nombre_usuario: string;
  nombre: string;
  email: string;
  password_hash: string;
  rol: string;
  vigencia: boolean;
  fecha_registro: string;
}
