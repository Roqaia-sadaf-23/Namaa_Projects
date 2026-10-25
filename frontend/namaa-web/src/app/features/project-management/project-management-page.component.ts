import { Component } from '@angular/core';
import { TranslationKey } from '../../core/i18n/translations';
import { FeaturePlaceholderComponent } from '../../shared/components/feature-placeholder/feature-placeholder.component';

@Component({
  selector: 'app-project-management-page',
  imports: [FeaturePlaceholderComponent],
  template: `
    <app-feature-placeholder
      sectionId="project-management"
      titleKey="page.projects.title"
      descriptionKey="page.projects.description"
      [scopeKeys]="scopeKeys"
    />
  `
})
export class ProjectManagementPageComponent {
  protected readonly scopeKeys: readonly TranslationKey[] = [
    'scope.projects.portfolio',
    'scope.projects.planning',
    'scope.projects.governance'
  ];
}
