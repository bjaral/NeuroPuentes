import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { DashboardComponent } from './dashboard.component';
import { TokenService } from '../../../../core/services/token.service';
import { DashboardService } from '../../services/dashboard.service';
import { AuthService } from '../../../../core/services/auth.service';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';

// Mocks simples
class MockTokenService {
  getName = jasmine.createSpy('getName').and.returnValue('Juan Pérez');
  getNameIdentifier = jasmine.createSpy('getNameIdentifier').and.returnValue('123');
}

class MockDashboardService {
  getEntrevistasUsuario = jasmine.createSpy('getEntrevistasUsuario').and.returnValue(
    of([
      {
        id: 1,
        titulo: 'Entrevista Técnica Frontend',
        fechaCierre: '2024-01-15T10:30:00Z'
      },
      {
        id: 2,
        titulo: 'Entrevista Behavioral',
        fechaCierre: '2024-01-10T14:20:00Z'
      }
    ])
  );
}

class MockAuthService {
  isAuthenticated = jasmine.createSpy('isAuthenticated').and.returnValue(true);
}

class MockRouter {
  navigate = jasmine.createSpy('navigate');
}

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;
  let tokenService: MockTokenService;
  let dashboardService: MockDashboardService;
  let router: MockRouter;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardComponent], // Solo el componente standalone
      providers: [
        { provide: TokenService, useClass: MockTokenService },
        { provide: DashboardService, useClass: MockDashboardService },
        { provide: AuthService, useClass: MockAuthService },
        { provide: Router, useClass: MockRouter },
        provideRouter([]), // Enfoque moderno para routing
        provideHttpClient(),
        provideHttpClientTesting(),
        provideNoopAnimations()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    
    tokenService = TestBed.inject(TokenService) as unknown as MockTokenService;
    dashboardService = TestBed.inject(DashboardService) as unknown as MockDashboardService;
    router = TestBed.inject(Router) as unknown as MockRouter;
  });

  // =========================================================================
  // PRUEBAS BÁSICAS
  // =========================================================================

  it('debería crear el componente correctamente', () => {
    expect(component).toBeTruthy();
  });

  it('debería cargar datos al inicializar', () => {
    fixture.detectChanges();
    
    expect(tokenService.getName).toHaveBeenCalled();
    expect(dashboardService.getEntrevistasUsuario).toHaveBeenCalledWith(123);
    expect(component.nombre).toBe('Juan Pérez');
  });

  // =========================================================================
  // PRUEBAS DE FUNCIONALIDAD PRINCIPAL
  // =========================================================================

  it('debería procesar entrevistas correctamente', () => {
    fixture.detectChanges();

    expect(component.entrevistasRealizadas).toBe(2);
    expect(component.entrevistas.length).toBe(2);
    expect(component.entrevistas[0].titulo).toBe('Entrevista Técnica Frontend');
  });

  it('debería manejar lista vacía de entrevistas', () => {
    dashboardService.getEntrevistasUsuario.and.returnValue(of([]));
    
    fixture.detectChanges();

    expect(component.entrevistasRealizadas).toBe(0);
    expect(component.entrevistas.length).toBe(0);
  });

  it('debería manejar errores del servicio', () => {
    dashboardService.getEntrevistasUsuario.and.returnValue(
      throwError(() => new Error('Error del servidor'))
    );

    fixture.detectChanges();

    expect(component.errorCargando).toBeTrue();
    expect(component.entrevistas).toEqual([]);
  });

  // =========================================================================
  // PRUEBAS DE NAVEGACIÓN
  // =========================================================================

  it('debería navegar al detalle de entrevista', () => {
    fixture.detectChanges();
    
    component.toDetalle(1);
    
    expect(router.navigate).toHaveBeenCalledWith(['/history-detail', 1]);
  });

  it('no debería navegar con ID inválido', () => {
    component.toDetalle(undefined);
    expect(router.navigate).not.toHaveBeenCalled();
    
    component.toDetalle(null as any);
    expect(router.navigate).not.toHaveBeenCalled();
  });

  // =========================================================================
  // PRUEBAS DE FORMATEO
  // =========================================================================

  it('debería formatear fechas correctamente', () => {
    const fecha = '2024-12-25T15:30:00Z';
    const formateada = component['formatearFecha'](fecha);
    
    expect(formateada).toBe('25-12-2024');
  });

  it('debería manejar fechas inválidas', () => {
    expect(component['formatearFecha']('')).toBe('Sin fecha');
    expect(component['formatearFecha'](null as any)).toBe('Sin fecha');
  });
});