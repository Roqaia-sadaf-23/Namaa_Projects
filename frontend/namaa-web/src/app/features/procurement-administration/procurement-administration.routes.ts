import { Routes } from '@angular/router';

export const PROCUREMENT_ADMINISTRATION_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./procurement-administration-page.component').then(
        (component) => component.ProcurementAdministrationPageComponent
      )
  }
];
