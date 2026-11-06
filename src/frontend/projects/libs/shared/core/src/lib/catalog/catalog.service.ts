import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { APP_ENVIRONMENT } from '@trainer-network/shared/environment';
import { catchError, map, Observable, of, startWith } from 'rxjs';

export interface CatalogPage {
  readonly id: string;
  readonly title: string;
  readonly sectionTitles: readonly string[];
}

export interface CatalogTopic {
  readonly id: string;
  readonly name: string;
  readonly slug: string;
  readonly description: string | null;
  readonly pages: readonly CatalogPage[];
}

export interface CatalogProfession {
  readonly id: string;
  readonly name: string;
  readonly slug: string;
  readonly topics: readonly CatalogTopic[];
}

export type CatalogState =
  | { readonly status: 'loading' }
  | { readonly status: 'ready'; readonly professions: readonly CatalogProfession[] }
  | { readonly status: 'error' };

/** Public outline of the content, readable before signing in. */
@Injectable({ providedIn: 'root' })
export class CatalogService {
  private readonly http = inject(HttpClient);
  private readonly url = `${inject(APP_ENVIRONMENT).apiBaseUrl}/catalog`;

  /** Starts with `loading`, then emits the outline or `error`. Never errors. */
  load(): Observable<CatalogState> {
    return this.http.get<CatalogProfession[]>(this.url).pipe(
      map((professions): CatalogState => ({ status: 'ready', professions })),
      catchError(() => of<CatalogState>({ status: 'error' })),
      startWith<CatalogState>({ status: 'loading' }),
    );
  }
}
