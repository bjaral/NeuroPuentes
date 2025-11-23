// angular core
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TokenService } from '../../../../core/services/token.service';
import { AuthService } from '../../../../core/services/auth.service';
import { DashboardService } from '../../services/dashboard.service';
import { Router } from '@angular/router';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { RatingTriggerComponent } from '../../../rating/components/rating-trigger/rating-trigger.component';

interface Entrevista {
  titulo: string;
  fecha: string;
  id?: number;
}

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonModule,
    RouterModule, ...MATERIAL_IMPORTS, RatingTriggerComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  nombre: string = 'Usuario';
  entrevistasRealizadas = 0;
  errorCargando = false;
  cargandoDatos = true;
  entrevistas: Entrevista[] = [];

  constructor(private tokenService: TokenService, private dashboardService: DashboardService, private authService: AuthService, private router: Router) { }

  ngOnInit() {
    this.cargarEntrevistas();
    const nombre = this.tokenService.getName();
    if (nombre) {
      this.nombre = nombre;
    }
    console.log('Nombre: ', this.nombre);
  }

  nuevaEntrevista() {
    // Mejorado: feedback más específico
    console.log('Redirigiendo a nueva entrevista...');
  }

  simularEntrevista() {
    // Mejorado: feedback más específico
    console.log('Iniciando simulación...');
  }

  // Manejo de errores y estados
  async cargarEntrevistas() {
    this.cargandoDatos = true;
    this.errorCargando = false;

    try {
      const usuarioId = Number(this.tokenService.getNameIdentifier());

      if (!usuarioId) {
        throw new Error('ID de usuario no encontrado');
      }

      this.dashboardService.getEntrevistasUsuario(usuarioId).subscribe({
        next: (data) => {
          //Validar datos antes de asignar
          if (data && Array.isArray(data)) {
            //Actualizar el contador de entrevistas realizadas
            this.entrevistasRealizadas = data.length;

            //Filtrar solo las que tienen fecha de cierre
            const entrevistasCompletadas = data.filter(e => e.fechaCierre);

            //Ordenar por fecha de cierra (más reciente primero)
            entrevistasCompletadas.sort((a, b) => {
              const fechaA = new Date(a.fechaCierre).getTime();
              const fechaB = new Date(b.fechaCierre).getTime();
              return fechaB - fechaA;
            });

            //Tomar solo las últimas 5 entrevistas completadas
            const ultimas5 = entrevistasCompletadas.slice(0, 5);

            //Mapear las entrevistas al formato esperado
            this.entrevistas = ultimas5.map(e => ({
              id: e.id,
              titulo: e.titulo || 'Sin título',
              fecha: this.formatearFecha(e.fechaCierre),
              //puntaje: 0,
              //estado: this.determinarEstado(e.fecha_cierre)
            }));

            this.errorCargando = false;
          } else {
            throw new Error('Datos de entrevistas inválidos');
          }
          this.cargandoDatos = false;
        },
        error: (err) => {
          console.error('Error cargando entrevistas: ', err);
          this.errorCargando = true;
          this.entrevistas = [];
          this.entrevistasRealizadas = 0;
          this.cargandoDatos = false;
        }
      });

    } catch (error) {
      console.error('Error cargando entrevistas:', error);
      this.errorCargando = true;
      this.entrevistas = [];
    } finally {
      this.cargandoDatos = false;
    }
  }

  private formatearFecha(fecha: string): string {
    if (!fecha) return 'Sin fecha';
    const date = new Date(fecha);
    return date.toLocaleDateString('es-CL', {
      year: 'numeric', month: '2-digit', day: '2-digit'
    });
  }

  // Función para reintentar carga
  reintentarCarga() {
    this.cargarEntrevistas();
  }

  toDetalle(id?: number) {
    if (!id) return;
    this.router.navigate(['/history-detail', id]);
  }
}