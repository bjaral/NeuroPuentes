import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatBottomSheet } from '@angular/material/bottom-sheet';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { RatingWidgetComponent } from '../rating-widget/rating-widget.component';
import { RatingService } from '../../services/rating.service';
import { TokenService } from '../../../../core/services/token.service';

/**
 * Componente del botón flotante que activa el widget de calificación
 * Se puede incluir en cualquier vista del sistema
 */
@Component({
  selector: 'app-rating-trigger',
  imports: [CommonModule, ...MATERIAL_IMPORTS],
  template: `
    <button 
      *ngIf="!yaCalificoEsteMes()"
      mat-fab 
      class="rating-trigger-btn"
      (click)="openRatingWidget()"
      [matTooltip]="'Califica tu experiencia'"
      matTooltipPosition="left"
      aria-label="Abrir calificación de satisfacción">
      <mat-icon>star_rate</mat-icon>
    </button>
  `,
  styles: [`
    @use '../../../../../styles.scss' as *;

    .rating-trigger-btn {
      position: fixed;
      bottom: 2rem;
      right: 2rem;
      z-index: 1000;
      background: linear-gradient(135deg, $accent-color, darken($accent-color, 10%));
      color: #424242;
      box-shadow: 0 4px 20px rgba($accent-color, 0.4);
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      animation: pulse-subtle 3s ease-in-out infinite;

      mat-icon {
        font-size: 1.75rem;
        width: 1.75rem;
        height: 1.75rem;
      }

      &:hover {
        box-shadow: 0 6px 28px rgba($accent-color, 0.6);
        transform: translateY(-4px) scale(1.05);
      }

      &:active {
        transform: translateY(-2px) scale(1);
      }
    }

    @keyframes pulse-subtle {
      0%, 100% {
        box-shadow: 0 4px 20px rgba(#FFD54F, 0.4);
      }
      50% {
        box-shadow: 0 4px 20px rgba(#FFD54F, 0.6), 0 0 0 8px rgba(#FFD54F, 0.1);
      }
    }

    @media (max-width: 768px) {
      .rating-trigger-btn {
        bottom: 1.5rem;
        right: 1.5rem;
        transform: scale(0.9);

        &:hover {
          transform: scale(0.95) translateY(-4px);
        }
      }
    }
  `]
})
export class RatingTriggerComponent implements OnInit {
  private bottomSheet = inject(MatBottomSheet);
  private ratingService = inject(RatingService);
  private tokenService = inject(TokenService);

  yaCalificoEsteMes = signal<boolean>(false);

  ngOnInit(): void {
    this.checkIfAlreadyRated();
  }

  /**
   * Verifica si el usuario ya calificó este mes
   */
  private checkIfAlreadyRated(): void {
    const userId = this.tokenService.getNameIdentifier();
    if (!userId) {
      this.yaCalificoEsteMes.set(true); // Ocultar si no está autenticado
      return;
    }

    this.ratingService.haCalificadoEsteMes(userId).subscribe({
      next: (yaCalifica) => {
        this.yaCalificoEsteMes.set(yaCalifica);
      },
      error: (error) => {
        console.error('Error al verificar calificación:', error);
        this.yaCalificoEsteMes.set(false); // Mostrar en caso de error
      }
    });
  }

  /**
   * Abre el widget de calificación
   */
  openRatingWidget(): void {
    const bottomSheetRef = this.bottomSheet.open(RatingWidgetComponent, {
      panelClass: 'rating-bottom-sheet',
      disableClose: false,
      hasBackdrop: true,
      backdropClass: 'rating-backdrop'
    });

    bottomSheetRef.afterDismissed().subscribe(result => {
      if (result === 'success') {
        this.yaCalificoEsteMes.set(true);
      }
    });
  }
}