/**
 * ============================================================
 * LayoutComponent - UNIT TESTS
 * ============================================================
 *
 * Este test valida que:
 *
 * 1. El layout renderiza correctamente el <app-header>.
 * 2. El layout contiene el <router-outlet>.
 * 3. El componente carga correctamente sus imports standalone.
 *
 * Como LayoutComponent no tiene lógica interna, estos tests
 * verifican la estructura del template y la integración básica.
 * ============================================================
 */

import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LayoutComponent } from './layout.component';
import { HeaderComponent } from '../header/header.component';
import { RouterOutlet } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';

describe('LayoutComponent', () => {

  let fixture: ComponentFixture<LayoutComponent>;
  let component: LayoutComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        LayoutComponent,     // standalone component
        HeaderComponent,     // standalone component
        RouterTestingModule  // para soportar <router-outlet>
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // ============================================================
  // TEST 1 — El componente debe crearse correctamente
  // ============================================================
  it('debe crear el componente', () => {
    expect(component).toBeTruthy();
  });

  // ============================================================
  // TEST 2 — El layout debe contener el header
  // ============================================================
  it('debe renderizar el HeaderComponent', () => {
    const header = fixture.nativeElement.querySelector('app-header');
    expect(header).toBeTruthy();
  });

  // ============================================================
  // TEST 3 — El layout debe tener un router-outlet
  // ============================================================
  it('debe renderizar router-outlet', () => {
    const outlet = fixture.nativeElement.querySelector('router-outlet');
    expect(outlet).toBeTruthy();
  });

});
