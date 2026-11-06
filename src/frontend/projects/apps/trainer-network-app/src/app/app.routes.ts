import { Routes } from '@angular/router';
import { LoginPage } from './auth/login-page';
import { HomePage } from './home/home-page';

export const routes: Routes = [
  { path: '', component: HomePage, title: 'Trainer Network' },
  { path: 'login', component: LoginPage, title: 'Log in · Trainer Network' },
  { path: '**', redirectTo: '' },
];
