import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink, RouterOutlet } from '@angular/router';
import { ApiHealthService, SessionStore } from '@trainer-network/shared/core';
import { Shell } from '@trainer-network/shared/ui';

@Component({
  selector: 'tn-root',
  imports: [RouterOutlet, RouterLink, Shell],
  template: `
    <tn-shell appName="Trainer Network" [apiStatus]="apiStatus()">
      <ng-container shellNav>
        @if (user(); as user) {
          <span class="text-slate-600" data-testid="user-name">{{ user.displayName }}</span>
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
  protected readonly user = inject(SessionStore).user;
  protected readonly apiStatus = toSignal(inject(ApiHealthService).check(), {
    initialValue: 'checking',
  });
}
