export interface CurrentUser {
  readonly id: string;
  readonly email: string;
  readonly displayName: string;
  readonly role: string;
}

export interface LoginRequest {
  readonly email: string;
  readonly password: string;
}

export interface LoginResponse {
  readonly accessToken: string;
  readonly expiresAtUtc: string;
  readonly user: CurrentUser;
}
