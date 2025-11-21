import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, FormsModule, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { AuthService } from '../../../../core/services/auth.service';

/**
 * Componente de registro de usuarios
 * Aplica heurísticas de Nielsen para formularios
 */
@Component({
  selector: 'app-register',
  imports: [CommonModule, ...MATERIAL_IMPORTS, RouterModule, FormsModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {

  // Heurística 1: Visibilidad del estado
  cargando = signal(false);
  errorMensaje = signal('');
  exitoMensaje = signal('');

  hidePassword = true;
  hideConfirmPassword = true;

  registerForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      nombre_usuario: ['', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(20)
      ]],
      nombre: ['', [
        Validators.required,
        Validators.minLength(2)
      ]],
      email: ['', [
        Validators.required,
        Validators.email
      ]],
      password: ['', [
        Validators.required,
        Validators.minLength(6)
      ]],
      confirmPassword: ['', Validators.required]
    }, { 
      validators: this.passwordMatchValidator 
    });
  }

  /**
   * Validador personalizado para verificar que las contraseñas coincidan
   * Heurística 5: Prevención de errores
   */
  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (!password || !confirmPassword) {
      return null;
    }

    return password.value === confirmPassword.value ? null : { passwordMismatch: true };
  }

  /**
   * Procesa el registro del usuario
   * Heurística 1: Feedback inmediato
   * Heurística 9: Mensajes de error claros
   */
  onSubmit() {
    // Limpiar mensajes anteriores
    this.errorMensaje.set('');
    this.exitoMensaje.set('');

    // Heurística 5: Prevención de errores
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    // Heurística 1: Indicar que está procesando
    this.cargando.set(true);

    const userData = {
      nombreUsuario: this.registerForm.value.nombre_usuario,
      nombre: this.registerForm.value.nombre,
      email: this.registerForm.value.email,
      password: this.registerForm.value.password,
      rol: 'Estudiante',
      vigencia: true
    };

    this.authService.register(userData).subscribe({
      next: (res) => {
        this.cargando.set(false);
        
        // Heurística 1: Feedback de éxito
        this.exitoMensaje.set('¡Cuenta creada exitosamente! Redirigiendo al login...');
        
        // Redirigir al login después de 2 segundos
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (err) => {
        this.cargando.set(false);
        
        // Heurística 9: Mensajes de error específicos
        if (err.status === 409) {
          this.errorMensaje.set('El usuario o email ya están registrados. Intenta con otros datos.');
        } else if (err.status === 400) {
          this.errorMensaje.set('Datos inválidos. Verifica la información ingresada.');
        } else if (err.status === 0) {
          this.errorMensaje.set('No se pudo conectar con el servidor. Verifica tu conexión.');
        } else {
          this.errorMensaje.set('Error al crear la cuenta. Intenta nuevamente.');
        }
      }
    });
  }

  /**
   * Obtiene mensaje de error específico para cada campo
   * Heurística 9: Ayuda a diagnosticar errores
   */
  getErrorMessage(field: string): string {
    const control = this.registerForm.get(field);
    
    if (!control || !control.errors || !control.touched) {
      return '';
    }

    if (control.errors['required']) {
      const fieldNames: { [key: string]: string } = {
        nombre_usuario: 'El nombre de usuario',
        nombre: 'El nombre',
        email: 'El correo',
        password: 'La contraseña',
        confirmPassword: 'La confirmación de contraseña'
      };
      return `${fieldNames[field]} es requerido`;
    }

    if (control.errors['email']) {
      return 'Ingresa un correo válido';
    }

    if (control.errors['minlength']) {
      const minLength = control.errors['minlength'].requiredLength;
      return `Debe tener al menos ${minLength} caracteres`;
    }

    if (control.errors['maxlength']) {
      const maxLength = control.errors['maxlength'].requiredLength;
      return `No puede tener más de ${maxLength} caracteres`;
    }

    return '';
  }

  /**
   * Obtiene mensaje de error para contraseñas que no coinciden
   * Heurística 9: Mensajes específicos
   */
  getPasswordMismatchError(): string {
    const confirmPassword = this.registerForm.get('confirmPassword');
    
    if (confirmPassword?.touched && this.registerForm.errors?.['passwordMismatch']) {
      return 'Las contraseñas no coinciden';
    }
    
    return '';
  }

  /**
   * Verifica si un campo tiene error y ha sido tocado
   * Heurística 5: Prevención de errores con feedback visual
   */
  hasError(field: string): boolean {
    const control = this.registerForm.get(field);
    return control ? control.invalid && control.touched : false;
  }

  /**
   * Verifica si las contraseñas no coinciden
   * Heurística 5: Feedback visual de error
   */
  hasPasswordMismatch(): boolean {
    const confirmPassword = this.registerForm.get('confirmPassword');
    return confirmPassword?.touched && this.registerForm.errors?.['passwordMismatch'] || false;
  }
}