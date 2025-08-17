import { Routes } from '@angular/router';
<<<<<<< HEAD
import { LoginComponent } from './features/auth/components/login/login.component';
import { SimulationComponent } from './features/simulation/components/simulation/simulation.component';

export const routes: Routes = [
    { 'path': 'login', 'component': LoginComponent },
    { 'path': 'simulation', 'component': SimulationComponent },

];
=======
import { ScenarioConfigComponent } from './features/scenarios/components/scenarios/scenario-config/scenario-config.component';
import { FeedbackComponent } from './features/feedback/components/feedback/feedback.component';

export const routes: Routes = [
  { path: 'scenarios', component: ScenarioConfigComponent},
  { 'path': 'feedback', 'component': FeedbackComponent }
];
>>>>>>> 5b699481f60893d9c1f639dda6f708fc1cc1432d
