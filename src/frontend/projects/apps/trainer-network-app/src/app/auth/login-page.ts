import { Component, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService, FormSubmission, safeReturnUrl } from '@trainer-network/shared/core';
import { FieldErrors } from '@trainer-network/shared/ui';

/** Form on the left, illustration on the right; the illustration goes under the form on small screens. */
@Component({
  selector: 'tn-login-page',
  imports: [ReactiveFormsModule, FieldErrors],
  template: `
    <section
      class="grid overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm md:grid-cols-2"
    >
      <form class="space-y-6 p-8 md:p-12" [formGroup]="form" (ngSubmit)="logIn()" novalidate>
        <div class="space-y-2">
          <h1 class="text-2xl font-bold tracking-tight">Welcome back</h1>
          <p class="text-slate-600">Log in to read and improve the pages trainers share.</p>
        </div>

        @if (expired) {
          <p class="alert-info" role="status">Your session has expired. Please log in again.</p>
        }
        @if (submission.errors().form; as message) {
          <p class="alert-error" role="alert">{{ message }}</p>
        }

        <div>
          <label class="form-label" for="email">Email</label>
          <input
            id="email"
            class="form-input"
            type="email"
            formControlName="email"
            autocomplete="email"
          />
          <tn-field-errors [messages]="submission.messagesFor('email')" />
        </div>

        <div>
          <label class="form-label" for="password">Password</label>
          <input
            id="password"
            class="form-input"
            type="password"
            formControlName="password"
            autocomplete="current-password"
          />
          <tn-field-errors [messages]="submission.messagesFor('password')" />
        </div>

        <button class="button-primary" type="submit" [disabled]="submission.pending()">
          Log in
        </button>
      </form>

      <div class="flex items-center justify-center bg-teal-50 p-8">
        <img
          src="illustrations/login.svg"
          alt=""
          class="w-full max-w-sm"
          width="500"
          height="500"
        />
      </div>
    </section>
  `,
})
export class LoginPage {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly query = inject(ActivatedRoute).snapshot.queryParamMap;

  protected readonly expired = this.query.has('expired');
  protected readonly form = inject(NonNullableFormBuilder).group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });
  protected readonly submission = new FormSubmission(this.form, {
    email: 'Enter a valid email address.',
    password: 'Enter your password.',
  });

  protected logIn(): void {
    this.submission.submit(
      () => this.auth.login(this.form.getRawValue()),
      () => void this.router.navigateByUrl(safeReturnUrl(this.query.get('returnUrl'))),
    );
  }
}
