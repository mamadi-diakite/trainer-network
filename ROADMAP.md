# Roadmap

This is the working plan for trainer-network. Stories are grouped by epic and tagged with the milestone they belong to.

## Scope

In scope:
- Sharing knowledge between trainers: pages, sections, discussions.
- Organizing content by profession and topic.
- Light collaboration: section-level edit rights, history, notifications.

Out of scope:
- Not tied to any certification scheme or legal framework.
- Not an LMS. No courses, no learners, no grades.
- No real user data. The demo runs on fictional accounts and content.

## Glossary

| Term | Meaning |
|------|---------|
| Profession | Top-level grouping (e.g. Occupational health, IT, Accounting, Sales) |
| Topic | A subject within a profession |
| Page | A document inside a topic, owned by its author |
| Section | A titled block of Markdown inside a page |
| Section editor | A trainer the author allowed to edit one section |
| Discussion | The message thread attached to a page |
| Subscribe | Follow a topic to hear about new pages |
| Follow | Follow a page to hear about changes |

## Roles

| Role | Can do |
|------|--------|
| Reader | Read pages, follow topics and pages |
| Trainer | Everything a Reader can, plus create pages, post in discussions, edit sections they are allowed to |
| Moderator | Everything a Trainer can, plus manage professions and topics, handle reports, hide content |

The page author is the admin of their own page, whatever their role.

## Milestones

| Milestone | Goal |
|-----------|------|
| M0 Foundation | Runnable app: login, demo data, read-only browsing |
| M1 Authoring | CI, page previews, trainers create and edit their own pages |
| M2 Collaboration | Section permissions, discussion, history |
| M3 Engagement | Subscriptions, notifications, dashboard, search |
| M4 Moderation and quality | Reports, tests, accessibility, docs |

---

## EPIC 1 — Project setup

| ID | Story | Milestone |
|----|-------|-----------|
| SETUP-1 | Repository with README, LICENSE (MIT), .gitignore, .editorconfig | M0 |
| SETUP-2 | .NET solution: Api, Domain, Application, Infrastructure and Tests projects | M0 |
| SETUP-3 | SQLite + EF Core, first migration, database created on startup in dev | M0 |
| SETUP-4 | Angular workspace with the main app and shared libraries | M0 |
| SETUP-5 | GitHub Actions: build and test both API and front end on each push | M1 |

## EPIC 2 — Accounts and authentication

| ID | Story | Milestone |
|----|-------|-----------|
| AUTH-1 | As a user, I log in with email and password and get a token | M0 |
| AUTH-2 | As a user, I log out, and I am sent back to login when my session expires | M0 |
| AUTH-3 | As a user, I change my password with current password, new password and confirmation | M0 |
| AUTH-4 | Routes and API endpoints are protected by role | M0 |
| AUTH-5 | Demo accounts are seeded for each role, with a documented demo password | M0 |
| AUTH-6 | As a user, I edit my profile: display name, profession, short bio | M1 |

Acceptance notes:
- Passwords are hashed. Never stored or logged in plain text.
- Demo emails use `@example.com`.
- No email flow for password reset. Out of scope for now.

## EPIC 3 — Login page and public overview

| ID | Story | Milestone |
|----|-------|-----------|
| LOGIN-1 | Split login page: form on the left, illustration on the right, stacks on mobile | M0 |
| LOGIN-2 | As a visitor, I see the list of professions, topics, page titles and section titles before logging in | M0 |
| LOGIN-3 | Illustration credit shown in the footer and in the README | M0 |

Acceptance notes:
- Visitors see titles only. Section content needs a login.

## EPIC 4 — Professions and topics

| ID | Story | Milestone |
|----|-------|-----------|
| TOPIC-1 | As a user, I browse professions and their topics | M0 |
| TOPIC-2 | As a user, I open a topic and see its pages as preview cards | M1 |
| TOPIC-3 | As a moderator, I create, rename, reorder and archive professions and topics | M4 |

## EPIC 5 — Pages and sections

