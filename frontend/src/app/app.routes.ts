import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/auth/components/login/login.component';
import { RegisterComponent } from './features/auth/components/register/register.component';
import { SimulationComponent } from './features/simulation/components/simulation/simulation.component';
import { ScenarioConfigComponent } from './features/scenarios/components/scenario-config/scenario-config.component';
import { FeedbackComponent } from './features/feedback/components/feedback/feedback.component';
import { DashboardComponent } from './features/dashboard/components/dashboard/dashboard.component';
import { HistoryComponent } from './features/history/components/history/history.component';
import { HistoryDetailComponent } from './features/history/components/history-detail/history-detail.component';

import { LayoutComponent } from './layout/layout/layout.component';

export const routes: Routes = [
    // Vistas con header
    { 'path': '', component: LayoutComponent, canActivateChild: [AuthGuard], 'children': [
        { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
        { path: 'dashboard', component: DashboardComponent },
        { path: 'history', component: HistoryComponent },
        { path: 'history-detail/:id', component: HistoryDetailComponent },
        { path: 'scenarios', component: ScenarioConfigComponent },
        { path: 'feedback', component: FeedbackComponent },
    ]},

    // Vistas sin header
    { path: 'login', component: LoginComponent },
    { path: 'registro', component: RegisterComponent },
    { path: 'simulation', component: SimulationComponent, canActivate: [AuthGuard] },

    { path: '**', redirectTo: 'dashboard' },
];
