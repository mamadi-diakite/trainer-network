import { Page } from '@trainer-network/shared/core';
import { vi } from 'vitest';
import { AppHarness } from '../../testing/app-harness';

const page: Page = {
  id: 'page-1',
  title: 'Explaining branches with a paper timeline',
  summary: 'No terminal for the first twenty minutes.',
  topic: {
    id: 't-1',
    name: 'Teaching Git',
    slug: 'teaching-git',
    professionName: 'Information technology',
    professionSlug: 'information-technology',
  },
  authorName: 'Karim Benali',
  updatedAtUtc: '2026-11-03T10:00:00Z',
  sections: [
    {
      id: 's-2',
      title: 'The setup',
      content: 'Each note is a commit. The strip is `main`.',
      position: 2,
    },
    {
      id: 's-1',
      title: 'Why paper first',
      content: '- *Picture* first\n- commands next <script>alert(1)</script>',
      position: 1,
    },
  ],
};

describe('Page reader', () => {
  let app: AppHarness;

  beforeEach(() => {
    app = new AppHarness();
    vi.spyOn(console, 'warn').mockImplementation(() => {});
  });

  afterEach(() => {
    app.http.verify();
    vi.restoreAllMocks();
  });

  async function openPage(response: Page = page): Promise<void> {
    app.signIn();
    await app.open('/pages/page-1');
    expect(app.text('[role="status"]')).toBe('Loading the page…');
    app.http.expectOne('/api/pages/page-1').flush(response);
    await app.settle();
  }

  it('is only for signed-in users', async () => {
    await app.open('/pages/page-1');

    expect(app.router.url).toBe('/login?returnUrl=%2Fpages%2Fpage-1');
  });

  it('shows the page header and a table of contents in section order', async () => {
    await openPage();

    expect(app.text('nav[aria-label="Breadcrumb"]')!.replace(/\s+/g, ' ')).toBe(
      'Home › Information technology › Teaching Git',
    );
    expect(app.text('h1')).toBe('Explaining branches with a paper timeline');
    expect(app.text('[data-testid="summary"]')).toBe('No terminal for the first twenty minutes.');
    expect(app.text('[data-testid="byline"]')).toBe('By Karim Benali · Updated Nov 3, 2026');
    expect(app.texts('nav[aria-labelledby="toc-title"] a')).toEqual([
      'Why paper first',
      'The setup',
    ]);
    expect(
      app.element.querySelector('nav[aria-labelledby="toc-title"] a')?.getAttribute('href'),
    ).toBe('/pages/page-1#section-1');
  });

  it('renders the sections from Markdown without unsafe content', async () => {
    await openPage();

    const sections = app.element.querySelectorAll('article section');
    expect([...sections].map((section) => section.id)).toEqual(['section-1', 'section-2']);
    const first = sections[0].querySelector('.markdown')!;
    expect(first.querySelector('li em')?.textContent).toBe('Picture');
    expect(first.querySelector('script')).toBeNull();
    expect(sections[1].querySelector('.markdown code')?.textContent).toBe('main');
  });

  it('shows a page without summary nor sections', async () => {
    await openPage({ ...page, summary: null, sections: [] });

    expect(app.element.querySelector('[data-testid="summary"]')).toBeNull();
    expect(app.text('article section, article .space-y-10 p')).toBe(
      'This page has no sections yet.',
    );
  });

  it('says when the page does not exist', async () => {
    app.signIn();
    await app.open('/pages/page-1');
    app.http.expectOne('/api/pages/page-1').flush(null, { status: 404, statusText: 'Not Found' });
    await app.settle();

    expect(app.text('.alert-info')).toBe('This page does not exist. It may have been removed.');
  });

  it('explains when the page cannot be loaded', async () => {
    app.signIn();
    await app.open('/pages/page-1');
    app.http.expectOne('/api/pages/page-1').flush(null, { status: 500, statusText: 'Error' });
    await app.settle();

    expect(app.text('[role="alert"]')).toBe(
      'The page could not be loaded. Please try again later.',
    );
  });
});
