import { AppHarness } from '../../testing/app-harness';

describe('Change password page', () => {
  let app: AppHarness;

  beforeEach(async () => {
    app = new AppHarness();
    app.signIn();
    await app.open('/account/password');
    await app.type('currentPassword', 'TestDemo123!');
    await app.type('newPassword', 'NewPass456');
    await app.type('confirmPassword', 'NewPass456');
  });

  afterEach(() => app.http.verify());

  it('shows the API messages next to each field', async () => {
    await app.submit();
    const request = app.http.expectOne('/api/auth/change-password');
    expect(request.request.headers.get('Authorization')).toBe('Bearer token-1');
    expect(request.request.body).toEqual({
      currentPassword: 'TestDemo123!',
      newPassword: 'NewPass456',
      confirmPassword: 'NewPass456',
    });
    request.flush(
      { errors: { currentPassword: ['Current password is not correct.'] } },
      { status: 400, statusText: 'Bad Request' },
    );
    await app.settle();

    expect(app.texts('tn-field-errors p')).toEqual(['Current password is not correct.']);
    expect(app.element.querySelector('[role="status"]')).toBeNull();
  });

  it('confirms the change and clears the form', async () => {
    await app.submit();
    app.http
      .expectOne('/api/auth/change-password')
      .flush(null, { status: 204, statusText: 'No Content' });
    await app.settle();

    expect(app.text('[role="status"]')).toBe('Your password has been changed.');
    expect(app.element.querySelector<HTMLInputElement>('#currentPassword')!.value).toBe('');
    expect(app.texts('tn-field-errors p')).toEqual([]);
  });

  it('asks for every field when empty', async () => {
    await app.type('currentPassword', '');
    await app.type('newPassword', '');
    await app.type('confirmPassword', '');

    await app.submit();

    expect(app.texts('tn-field-errors p')).toEqual([
      'Enter your current password.',
      'Choose a new password.',
      'Confirm your new password.',
    ]);
  });

  it('shows a general message when the API fails', async () => {
    await app.submit();
    app.http
      .expectOne('/api/auth/change-password')
      .flush(null, { status: 500, statusText: 'Server Error' });
    await app.settle();

    expect(app.text('[role="alert"]')).toBe('Something went wrong. Please try again.');
  });

  it('sends the user to login when the session is refused', async () => {
    await app.submit();
    app.http
      .expectOne('/api/auth/change-password')
      .flush(null, { status: 401, statusText: 'Unauthorized' });
    await app.settle();

    expect(app.router.url).toBe('/login?expired=true');
    expect(app.text('[role="status"]')).toBe('Your session has expired. Please log in again.');
    expect(app.texts('header nav a')).toEqual(['Log in']);
  });
});
