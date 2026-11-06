import { CatalogProfession } from '@trainer-network/shared/core';
import { AppHarness } from '../../testing/app-harness';

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
        description: 'Getting adults to speak from day one.',
        pages: [
          {
            id: 'page-1',
            title: 'Role plays that work',
            sectionTitles: ['Set-up', 'Debrief'],
          },
          { id: 'page-2', title: 'Warm-ups', sectionTitles: [] },
        ],
      },
      { id: 't-2', name: 'Grammar', slug: 'grammar', description: null, pages: [] },
    ],
  },
];

describe('Home page', () => {
  let app: AppHarness;

  beforeEach(() => (app = new AppHarness()));

  afterEach(() => app.http.verify());

  it('introduces the platform and shows the outline while loading', async () => {
    await app.open('/');

    expect(app.text('h1')).toBe('Share what works in training');
    expect(app.text('[role="status"]')).toBe('Loading the topics…');

    app.http.expectOne('/api/catalog').flush(professions);
    await app.settle();

    expect(app.element.querySelector('[role="status"]')).toBeNull();
    expect(app.texts('article h3')).toEqual(['Language trainers']);
    expect(app.texts('article h4')).toEqual(['Speaking practice', 'Grammar']);
    expect(app.texts('article h4 + p')).toEqual(['Getting adults to speak from day one.']);
    expect(app.texts('article li > a')).toEqual(['Role plays that work', 'Warm-ups']);
    expect(app.element.querySelector('article li > a')?.getAttribute('href')).toBe('/pages/page-1');
    expect(app.element.querySelector('article h4 a')?.getAttribute('href')).toBe(
      '/topics/speaking-practice',
    );
    expect(app.texts('[data-testid="section-titles"]')).toEqual(['· Set-up, Debrief']);
    expect(app.texts('article li.text-slate-500')).toEqual(['No pages yet.']);
  });

  it('says when there are no topics yet', async () => {
    await app.openHome([]);

    expect(app.text('#catalog-title + div p')).toBe('No topics yet.');
  });

  it('explains when the outline cannot be loaded', async () => {
    await app.open('/');
    app.http.expectOne('/api/catalog').flush(null, { status: 503, statusText: 'Unavailable' });
    await app.settle();

    expect(app.text('[role="alert"]')).toBe(
      'The topics could not be loaded. Please try again later.',
    );
  });
});
