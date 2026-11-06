import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { APP_ENVIRONMENT } from '@trainer-network/shared/environment';
import { map, Observable } from 'rxjs';
import { Page } from './pages.models';

@Injectable({ providedIn: 'root' })
export class PagesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(APP_ENVIRONMENT).apiBaseUrl}/pages`;

  /** The page with its sections sorted by position. */
  get(id: string): Observable<Page> {
    return this.http.get<Page>(`${this.baseUrl}/${id}`).pipe(
      map((page) => ({
        ...page,
        sections: [...page.sections].sort((a, b) => a.position - b.position),
      })),
    );
  }
}
