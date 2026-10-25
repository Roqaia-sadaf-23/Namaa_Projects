import { Routes } from '@angular/router';

export const PROJECT_MANAGEMENT_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./project-management-page.component').then(
        (component) => component.ProjectManagementPageComponent
      )
  }
];
