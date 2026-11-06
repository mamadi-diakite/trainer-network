import { AppHarness, demoSession } from '../../testing/app-harness';

describe('Login page', () => {
  let app: AppHarness;

  beforeEach(async () => {
    app = new AppHarness();
    await app.open('/login');
  });

  afterEach(() => app.http.verify());

  it('puts the form next to the illustration', () => {
    expect(app.text('h1')).toBe('Welcome back');
    const illustration = app.element.querySelector('section > div img');
    expect(illustration?.getAttribute('src')).toBe('illustrations/login.svg');
    expect(illustration?.getAttribute('alt')).toBe('');
  });

  it('asks for the missing fields without calling the API', async () => {
    await app.submit();

    expect(app.texts('tn-field-errors p')).toEqual([
      'Enter a valid email address.',
      'Enter your password.',
    ]);
  });

  it('shows the API message when the credentials are wrong', async () => {
    await app.type('email', 'claire.martin@example.com');
    await app.type('password', 'wrong');

    await app.submit();
    expect(app.button().disabled).toBe(true);
    const request = app.http.expectOne('/api/auth/login');
    expect(request.request.body).toEqual({ email: 'claire.martin@example.com', password: 'wrong' });
    request.flush(
      { title: 'Email or password is not correct.' },
      { status: 401, statusText: 'Unauthorized' },
    );
    await app.settle();

    expect(app.text('[role="alert"]')).toBe('Email or password is not correct.');
    expect(app.button().disabled).toBe(false);
  });

  it('shows a general message when the API fails', async () => {
    await app.type('email', 'claire.martin@example.com');
    await app.type('password', 'TestDemo123!');

    await app.submit();
    app.http.expectOne('/api/auth/login').flush(null, { status: 500, statusText: 'Server Error' });
    await app.settle();

    expect(app.text('[role="alert"]')).toBe('Something went wrong. Please try again.');
  });

  it('signs in and goes home', async () => {
    await app.type('email', 'claire.martin@example.com');
    await app.type('password', 'TestDemo123!');

    await app.submit();
    app.http.expectOne('/api/auth/login').flush(demoSession);
    await app.settle();
    app.http.expectOne('/api/catalog').flush([]);
    await app.settle();

    expect(app.router.url).toBe('/');
    expect(app.text('[data-testid="user-name"]')).toBe('Claire Martin');
  });
});
