import { DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PageSection, PagesService, withLoadState } from '@trainer-network/shared/core';
import { MarkdownPipe } from '@trainer-network/shared/ui';
import { switchMap } from 'rxjs';

/** A page with its table of contents and its sections rendered from Markdown. */
@Component({
  selector: 'tn-page-reader',
  imports: [DatePipe, RouterLink, MarkdownPipe],
  template: `
    @let state = page();
    @if (state.status === 'loading') {
      <p class="text-slate-600" role="status">Loading the page…</p>
    } @else if (state.status === 'ready') {
      @let current = state.value;
      <article class="space-y-8">
        <nav class="text-sm text-slate-500" aria-label="Breadcrumb">
          <a routerLink="/" class="link">Home</a> › {{ current.topic.professionName }} ›
          <a [routerLink]="['/topics', current.topic.slug]" class="link">{{
            current.topic.name
          }}</a>
        </nav>

        <header class="space-y-2">
          <h1 class="text-3xl font-bold tracking-tight">{{ current.title }}</h1>
          @if (current.summary) {
            <p class="text-lg text-slate-600" data-testid="summary">{{ current.summary }}</p>
          }
          <p class="text-sm text-slate-500" data-testid="byline">
            By {{ current.authorName }} · Updated {{ current.updatedAtUtc | date: 'mediumDate' }}
          </p>
        </header>

        <div class="grid gap-8 lg:grid-cols-[14rem_1fr]">
          <nav class="self-start lg:sticky lg:top-8" aria-labelledby="toc-title">
            <h2 id="toc-title" class="text-sm font-semibold tracking-wide text-slate-500 uppercase">
              Contents
            </h2>
            <ol class="mt-2 space-y-1 text-sm">
              @for (section of current.sections; track section.id) {
                <li>
                  <a [routerLink]="[]" [fragment]="anchor(section)" class="link">{{
                    section.title
                  }}</a>
                </li>
              }
            </ol>
          </nav>

          <div class="space-y-10">
            @for (section of current.sections; track section.id) {
              <section [id]="anchor(section)" class="scroll-mt-8">
                <h2 class="text-xl font-semibold">{{ section.title }}</h2>
                <div class="markdown mt-3" [innerHTML]="section.content | markdown"></div>
              </section>
            } @empty {
              <p class="text-slate-600">This page has no sections yet.</p>
            }
          </div>
        </div>
      </article>
    } @else if (state.status === 'missing') {
      <p class="alert-info">This page does not exist. It may have been removed.</p>
    } @else {
      <p class="alert-error" role="alert">The page could not be loaded. Please try again later.</p>
    }
  `,
})
export class PageReader {
  private readonly pages = inject(PagesService);

  protected readonly page = toSignal(
    inject(ActivatedRoute).paramMap.pipe(
      switchMap((params) => withLoadState(this.pages.get(params.get('id')!))),
    ),
    { requireSync: true },
  );

  protected anchor(section: PageSection): string {
    return `section-${section.position}`;
  }
}
