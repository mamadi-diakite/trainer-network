import { TestBed } from '@angular/core/testing';
import { APP_ENVIRONMENT } from '@trainer-network/shared/environment';
import { AppHarness } from '../testing/app-harness';

describe('App', () => {
  let app: AppHarness;

  beforeEach(() => (app = new AppHarness()));

  afterEach(() => app.http.verify());

  it('uses the API behind the same origin', () => {
    expect(TestBed.inject(APP_ENVIRONMENT).apiBaseUrl).toBe('/api');
  });

  it('shows the API status, the login link and the illustration credit', async () => {
    await app.openHome();

    expect(app.text('[data-testid="api-status"]')).toBe('API online');
    expect(app.texts('header nav a')).toEqual(['Log in']);
    const credit = app.element.querySelector('footer a');
    expect(credit?.textContent).toBe('People illustrations by Storyset');
    expect(credit?.getAttribute('href')).toBe('https://storyset.com/people');
  });

  it('shows the signed-in user instead of the login link', async () => {
    app.signIn();

    await app.openHome();

    expect(app.text('[data-testid="user-name"]')).toBe('Claire Martin');
    expect(app.texts('header nav a')).toEqual([]);
  });

  it('sends unknown addresses back home', async () => {
    await app.open('/does-not-exist');
    app.http.expectOne('/api/catalog').flush([]);

    expect(app.router.url).toBe('/');
  });
});
