/**
 * ========================================
 * SimulationService - UNIT TEST (HTTP)
 * ========================================
 *
 * Se prueban:
 * 1) Que construir FormData no falle
 * 2) Que haga POST a /api/Entrevistas/iniciar-con-ia
 */

import { TestBed } from '@angular/core/testing';
import { SimulationService } from './simulation.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

describe('SimulationService - Unit Tests', () => {

  let service: SimulationService;
  let httpMock: HttpTestingController;

  beforeEach(() => {

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [SimulationService]
    });

    service = TestBed.inject(SimulationService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  // ======================================================
  // TEST 1 — Petición HTTP correcta
  // ======================================================
  it('debe enviar POST a /api/Entrevistas/iniciar-con-ia con FormData', () => {

    const formData = new FormData();
    formData.append('usuarioId', '1');

    service.iniciarEntrevista(formData).subscribe();

    const req = httpMock.expectOne('http://localhost:5299/api/Entrevistas/iniciar-con-ia');

    expect(req.request.method).toBe('POST');
    expect(req.request.body instanceof FormData).toBeTrue();

    req.flush({});
  });

});
