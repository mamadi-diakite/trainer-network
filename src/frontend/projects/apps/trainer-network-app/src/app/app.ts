import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { ApiHealthService, AuthService, SessionStore } from '@trainer-network/shared/core';
import { Shell } from '@trainer-network/shared/ui';

@Component({
  selector: 'tn-root',
  imports: [RouterOutlet, RouterLink, Shell],
  template: `
    <tn-shell appName="Trainer Network" [apiStatus]="apiStatus()">
      <ng-container shellNav>
        @if (user(); as user) {
          <span class="text-slate-600" data-testid="user-name">{{ user.displayName }}</span>
          <a routerLink="/account/password" class="link">Change password</a>
          <button type="button" class="link" (click)="logOut()">Log out</button>
        } @else {
          <a routerLink="/login" class="link">Log in</a>
        }
      </ng-container>
      <a shellCredit href="https://storyset.com/people" class="hover:underline"
        >People illustrations by Storyset</a
      >
      <router-outlet />
    </tn-shell>
  `,
})
export class App {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly user = inject(SessionStore).user;
  protected readonly apiStatus = toSignal(inject(ApiHealthService).check(), {
    initialValue: 'checking',
  });

  protected logOut(): void {
    this.auth.logout();
    void this.router.navigateByUrl('/login');
  }
}
