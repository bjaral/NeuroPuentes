import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/components/login/login.component';
import { SimulationComponent } from './features/simulation/components/simulation/simulation.component';
import { ScenarioConfigComponent } from './features/scenarios/components/scenario-config/scenario-config.component';
import { FeedbackComponent } from './features/feedback/components/feedback/feedback.component';
import { DashboardComponent } from './features/dashboard/components/dashboard/dashboard.component';
import { HistoryComponent } from './features/history/components/history/history.component';
import { HistoryDetailComponent } from './features/history/components/history-detail/history-detail.component';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'simulation', component: SimulationComponent },
    { path: 'scenarios', component: ScenarioConfigComponent },
    { path: 'feedback', component: FeedbackComponent },
    { path: 'dashboard', component: DashboardComponent },
    { path: 'history', component: HistoryComponent },
    { path: 'history-detail', component: HistoryDetailComponent }
];
