/**
 * =====================================================
 * DashboardService - UNIT TESTS (HTTP)
 * =====================================================
 *
 * Prueba:
 * 1. Que el servicio haga GET hacia:
 *    /api/Entrevistas/usuario/{id}
 *
 * 2. Que envíe correctamente el usuarioId
 * 3. Que devuelva un array
 */

import { TestBed } from '@angular/core/testing';
import { DashboardService } from './dashboard.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from '../../../core/services/auth.service';
import { environment } from '../../../../environments/environment';

describe('DashboardService - Unit Tests', () => {

  let service: DashboardService;
  let httpMock: HttpTestingController;

  const mockAuthService = {
    getCurrentUser: { name: 'Usuario Mock' }
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        DashboardService,
        { provide: AuthService, useValue: mockAuthService }
      ]
    });

    service = TestBed.inject(DashboardService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  // =====================================================
  // TEST 1 — GET /api/Entrevistas/usuario/{usuarioId}
  // =====================================================
  it('debe hacer una petición GET al endpoint correcto con el usuarioId', () => {

    const usuarioId = 123;

    service.getEntrevistasUsuario(usuarioId).subscribe();

    const req = httpMock.expectOne(
      `${environment.apiUrl}/api/Entrevistas/usuario/${usuarioId}`
    );

    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

});
