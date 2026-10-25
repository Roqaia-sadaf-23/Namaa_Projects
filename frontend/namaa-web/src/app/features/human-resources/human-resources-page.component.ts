import { Component } from '@angular/core';
import { TranslationKey } from '../../core/i18n/translations';
import { FeaturePlaceholderComponent } from '../../shared/components/feature-placeholder/feature-placeholder.component';

@Component({
  selector: 'app-human-resources-page',
  imports: [FeaturePlaceholderComponent],
  template: `
    <app-feature-placeholder
      sectionId="human-resources"
      titleKey="page.hr.title"
      descriptionKey="page.hr.description"
      [scopeKeys]="scopeKeys"
    />
  `
})
export class HumanResourcesPageComponent {
  protected readonly scopeKeys: readonly TranslationKey[] = [
    'scope.hr.employees',
    'scope.hr.attendance',
    'scope.hr.services'
  ];
}
