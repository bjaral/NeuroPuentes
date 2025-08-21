import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/components/login/login.component';
import { SimulationComponent } from './features/simulation/components/simulation/simulation.component';
import { ScenarioConfigComponent } from './features/scenarios/components/scenarios/scenario-config/scenario-config.component';
import { FeedbackComponent } from './features/feedback/components/feedback/feedback.component';

export const routes: Routes = [
  { 'path': 'login', 'component': LoginComponent },
  { 'path': 'simulation', 'component': SimulationComponent },
  { 'path': 'scenarios', component: ScenarioConfigComponent },
  { 'path': 'feedback', 'component': FeedbackComponent }
];
