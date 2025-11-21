import { Component, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { CommonModule } from '@angular/common';

/**
 * Componente de inicio de sesión
 * Aplica heurísticas de Nielsen para formularios
 */
@Component({
  selector: 'app-login',
  imports: [CommonModule, ...MATERIAL_IMPORTS, RouterModule, FormsModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  
  // Heurística 1: Visibilidad del estado del sistema
  cargando = signal(false);
  errorMensaje = signal('');
  
  hidePassword = true;
  loginForm: any;

  constructor(
    private fb: FormBuilder, 
    private authService: AuthService, 
    private router: Router
  ) {
    // Acepta tanto email como nombre_usuario para mayor flexibilidad
    this.loginForm = this.fb.group({
      emailOrUsername: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(5)]],
    });
  }

  /**
   * Procesa el inicio de sesión
   * Heurística 1: Feedback inmediato
   * Heurística 9: Mensajes de error claros
   */
  onSubmit() {
    // Limpiar error anterior
    this.errorMensaje.set('');

    // Heurística 5: Prevención de errores
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    // Heurística 1: Indicar que está procesando
    this.cargando.set(true);

    const inputValue = this.loginForm.value.emailOrUsername;
    
    // Determinar si es email o nombre de usuario
    const isEmail = inputValue.includes('@');
    
    // Construir credenciales según el formato que espera el backend
    const credentials: any = {
      password: this.loginForm.value.password
    };

    // Agregar email o nombre_usuario según corresponda
    if (isEmail) {
      credentials.email = inputValue;
    } else {
      credentials.nombreUsuario = inputValue;
    }

    this.authService.login(credentials).subscribe({
      next: (res) => {
        this.cargando.set(false);
        
        if (res && res.token) {
          // El token ya se guarda automáticamente en el servicio
          this.router.navigate(['/dashboard']);
        } else {
          // Heurística 9: Mensaje de error específico
          this.errorMensaje.set('No se recibió un token válido del servidor');
        }
      },
      error: (err) => {
        this.cargando.set(false);
        
        // Heurística 9: Mensajes de error específicos y útiles
        if (err.status === 401) {
          this.errorMensaje.set('Credenciales incorrectas. Verifica tu usuario/correo y contraseña.');
        } else if (err.status === 0) {
          this.errorMensaje.set('No se pudo conectar con el servidor. Verifica tu conexión.');
        } else if (err.status === 404) {
          this.errorMensaje.set('Usuario no encontrado. Verifica tus credenciales.');
        } else {
          this.errorMensaje.set('Error al iniciar sesión. Intenta nuevamente.');
        }
      }
    });
  }

  /**
   * Obtiene mensaje de error específico para cada campo
   * Heurística 9: Ayuda a diagnosticar errores
   */
  getErrorMessage(field: string): string {
    const control = this.loginForm.get(field);
    
    if (!control || !control.errors || !control.touched) {
      return '';
    }

    if (control.errors['required']) {
      return field === 'emailOrUsername' 
        ? 'El correo o nombre de usuario es requerido' 
        : 'La contraseña es requerida';
    }

    if (control.errors['minlength']) {
      return `La contraseña debe tener al menos ${control.errors['minlength'].requiredLength} caracteres`;
    }

    return '';
  }

  /**
   * Verifica si un campo tiene error y ha sido tocado
   * Heurística 5: Prevención de errores con feedback visual
   */
  hasError(field: string): boolean {
    const control = this.loginForm.get(field);
    return control ? control.invalid && control.touched : false;
  }
}