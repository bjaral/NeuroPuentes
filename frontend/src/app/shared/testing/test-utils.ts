import { TestBed } from '@angular/core/testing';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';

// Configuración base para componentes
export const configureTestingModule = (component: any, providers: any[] = []) => {
  return TestBed.configureTestingModule({
    imports: [component],
    providers: [
      provideRouter([]), // Router por defecto
      ...providers
    ]
  });
};

// Configuración base para servicios HTTP
export const configureHttpServiceTest = (service: any) => {
  return TestBed.configureTestingModule({
    providers: [
      service,
      provideHttpClient(withInterceptorsFromDi()),
      provideHttpClientTesting()
    ]
  });
};