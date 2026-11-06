import { TestBed } from '@angular/core/testing';
import { vi } from 'vitest';
import { MarkdownPipe } from './markdown.pipe';

describe('MarkdownPipe', () => {
  let pipe: MarkdownPipe;

  beforeEach(() => {
    vi.spyOn(console, 'warn').mockImplementation(() => {});
    pipe = TestBed.runInInjectionContext(() => new MarkdownPipe());
  });

  afterEach(() => vi.restoreAllMocks());

  it('renders lists, emphasis and inline code', () => {
    const html = pipe.transform('- *who* walks here?\n- the `main` strip');

    expect(html).toBe(
      '<ul>&#10;<li><em>who</em> walks here?</li>&#10;<li>the <code>main</code> strip</li>&#10;</ul>&#10;',
    );
  });

  it('removes scripts, event handlers and javascript links', () => {
    const html = pipe.transform(
      'Hi <script>alert(1)</script><img src="x.png" onerror="alert(2)"> [link](javascript:alert(3))',
    );

    expect(html).not.toContain('<script');
    expect(html).not.toContain('onerror');
    expect(html).not.toContain('href="javascript:');
    expect(html).toContain('<img src="x.png">');
  });

  it('renders nothing for an empty section', () => {
    expect(pipe.transform('')).toBe('');
  });
});
