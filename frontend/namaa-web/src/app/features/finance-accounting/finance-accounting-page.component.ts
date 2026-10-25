import { Component } from '@angular/core';
import { TranslationKey } from '../../core/i18n/translations';
import { FeaturePlaceholderComponent } from '../../shared/components/feature-placeholder/feature-placeholder.component';

@Component({
  selector: 'app-finance-accounting-page',
  imports: [FeaturePlaceholderComponent],
  template: `
    <app-feature-placeholder
      sectionId="finance-accounting"
      titleKey="page.finance.title"
      descriptionKey="page.finance.description"
      [scopeKeys]="scopeKeys"
    />
  `
})
export class FinanceAccountingPageComponent {
  protected readonly scopeKeys: readonly TranslationKey[] = [
    'scope.finance.ledger',
    'scope.finance.budgets',
    'scope.finance.reports'
  ];
}
