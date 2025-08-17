import { Routes } from '@angular/router';
import { ScenarioConfigComponent } from './features/scenarios/components/scenarios/scenario-config/scenario-config.component';
import { FeedbackComponent } from './features/feedback/components/feedback/feedback.component';

export const routes: Routes = [
  { path: 'scenarios', component: ScenarioConfigComponent},
  { 'path': 'feedback', 'component': FeedbackComponent }
];