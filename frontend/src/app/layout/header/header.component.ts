import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MATERIAL_IMPORTS } from '../../shared/material/material';
import { TokenService } from '../../core/services/token.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-header',
  imports: [CommonModule, RouterModule, ...MATERIAL_IMPORTS],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  isMobileMenuOpen: boolean = false;
  nombreUsuario: string = 'Usuario';

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  constructor(private tokenService: TokenService, private authService: AuthService){}
  
    ngOnInit() {
      const nombre = this.tokenService.getUsername();
      if (nombre) {
        this.nombreUsuario = nombre;
      }
      console.log('Nombre Usuario: ', this.nombreUsuario);
    }
}
