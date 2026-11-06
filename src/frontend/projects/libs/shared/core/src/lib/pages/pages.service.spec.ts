import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideAppEnvironment } from '@trainer-network/shared/environment';
import { firstValueFrom } from 'rxjs';
import { Page } from './pages.models';
import { PagesService } from './pages.service';

describe('PagesService', () => {
  it('reads a page and sorts its sections by position', async () => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideAppEnvironment({ apiBaseUrl: '/api' }),
      ],
    });
    const http = TestBed.inject(HttpTestingController);
    const page = firstValueFrom(TestBed.inject(PagesService).get('p-1'));

    http.expectOne({ method: 'GET', url: '/api/pages/p-1' }).flush({
      id: 'p-1',
      sections: [
        { id: 's-2', title: 'Second', content: '', position: 2 },
        { id: 's-1', title: 'First', content: '', position: 1 },
      ],
    } as unknown as Page);

    expect((await page).sections.map((section) => section.title)).toEqual(['First', 'Second']);
    http.verify();
  });
});
