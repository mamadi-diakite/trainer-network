import { inject, Pipe, PipeTransform, SecurityContext } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { marked } from 'marked';

/**
 * Renders Markdown written by users to HTML safe for `[innerHTML]`: scripts, event handlers and
 * `javascript:` links are removed by the Angular sanitizer.
 */
@Pipe({ name: 'markdown' })
export class MarkdownPipe implements PipeTransform {
  private readonly sanitizer = inject(DomSanitizer);

  transform(source: string): string {
    const html = marked.parse(source, { async: false, gfm: true });
    return this.sanitizer.sanitize(SecurityContext.HTML, html)!;
  }
}
