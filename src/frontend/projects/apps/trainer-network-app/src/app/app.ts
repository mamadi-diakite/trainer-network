import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterOutlet } from '@angular/router';
import { ApiHealthService } from '@trainer-network/shared/core';
import { Shell } from '@trainer-network/shared/ui';

@Component({
  selector: 'tn-root',
  imports: [RouterOutlet, Shell],
  template: `
    <tn-shell appName="Trainer Network" [apiStatus]="apiStatus()">
      <router-outlet />
    </tn-shell>
  `,
})
export class App {
  protected readonly apiStatus = toSignal(inject(ApiHealthService).check(), {
    initialValue: 'checking',
  });
}
