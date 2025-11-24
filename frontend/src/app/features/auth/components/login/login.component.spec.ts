/**
 * ===========================
 * LoginComponent - UNIT TESTS
 * ===========================
 * 
 * Pruebas requeridas:
 * 6. LoginComponent - Validación: Verifica que el estado inicial del formulario sea inválido (invalid) 
 *    y que el botón de "Ingresar" permanezca deshabilitado visualmente hasta que se completen los campos requeridos.
 * 7. LoginComponent - Envío: Simula la interacción de llenar los campos de email/password y hacer clic en el botón, 
 *    verificando que el componente invoque al método login() del AuthService con los parámetros correctos.
 */

import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from './login.component';
import { AuthService } from '../../../../core/services/auth.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { provideRouter } from '@angular/router';

// Mocks simples
class MockAuthService {
  login = jasmine.createSpy('login').and.returnValue(of({ token: 'fake-token' }));
}

class MockRouter {
  navigate = jasmine.createSpy('navigate');
}

describe('LoginComponent - Unit Tests', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authService: MockAuthService;
  let router: MockRouter;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginComponent], // Solo el componente standalone - NO otros imports
      providers: [
        { provide: AuthService, useClass: MockAuthService },
        { provide: Router, useClass: MockRouter },
        provideRouter([]), // ✅ Enfoque moderno para routing
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    authService = TestBed.inject(AuthService) as unknown as MockAuthService;
    router = TestBed.inject(Router) as unknown as MockRouter;
    
    fixture.detectChanges();
  });

  // ======================================================
  // TEST 6 - Validación del formulario
  // ======================================================

  describe('Validación del formulario', () => {
    
    it('debe iniciar con formulario inválido', () => {
      // Verificar que el estado inicial del formulario sea inválido
      expect(component.loginForm.valid).toBeFalse();
    });

    it('debe tener campos requeridos marcados como inválidos al inicio', () => {
      expect(component.loginForm.get('emailOrUsername')?.valid).toBeFalse();
      expect(component.loginForm.get('password')?.valid).toBeFalse();
    });

    it('el botón "Iniciar Sesión" debe estar deshabilitado cuando el formulario es inválido', () => {
      // Verificar que el botón permanece deshabilitado visualmente
      const button: HTMLButtonElement = fixture.nativeElement.querySelector('button[type="submit"]');
      expect(button.disabled).toBeTrue();
    });

    it('el botón "Iniciar Sesión" debe habilitarse cuando el formulario es válido', () => {
      // Completar campos requeridos
      component.loginForm.setValue({
        emailOrUsername: 'usuario123',
        password: '12345'
      });

      fixture.detectChanges();

      // Verificar que el botón se habilita visualmente
      const button: HTMLButtonElement = fixture.nativeElement.querySelector('button[type="submit"]');
      expect(button.disabled).toBeFalse();
    });
  });

  // ======================================================
  // TEST 7 - Envío del formulario
  // ======================================================

  describe('Envío del formulario', () => {
    
    it('debe llamar a authService.login() con credenciales correctas usando nombre de usuario', () => {
      // Simular interacción de llenar campos
      component.loginForm.setValue({
        emailOrUsername: 'usuario123',
        password: '12345'
      });

      fixture.detectChanges();

      // Simular clic en el botón (envío del formulario)
      component.onSubmit();

      // Verificar que se invoca al método login() del AuthService con parámetros correctos
      expect(authService.login).toHaveBeenCalledWith({
        nombreUsuario: 'usuario123',
        password: '12345'
      });
    });

    it('debe llamar a authService.login() con credenciales correctas usando email', () => {
      // Simular interacción con email
      component.loginForm.setValue({
        emailOrUsername: 'usuario@ejemplo.com',
        password: '12345'
      });

      component.onSubmit();

      // Verificar que detecta email y usa el campo correcto
      expect(authService.login).toHaveBeenCalledWith({
        email: 'usuario@ejemplo.com',
        password: '12345'
      });
    });

    it('debe navegar al dashboard después de login exitoso', () => {
      component.loginForm.setValue({
        emailOrUsername: 'usuario123',
        password: '12345'
      });

      component.onSubmit();

      // Verificar navegación
      expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
    });

    it('NO debe llamar a authService.login() cuando el formulario es inválido', () => {
      // Dejar formulario vacío (inválido)
      component.loginForm.setValue({
        emailOrUsername: '',
        password: ''
      });

      component.onSubmit();

      // Verificar que NO se llama al servicio
      expect(authService.login).not.toHaveBeenCalled();
    });

    it('debe marcar todos los campos como tocados al enviar formulario inválido', () => {
      // Formulario inválido
      component.loginForm.setValue({
        emailOrUsername: '',
        password: '123' // Contraseña muy corta
      });

      component.onSubmit();

      // Verificar validación visual
      expect(component.loginForm.get('emailOrUsername')?.touched).toBeTrue();
      expect(component.loginForm.get('password')?.touched).toBeTrue();
    });
  });
});