| ID | Story | Milestone |
|----|-------|-----------|
| PAGE-1 | As a user, I read a page with its sections in order and a table of contents built from section titles | M0 |
| PAGE-2 | Section content is rendered from Markdown and sanitized | M0 |
| PAGE-3 | As a trainer, I create a page in a topic | M1 |
| PAGE-4 | As a page author, I add, edit and delete sections | M1 |
| PAGE-5 | As a page author, I reorder sections. New sections go at the end by default | M1 |
| PAGE-6 | Preview card shows title, author, last update and section titles | M1 |
| PAGE-7 | As a page author, I archive my page | M1 |

## EPIC 6 — Section permissions

| ID | Story | Milestone |
|----|-------|-----------|
| PERM-1 | As a page author, I let another trainer edit a section | M2 |
| PERM-2 | As a page author, I remove that right at any time | M2 |
| PERM-3 | As a page author, I grant a trainer all sections of my page in one action (stored as one right per section) | M2 |
| PERM-4 | As a trainer, I see the list of sections I am allowed to edit | M2 |
| PERM-5 | Rights are checked by the API, not only hidden in the UI. Covered by integration tests | M2 |

## EPIC 7 — Page discussion

| ID | Story | Milestone |
|----|-------|-----------|
| DISC-1 | As a trainer, I post a message in a page's discussion | M2 |
| DISC-2 | Messages show author and date, oldest first, with paging | M2 |
| DISC-3 | As a trainer, I edit or delete my own message | M2 |
| DISC-4 | As a trainer, I link my message to a specific section | M2 |
| DISC-5 | As a page author, I grant section edit rights straight from a message | M2 |

## EPIC 8 — Section history

| ID | Story | Milestone |
|----|-------|-----------|
| HIST-1 | Each save of a section creates a version with author and date | M2 |
| HIST-2 | As a user, I see the version list of a section | M2 |
| HIST-3 | As a user, I compare two versions side by side | M2 |
| HIST-4 | As a page author, I restore an older version | M2 |

## EPIC 9 — Subscriptions and notifications

| ID | Story | Milestone |
|----|-------|-----------|
| NOTIF-1 | As a user, I subscribe to a topic and get notified of new pages in it | M3 |
| NOTIF-2 | As a user, I follow a page and get notified when its sections change | M3 |
| NOTIF-3 | Notifications are created by the API when these events happen | M3 |
| NOTIF-4 | As a user, I see a bell with an unread count, open the list and mark items as read | M3 |
| NOTIF-5 | As a trainer, I get notified when I am given or lose edit rights on a section | M3 |

Acceptance notes:
- In-app only. No email.
- Polling is fine to start. Real-time push can come later.

## EPIC 10 — Dashboard

| ID | Story | Milestone |
|----|-------|-----------|
| DASH-1 | After login, I see all professions, topics and pages, with a filter by profession | M3 |
| DASH-2 | I see my subscribed topics and followed pages first | M3 |
| DASH-3 | I see recent activity: new pages, updated sections, new messages | M3 |

## EPIC 11 — Search

| ID | Story | Milestone |
|----|-------|-----------|
| SEARCH-1 | As a user, I search page titles, section titles and content (SQLite FTS5) | M3 |
| SEARCH-2 | I filter search results by profession and topic | M3 |

## EPIC 12 — Moderation

| ID | Story | Milestone |
|----|-------|-----------|
| MOD-1 | As a user, I report a message or a section | M4 |
| MOD-2 | As a moderator, I review reports and hide or restore content | M4 |
| MOD-3 | Moderator actions are logged | M4 |

## EPIC 13 — Quality and docs

| ID | Story | Milestone |
|----|-------|-----------|
| QA-1 | API integration tests for auth and permissions | M4 |
| QA-2 | Unit tests for key Angular components and services | M4 |
| QA-3 | End-to-end smoke test: login, open a page, post a message (Playwright) | M4 |
| QA-4 | Accessibility pass: keyboard navigation, labels, contrast | M4 |
| QA-5 | Docs: architecture overview, contributing guide, demo accounts | M4 |

---

## Later ideas

- Real-time notifications with SignalR
- Reactions and mentions in discussions
- French and English UI
- Export a page to PDF or Markdown
- Docker Compose for one-command start
