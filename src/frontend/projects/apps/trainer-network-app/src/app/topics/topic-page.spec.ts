import { CatalogProfession } from '@trainer-network/shared/core';
import { AppHarness } from '../../testing/app-harness';

const professions: CatalogProfession[] = [
  {
    id: 'p-1',
    name: 'Information technology',
    slug: 'information-technology',
    topics: [
      {
        id: 't-1',
        name: 'Teaching Git',
        slug: 'teaching-git',
        description: 'Making version control click for beginners.',
        pages: [
          {
            id: 'page-1',
            title: 'Explaining branches with a paper timeline',
            sectionTitles: ['Why paper first', 'The setup'],
          },
          { id: 'page-2', title: 'Draft page', sectionTitles: [] },
        ],
      },
      { id: 't-2', name: 'Web basics', slug: 'web-basics', description: null, pages: [] },
    ],
  },
];

describe('Topic page', () => {
  let app: AppHarness;

  beforeEach(() => (app = new AppHarness()));

  afterEach(() => app.http.verify());

  async function openTopic(slug: string): Promise<void> {
    await app.open(`/topics/${slug}`);
    expect(app.text('[role="status"]')).toBe('Loading the topic…');
    app.http.expectOne('/api/catalog').flush(professions);
    await app.settle();
  }

  it('shows the topic with its pages to visitors, with a hint to log in', async () => {
    await openTopic('teaching-git');

    expect(app.text('nav[aria-label="Breadcrumb"]')).toBe('Home › Information technology');
    expect(app.text('h1')).toBe('Teaching Git');
    expect(app.text('h1 + p')).toBe('Making version control click for beginners.');
    expect(app.text('[data-testid="visitor-hint"]')).toBe('Log in to read the full pages.');
    expect(app.texts('[data-testid="pages"] li > a')).toEqual([
      'Explaining branches with a paper timeline',
      'Draft page',
    ]);
    expect(app.texts('[data-testid="pages"] li > p')).toEqual(['Why paper first · The setup']);
    expect(app.element.querySelector('[data-testid="pages"] li > a')?.getAttribute('href')).toBe(
      '/pages/page-1',
    );
  });

  it('shows an empty topic without the hint to signed-in users', async () => {
    app.signIn();

    await openTopic('web-basics');

    expect(app.text('h1')).toBe('Web basics');
    expect(app.element.querySelector('h1 + p')).toBeNull();
    expect(app.element.querySelector('[data-testid="visitor-hint"]')).toBeNull();
    expect(app.text('[data-testid="pages"] li')).toBe('No pages yet in this topic.');
  });

  it('says when the topic does not exist', async () => {
    await openTopic('unknown');

    expect(app.text('.alert-info')).toBe('This topic does not exist.');
  });

  it('explains when the catalog cannot be loaded', async () => {
    await app.open('/topics/teaching-git');
    app.http.expectOne('/api/catalog').flush(null, { status: 503, statusText: 'Unavailable' });
    await app.settle();

    expect(app.text('[role="alert"]')).toBe(
      'The topic could not be loaded. Please try again later.',
    );
  });
});
