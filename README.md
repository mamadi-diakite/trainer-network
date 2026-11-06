# trainer-network

A web platform where trainers share teaching methods, tips and lessons learned.

Content is organized by profession, then by topic, then by page. Each page is split into sections. The page author decides who can edit each section, and every page has a discussion thread where trainers propose ideas before they land in the page.

## Who it is for

Independent trainers and small training teams who want a shared, living knowledge base instead of scattered notes and email threads.

## Stack

- Front end: Angular workspace (app + libraries)
- Back end: ASP.NET Core Web API (C#)
- Database: SQLite with EF Core

## Status

Early development (milestone M0). What works today:

- Before logging in, see the whole catalog: professions, topics, page titles and section titles. Open a topic to see its pages.
- Log in, log out, change your password. Sessions expire after 60 minutes, and you are sent back to the login page.
- Read a page: breadcrumb, author, last update, a table of contents built from the section titles, and the sections in order. Section content is written in Markdown and cleaned before it is shown.

Writing pages, section permissions and discussions are planned (see [ROADMAP.md](ROADMAP.md)).

## Getting started

### Prerequisites

- [.NET SDK 10.0.403](https://dotnet.microsoft.com/download) or a later 10.0 feature band
- [Node.js 24](https://nodejs.org/) with npm 11

### Run the API

```bash
cd src/backend
dotnet run --project src/TrainerNetwork.Api --launch-profile http
```

The API listens on http://localhost:5080. In Development it creates the SQLite database in `src/backend/src/TrainerNetwork.Api/App_Data/`, applies the migrations, and adds the demo data below when the database is empty.

The access tokens are signed with a key that is never stored in the repository. In Development, a random key is generated at startup, so you need to log in again after each restart. In any other environment, set a base64 key of at least 32 bytes:

```bash
export Jwt__SigningKey="$(openssl rand -base64 32)"
```

### Run the web app

```bash
cd src/frontend
npm install
npm start
```

Open http://localhost:4200. Calls to `/api` are sent to the API on port 5080.

### Demo data

All demo accounts use the password `TestDemo123!`. They are fictional and only exist in Development.

| Name | Email | Role |
|------|-------|------|
| Demo Moderator | moderator@example.com | Moderator |
| Claire Martin | claire.martin@example.com | Trainer |
| Karim Benali | karim.benali@example.com | Trainer |
| Sophie Leroy | sophie.leroy@example.com | Trainer |
| Demo Reader | reader@example.com | Reader |

The demo content covers 4 professions (occupational health and safety, information technology, accounting, sales), 8 topics and 6 pages written by the three trainers. Some topics have no page yet, to show the empty state.

To start again from an empty demo, stop the API and delete `src/backend/src/TrainerNetwork.Api/App_Data/`.

### Run the tests

Back end (with coverage, migrations and generated code excluded):

```bash
cd src/backend
dotnet test --settings coverage.runsettings --collect "XPlat Code Coverage"
```

Front end (Vitest, the run fails below 100% coverage):

```bash
cd src/frontend
npm run test:ci
```

## Project layout

```
src/backend/    ASP.NET Core solution: Domain, Application.Contracts, Application, Infrastructure, Api, and their test projects
src/frontend/   Angular workspace: projects/apps/trainer-network-app and the shared libraries in projects/libs/shared
```

## Credits

The login page illustration comes from Storyset:
<a href="https://storyset.com/people">People illustrations by Storyset</a>

## License

[MIT](LICENSE)
