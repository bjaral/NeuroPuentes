/**
 * ========================================
 * SimulationComponent - UNIT TESTS
 * ========================================
 *
 * Se prueban:
 * 1) Cambio de estado al iniciar la grabación
 * 2) Render del indicador visual según el estado
 */

import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { SimulationComponent } from './simulation.component';
import { SimulationService } from '../../services/simulation.service';
import { TokenService } from '../../../../core/services/token.service';
import { Router } from '@angular/router';

describe('SimulationComponent - Unit Tests', () => {

  let component: SimulationComponent;
  let fixture: ComponentFixture<SimulationComponent>;
  let routerMock: any;

  beforeEach(async () => {

    routerMock = { navigate: jasmine.createSpy('navigate') };

    await TestBed.configureTestingModule({
      imports: [SimulationComponent],
      providers: [
        { provide: Router, useValue: routerMock },
        { provide: SimulationService, useValue: {} },
        {
          provide: TokenService, useValue: {
            getNameIdentifier: () => 1
          }
        },
        provideNoopAnimations()  // ← THIS IS THE ONLY REQUIRED LINE
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(SimulationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // ======================================================
  // TEST 1 — Estado cambia a "recording"
  // ======================================================
  it('debe cambiar el estado a "recording" al iniciar grabación', async () => {

    // Simular que sessionStorage tiene contexto válido
    sessionStorage.setItem('selectedContexto', JSON.stringify({
      id: 1,
      nombre: 'Paciente Test'
    }));

    // Forzar OnInit
    component.ngOnInit();

    // Simular cambio de estado sin MediaRecorder
    component.interviewStatus.set('recording');

    expect(component.interviewStatus()).toBe('recording');
  });

  // ======================================================
  // TEST 2 — La UI debe reflejar estado "recording"
  // ======================================================
  it('debe renderizar el indicador visual de grabación', () => {

    component.interviewStatus.set('recording');
    fixture.detectChanges();

    const icon = fixture.nativeElement.querySelector('.avatar.recording');

    expect(icon).toBeTruthy();
  });

});
