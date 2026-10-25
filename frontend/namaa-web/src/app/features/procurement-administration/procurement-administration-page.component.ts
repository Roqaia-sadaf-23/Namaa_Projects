import { Component } from '@angular/core';
import { TranslationKey } from '../../core/i18n/translations';
import { FeaturePlaceholderComponent } from '../../shared/components/feature-placeholder/feature-placeholder.component';

@Component({
  selector: 'app-procurement-administration-page',
  imports: [FeaturePlaceholderComponent],
  template: `
    <app-feature-placeholder
      sectionId="procurement-administration"
      titleKey="page.procurement.title"
      descriptionKey="page.procurement.description"
      [scopeKeys]="scopeKeys"
    />
  `
})
export class ProcurementAdministrationPageComponent {
  protected readonly scopeKeys: readonly TranslationKey[] = [
    'scope.procurement.requests',
    'scope.procurement.suppliers',
    'scope.procurement.admin'
  ];
}
