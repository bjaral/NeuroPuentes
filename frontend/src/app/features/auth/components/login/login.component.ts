import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [CommonModule, ...MATERIAL_IMPORTS, RouterModule, FormsModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  hidePassword = true;
  loginForm: any;

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(5)]],
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const credentials = {
        correo: this.loginForm.value.email,
        password: this.loginForm.value.password
      };

      this.authService.login(credentials).subscribe({
        next: (res) => {
          if (res && res.token) {
            localStorage.setItem('tokenNeuroPuentes', res.token);
          } else {
            // Manejar caso donde no hay token en la respuesta
          }
          console.log('Inicio de sesión exitoso');
          this.toPerfil();
        },
        error: (err) => {
          alert('Error en el inicio de sesión');
        }
      });
    } else {
      console.error('Formulario no válido');
    }
  }

  toPerfil() {
    this.router.navigate(['/']);
  }

}
