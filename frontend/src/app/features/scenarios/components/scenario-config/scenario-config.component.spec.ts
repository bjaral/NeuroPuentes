import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScenarioConfigComponent } from './scenario-config.component';

describe('ScenarioConfigComponent', () => {
  let component: ScenarioConfigComponent;
  let fixture: ComponentFixture<ScenarioConfigComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScenarioConfigComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScenarioConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
