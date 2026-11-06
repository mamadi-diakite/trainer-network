import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { CatalogService } from '@trainer-network/shared/core';

/** Introduction and the public outline of professions, topics, pages and sections. */
@Component({
  selector: 'tn-home-page',
  imports: [RouterLink],
  template: `
    <div class="space-y-10">
      <section class="max-w-2xl space-y-4">
        <h1 class="text-3xl font-bold tracking-tight">Share what works in training</h1>
        <p class="text-lg text-slate-600">
          Trainer Network is a place where trainers share teaching methods, tips and lessons
          learned, organized by profession and topic.
        </p>
      </section>

      <section class="space-y-4" aria-labelledby="catalog-title">
        <h2 id="catalog-title" class="text-xl font-semibold">What you will find</h2>

        @let state = catalog();
        @if (state.status === 'loading') {
          <p class="text-slate-600" role="status">Loading the topics…</p>
        } @else if (state.status === 'ready') {
          <div class="grid gap-4 md:grid-cols-2">
            @for (profession of state.professions; track profession.id) {
              <article class="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
                <h3 class="text-lg font-semibold">{{ profession.name }}</h3>
                @for (topic of profession.topics; track topic.id) {
                  <div class="mt-4">
                    <h4>
                      <a [routerLink]="['/topics', topic.slug]" class="link">{{ topic.name }}</a>
                    </h4>
                    @if (topic.description) {
                      <p class="text-sm text-slate-600">{{ topic.description }}</p>
                    }
                    <ul class="mt-2 space-y-1 text-sm">
                      @for (page of topic.pages; track page.id) {
                        <li>
                          <a
                            [routerLink]="['/pages', page.id]"
                            class="font-medium hover:underline"
                            >{{ page.title }}</a
                          >
                          @if (page.sectionTitles.length > 0) {
                            <span class="text-slate-500" data-testid="section-titles">
                              · {{ page.sectionTitles.join(', ') }}</span
                            >
                          }
                        </li>
                      } @empty {
                        <li class="text-slate-500">No pages yet.</li>
                      }
                    </ul>
                  </div>
                }
              </article>
            } @empty {
              <p class="text-slate-600">No topics yet.</p>
            }
          </div>
        } @else {
          <p class="alert-error" role="alert">
            The topics could not be loaded. Please try again later.
          </p>
        }
      </section>
    </div>
  `,
})
export class HomePage {
  protected readonly catalog = toSignal(inject(CatalogService).load(), { requireSync: true });
}
