import { Routes } from '@angular/router';
import { MainLayout } from './layout/main-layout/main-layout';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    //loadComponent: () => import('./layout/main-layout/main-layout').then((m) => m.MainLayout),
  },
];
