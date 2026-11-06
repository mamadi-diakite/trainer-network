import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CatalogProfession, CatalogService, SessionStore } from '@trainer-network/shared/core';
import { combineLatest, map } from 'rxjs';

/** A topic of the public catalog, with its pages and their sections. */
@Component({
  selector: 'tn-topic-page',
  imports: [RouterLink],
  template: `
    @let state = view();
    @if (state.status === 'loading') {
      <p class="text-slate-600" role="status">Loading the topic…</p>
    } @else if (state.status === 'ready') {
      @if (state.found; as found) {
        <section class="space-y-6">
          <nav class="text-sm text-slate-500" aria-label="Breadcrumb">
            <a routerLink="/" class="link">Home</a> › {{ found.profession.name }}
          </nav>
          <div class="space-y-2">
            <h1 class="text-2xl font-bold tracking-tight">{{ found.topic.name }}</h1>
            @if (found.topic.description) {
              <p class="text-lg text-slate-600">{{ found.topic.description }}</p>
            }
          </div>

          @if (!user()) {
            <p class="alert-info" data-testid="visitor-hint">
              <a routerLink="/login" class="link">Log in</a> to read the full pages.
            </p>
          }

          <ul class="space-y-3" data-testid="pages">
            @for (page of found.topic.pages; track page.id) {
              <li class="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
                <a
                  [routerLink]="['/pages', page.id]"
                  class="text-lg font-semibold hover:underline"
                  >{{ page.title }}</a
                >
                @if (page.sectionTitles.length > 0) {
                  <p class="mt-1 text-sm text-slate-500">{{ page.sectionTitles.join(' · ') }}</p>
                }
              </li>
            } @empty {
              <li class="text-slate-600">No pages yet in this topic.</li>
            }
          </ul>
        </section>
      } @else {
        <p class="alert-info">This topic does not exist.</p>
      }
    } @else {
      <p class="alert-error" role="alert">The topic could not be loaded. Please try again later.</p>
    }
  `,
})
export class TopicPage {
  protected readonly user = inject(SessionStore).user;
  protected readonly view = toSignal(
    combineLatest([inject(CatalogService).load(), inject(ActivatedRoute).paramMap]).pipe(
      map(([catalog, params]) =>
        catalog.status === 'ready'
          ? { status: 'ready' as const, found: findTopic(catalog.professions, params.get('slug')) }
          : catalog,
      ),
    ),
    { requireSync: true },
  );
}

function findTopic(professions: readonly CatalogProfession[], slug: string | null) {
  for (const profession of professions) {
    const topic = profession.topics.find((candidate) => candidate.slug === slug);
    if (topic) {
      return { profession, topic };
    }
  }
  return null;
}
