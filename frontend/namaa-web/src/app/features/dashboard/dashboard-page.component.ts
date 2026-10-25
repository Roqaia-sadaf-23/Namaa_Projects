import { Component } from '@angular/core';
import { TranslationKey } from '../../core/i18n/translations';
import { FeaturePlaceholderComponent } from '../../shared/components/feature-placeholder/feature-placeholder.component';

@Component({
  selector: 'app-dashboard-page',
  imports: [FeaturePlaceholderComponent],
  template: `
    <app-feature-placeholder
      sectionId="dashboard"
      titleKey="page.dashboard.title"
      descriptionKey="page.dashboard.description"
      [scopeKeys]="scopeKeys"
    />
  `
})
export class DashboardPageComponent {
  protected readonly scopeKeys: readonly TranslationKey[] = [
    'scope.dashboard.overview',
    'scope.dashboard.modules',
    'scope.dashboard.readiness'
  ];
}
