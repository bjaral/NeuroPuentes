import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatBottomSheetRef } from '@angular/material/bottom-sheet';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { RatingService } from '../../services/rating.service';
import { TokenService } from '../../../../core/services/token.service';

/**
 * Componente de calificación flotante
 * Permite al usuario calificar su experiencia del 0-10 con estrellas
 * y agregar un mensaje opcional
 */
@Component({
  selector: 'app-rating-widget',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ...MATERIAL_IMPORTS],
  templateUrl: './rating-widget.component.html',
  styleUrl: './rating-widget.component.scss'
})
export class RatingWidgetComponent {
  private fb = inject(FormBuilder);
  private ratingService = inject(RatingService);
  private tokenService = inject(TokenService);
  private bottomSheetRef = inject(MatBottomSheetRef<RatingWidgetComponent>);

  // Estado del componente
  hoveredRating = signal<number>(0);
  isSubmitting = signal<boolean>(false);
  submitSuccess = signal<boolean>(false);

  // Formulario
  ratingForm: FormGroup;

  // 5 estrellas (cada una representa 2 puntos, total 10)
  stars = [1, 2, 3, 4, 5];

  constructor() {
    this.ratingForm = this.fb.group({
      calificacion: [0, [Validators.required, Validators.min(0), Validators.max(10)]],
      mensaje: ['', [Validators.maxLength(500)]]
    });
  }

  /**
   * Selecciona una calificación según la posición del click
   * Click en lado izquierdo = media estrella (1 punto)
   * Click en lado derecho = estrella completa (2 puntos)
   */
  selectRating(starIndex: number, event: MouseEvent): void {
    const target = event.currentTarget as HTMLElement;
    const rect = target.getBoundingClientRect();
    const clickX = event.clientX - rect.left;
    const halfWidth = rect.width / 2;
    
    // Si el click es en la mitad izquierda, seleccionar media estrella
    const isHalf = clickX < halfWidth;
    const rating = isHalf ? (starIndex * 2) - 1 : starIndex * 2;
    
    this.ratingForm.patchValue({ calificacion: rating });
  }

  /**
   * Actualiza el hover de las estrellas según la posición del mouse
   */
  onStarHover(starIndex: number, event: MouseEvent): void {
    const target = event.currentTarget as HTMLElement;
    const rect = target.getBoundingClientRect();
    const hoverX = event.clientX - rect.left;
    const halfWidth = rect.width / 2;
    
    const isHalf = hoverX < halfWidth;
    const rating = isHalf ? (starIndex * 2) - 1 : starIndex * 2;
    
    this.hoveredRating.set(rating);
  }

  /**
   * Resetea el hover
   */
  onStarLeave(): void {
    this.hoveredRating.set(0);
  }

  /**
   * Determina el estado de visualización de cada estrella
   * Retorna: 'empty', 'half', o 'full'
   */
  getStarDisplay(starIndex: number): 'empty' | 'half' | 'full' {
    const currentRating = this.ratingForm.get('calificacion')?.value || 0;
    const displayRating = this.hoveredRating() || currentRating;
    
    const starValue = starIndex * 2; // Valor completo de la estrella
    const halfValue = (starIndex * 2) - 1; // Valor de media estrella
    
    if (displayRating >= starValue) {
      return 'full';
    } else if (displayRating >= halfValue) {
      return 'half';
    }
    return 'empty';
  }

  /**
   * Obtiene el texto descriptivo según la calificación
   */
  getRatingText(): string {
    const rating = this.hoveredRating() || this.ratingForm.get('calificacion')?.value || 0;
    
    if (rating === 0) return 'Sin calificar';
    if (rating <= 2) return 'Muy insatisfecho';
    if (rating <= 4) return 'Insatisfecho';
    if (rating <= 6) return 'Neutral';
    if (rating <= 8) return 'Satisfecho';
    if (rating <= 9) return 'Muy satisfecho';
    return '¡Excelente!';
  }

  /**
   * Envía la calificación
   */
  onSubmit(): void {
    if (this.ratingForm.invalid) {
      return;
    }

    const userId = this.tokenService.getNameIdentifier();
    if (!userId) {
      console.error('Usuario no autenticado');
      return;
    }

    this.isSubmitting.set(true);

    const calificacion = {
      usuarioId: userId,
      calificacion: this.ratingForm.get('calificacion')?.value,
      mensaje: this.ratingForm.get('mensaje')?.value || ''
    };

    this.ratingService.crearCalificacion(calificacion).subscribe({
      next: () => {
        this.submitSuccess.set(true);
        // Cerrar después de 2 segundos
        setTimeout(() => {
          this.bottomSheetRef.dismiss('success');
        }, 2000);
      },
      error: (error) => {
        console.error('Error al enviar calificación:', error);
        this.isSubmitting.set(false);
        // Aquí podrías mostrar un mensaje de error al usuario
      }
    });
  }

  /**
   * Cierra el widget
   */
  close(): void {
    this.bottomSheetRef.dismiss();
  }
}