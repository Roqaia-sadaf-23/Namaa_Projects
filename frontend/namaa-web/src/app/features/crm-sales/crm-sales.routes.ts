import { Routes } from '@angular/router';

export const CRM_SALES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./crm-sales-page.component').then((component) => component.CrmSalesPageComponent)
  }
];
