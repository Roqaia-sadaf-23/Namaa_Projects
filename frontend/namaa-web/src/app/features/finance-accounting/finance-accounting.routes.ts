import { Routes } from '@angular/router';

export const FINANCE_ACCOUNTING_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./finance-accounting-page.component').then(
        (component) => component.FinanceAccountingPageComponent
      )
  }
];
