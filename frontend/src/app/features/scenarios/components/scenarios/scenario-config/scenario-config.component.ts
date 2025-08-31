import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatRippleModule } from '@angular/material/core';
import { ScenariosService } from '../../../services/scenarios.service';
import type { Scenario } from '../../../services/scenarios.service';
import { MATERIAL_IMPORTS } from '../../../../../shared/material/material';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-scenario-config',
  standalone: true,
  imports: [
    CommonModule,
    MATERIAL_IMPORTS,
    MatCardModule,
    MatButtonModule,
    MatDividerModule,
    MatIconModule,
    MatTooltipModule,
    MatRippleModule,
    RouterModule
],
  templateUrl: './scenario-config.component.html',
  styleUrls: ['./scenario-config.component.scss']
})
export class ScenarioConfigComponent implements OnInit {
  private scenariosService = inject(ScenariosService);
  
  scenarios: Scenario[] = [];
  selectedScenario = signal<Scenario | null>(null);
  isRandomizing = signal(false);
  pulsingScenarioId: number | null = null;

  ngOnInit(): void {
    this.loadScenarios();
  }

  private loadScenarios(): void {
    this.scenarios = this.scenariosService.getScenarios();
    
    // Seleccionar el primer escenario por defecto
    if (this.scenarios.length > 0) {
      this.selectedScenario.set(this.scenarios[0]);
    }
  }

  selectScenario(scenario: Scenario): void {
    if (this.isRandomizing()) return;
    
    this.selectedScenario.set(scenario);
    this.addSelectionEffect(scenario.id);
  }

  selectRandom(): void {
    if (this.isRandomizing()) return;
    
    this.isRandomizing.set(true);
    
    // Simular tiempo de selección con efecto visual
    setTimeout(() => {
      const randomScenario = this.scenariosService.getRandomScenario();
      this.selectedScenario.set(randomScenario);
      this.isRandomizing.set(false);
      this.addSelectionEffect(randomScenario.id);
    }, 1500);
  }

  startSimulation(): void {
    const scenario = this.selectedScenario();
    if (!scenario) return;

    // Log para debugging
    console.log('🎯 Iniciando simulación:', {
      id: scenario.id,
      title: scenario.title,
      edad: scenario.edad,
      contexto: scenario.contexto,
      tags: scenario.tags
    });
    
    // Confirmación visual
    const message = this.buildSimulationMessage(scenario);
    alert(message);
    
    // Aquí iría la lógica real de navegación
    // this.router.navigate(['/simulation'], { state: { scenario } });
  }

  private buildSimulationMessage(scenario: Scenario): string {
    let message = `🎯 Simulación iniciada:\n\n`;
    message += `📋 ${scenario.title}\n`;
    message += `🎂 Edad: ${scenario.edad} años\n\n`;
    message += `📝 Contexto:\n${scenario.contexto}\n`;
    
    if (scenario.tags && scenario.tags.length > 0) {
      message += `\n🏷️ Etiquetas: ${scenario.tags.join(', ')}`;
    }
    
    return message;
  }

  private addSelectionEffect(scenarioId: number): void {
    // Efecto visual para la selección usando class binding
    this.pulsingScenarioId = scenarioId;
    
    setTimeout(() => {
      this.pulsingScenarioId = null;
    }, 600);
  }

  // Métodos auxiliares para el template
  trackByScenario(index: number, scenario: Scenario): number {
    return scenario.id;
  }

  isScenarioSelected(scenario: Scenario): boolean {
    return this.selectedScenario()?.id === scenario.id;
  }

  getScenarioStats(): string {
    const total = this.scenarios.length;
    const avgAge = Math.round(
      this.scenarios.reduce((sum, s) => sum + s.edad, 0) / total
    );
    return `${total} escenarios disponibles (edad promedio: ${avgAge} años)`;
  }
}