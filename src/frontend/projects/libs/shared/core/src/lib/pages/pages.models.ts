export interface PageTopic {
  readonly id: string;
  readonly name: string;
  readonly slug: string;
  readonly professionName: string;
  readonly professionSlug: string;
}

export interface PageSection {
  readonly id: string;
  readonly title: string;
  /** Markdown source, rendered and sanitized by the client. */
  readonly content: string;
  readonly position: number;
}

export interface Page {
  readonly id: string;
  readonly title: string;
  readonly summary: string | null;
  readonly topic: PageTopic;
  readonly authorName: string;
  readonly updatedAtUtc: string;
  readonly sections: readonly PageSection[];
}
