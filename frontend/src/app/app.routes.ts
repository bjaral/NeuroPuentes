import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/components/login/login.component';
import { SimulationComponent } from './features/simulation/components/simulation/simulation.component';

export const routes: Routes = [
    { 'path': 'login', 'component': LoginComponent },
    { 'path': 'simulation', 'component': SimulationComponent },

];
