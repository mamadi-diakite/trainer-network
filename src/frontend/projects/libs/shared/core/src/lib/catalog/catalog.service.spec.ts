import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideAppEnvironment } from '@trainer-network/shared/environment';
import { CatalogProfession, CatalogService, CatalogState } from './catalog.service';

const professions: CatalogProfession[] = [
  {
    id: 'p-1',
    name: 'Language trainers',
    slug: 'language-trainers',
    topics: [
      {
        id: 't-1',
        name: 'Speaking practice',
        slug: 'speaking-practice',
        description: null,
        pages: [{ id: 'page-1', title: 'Role plays that work', sectionTitles: ['Set-up'] }],
      },
    ],
  },
];

describe('CatalogService', () => {
  let service: CatalogService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideAppEnvironment({ apiBaseUrl: '/api' }),
      ],
    });
    service = TestBed.inject(CatalogService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  function collect(): CatalogState[] {
    const states: CatalogState[] = [];
    service.load().subscribe((state) => states.push(state));
    return states;
  }

  it('loads the public outline', () => {
    const states = collect();

    http.expectOne('/api/catalog').flush(professions);

    expect(states).toEqual([{ status: 'loading' }, { status: 'ready', professions }]);
  });

  it('reports an error when the request fails', () => {
    const states = collect();

    http.expectOne('/api/catalog').flush(null, { status: 503, statusText: 'Unavailable' });

    expect(states).toEqual([{ status: 'loading' }, { status: 'error' }]);
  });
});
