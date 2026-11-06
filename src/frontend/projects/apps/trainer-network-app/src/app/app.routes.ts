import { Routes } from '@angular/router';
import { authGuard, guestGuard } from '@trainer-network/shared/core';
import { ChangePasswordPage } from './auth/change-password-page';
import { LoginPage } from './auth/login-page';
import { HomePage } from './home/home-page';
import { PageReader } from './pages/page-reader';
import { TopicPage } from './topics/topic-page';

export const routes: Routes = [
  { path: '', component: HomePage, title: 'Trainer Network' },
  {
    path: 'login',
    component: LoginPage,
    canActivate: [guestGuard],
    title: 'Log in · Trainer Network',
  },
  {
    path: 'account/password',
    component: ChangePasswordPage,
    canActivate: [authGuard],
    title: 'Change password · Trainer Network',
  },
  { path: 'topics/:slug', component: TopicPage, title: 'Topic · Trainer Network' },
  {
    path: 'pages/:id',
    component: PageReader,
    canActivate: [authGuard],
    title: 'Page · Trainer Network',
  },
  { path: '**', redirectTo: '' },
];
