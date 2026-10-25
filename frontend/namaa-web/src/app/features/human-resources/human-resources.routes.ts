import { Routes } from '@angular/router';

export const HUMAN_RESOURCES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./human-resources-page.component').then(
        (component) => component.HumanResourcesPageComponent
      )
  }
];
