import { Component } from '@angular/core';
import { TranslationKey } from '../../core/i18n/translations';
import { FeaturePlaceholderComponent } from '../../shared/components/feature-placeholder/feature-placeholder.component';

@Component({
  selector: 'app-crm-sales-page',
  imports: [FeaturePlaceholderComponent],
  template: `
    <app-feature-placeholder
      sectionId="crm-sales"
      titleKey="page.crm.title"
      descriptionKey="page.crm.description"
      [scopeKeys]="scopeKeys"
    />
  `
})
export class CrmSalesPageComponent {
  protected readonly scopeKeys: readonly TranslationKey[] = [
    'scope.crm.customers',
    'scope.crm.opportunities',
    'scope.crm.sales'
  ];
}
