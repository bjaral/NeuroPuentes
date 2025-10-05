import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { TokenService } from './token.service';
import { environment } from '../../../environments/environment';

/**
 * Interface para respuesta de login
 */
interface LoginResponse {
  token: string;
  usuario?: any;
}

/**
 * Interface para credenciales de login
 */
interface LoginCredentials {
  nombre_usuario?: string;
  email?: string;
  password: string;
}

/**
 * Interface para datos de registro
 */
interface RegisterData {
  nombre_usuario: string;
  nombre: string;
  email: string;
  password: string;
  rol: string;
  vigencia: boolean;
}

/**
 * Servicio de autenticación
 * Maneja login, registro y logout de usuarios
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {

  // Configurar según tu backend
  private authUrl = `http://localhost:5299/api/Auth`;
  private usuariosUrl = `http://localhost:5299/api/Usuarios`;

  constructor(
    private http: HttpClient,
    private tokenService: TokenService
  ) { }

  /**
   * Inicia sesión de usuario
   * @param credentials Credenciales (nombre_usuario o email + password)
   * @returns Observable con el token de autenticación
   */
  login(credentials: LoginCredentials): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.authUrl}/login`, credentials).pipe(
      tap(response => {
        // Guardar token automáticamente si viene en la respuesta
        if (response && response.token) {
          localStorage.setItem('tokenNeuroPuentes', response.token);
        }
      })
    );
  }

  /**
   * Registra un nuevo usuario
   * @param userData Datos del usuario a registrar
   * @returns Observable con la respuesta del servidor
   */
  register(userData: RegisterData): Observable<any> {
    return this.http.post(this.usuariosUrl, userData);
  }

  /**
   * Cierra sesión del usuario
   * Limpia el token almacenado
   */
  logout(): void {
    this.tokenService.clearToken();
  }

  /**
   * Verifica si el usuario está autenticado
   * @returns true si hay un token válido y no expirado
   */
  isAuthenticated(): boolean {
    const token = this.tokenService.getToken();
    
    if (!token) {
      return false;
    }

    return !this.tokenService.isTokenExpired();
  }

  /**
   * Obtiene el usuario actual desde el token
   * @returns Información del usuario o null
   */
  getCurrentUser(): any {
    return this.tokenService.decodeToken();
  }

  /**
   * Obtiene el ID del usuario actual
   * @returns ID del usuario o null
   */
  getCurrentUserId(): number | null {
    return this.tokenService.getNameIdentifier();
  }

  /**
   * Obtiene el rol del usuario actual
   * @returns Rol del usuario o null
   */
  getCurrentUserRole(): string | null {
    return this.tokenService.getRole();
  }
}