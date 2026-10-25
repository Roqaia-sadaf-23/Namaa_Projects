import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./layout/main-layout/main-layout.component').then(
        (component) => component.MainLayoutComponent
      ),
    children: [
      {
        path: 'dashboard',
        data: { titleKey: 'nav.dashboard' },
        loadChildren: () =>
          import('./features/dashboard/dashboard.routes').then((routes) => routes.DASHBOARD_ROUTES)
      },
      {
        path: 'projects',
        data: { titleKey: 'nav.projects' },
        loadChildren: () =>
          import('./features/project-management/project-management.routes').then(
            (routes) => routes.PROJECT_MANAGEMENT_ROUTES
          )
      },
      {
        path: 'crm',
        data: { titleKey: 'nav.crm' },
        loadChildren: () =>
          import('./features/crm-sales/crm-sales.routes').then((routes) => routes.CRM_SALES_ROUTES)
      },
      {
        path: 'finance',
        data: { titleKey: 'nav.finance' },
        loadChildren: () =>
          import('./features/finance-accounting/finance-accounting.routes').then(
            (routes) => routes.FINANCE_ACCOUNTING_ROUTES
          )
      },
      {
        path: 'hr',
        data: { titleKey: 'nav.hr' },
        loadChildren: () =>
          import('./features/human-resources/human-resources.routes').then(
            (routes) => routes.HUMAN_RESOURCES_ROUTES
          )
      },
      {
        path: 'procurement',
        data: { titleKey: 'nav.procurement' },
        loadChildren: () =>
          import('./features/procurement-administration/procurement-administration.routes').then(
            (routes) => routes.PROCUREMENT_ADMINISTRATION_ROUTES
          )
      },
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
