import { Injectable } from '@angular/core';
import { CanActivate, CanActivateChild, Router } from '@angular/router';
import { TokenService } from '../services/token.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private tokenService: TokenService, private router: Router) {}

  private checkAccess(): boolean {
    const token = this.tokenService.getToken();

    if (!token || this.tokenService.isTokenExpired()) {
      this.router.navigate(['/login']);
      return false;
    }

    return true;
  }

  canActivate(): boolean {
    return this.checkAccess();
  }

  canActivateChild(): boolean {
    return this.checkAccess();
  }
}
