import { Component, inject, input } from '@angular/core';
import { LanguageService } from '../../../core/i18n/language.service';
import { TranslationKey } from '../../../core/i18n/translations';

@Component({
  selector: 'app-feature-placeholder',
  templateUrl: './feature-placeholder.component.html',
  styleUrl: './feature-placeholder.component.scss'
})
export class FeaturePlaceholderComponent {
  protected readonly language = inject(LanguageService);

  readonly sectionId = input.required<string>();
  readonly titleKey = input.required<TranslationKey>();
  readonly descriptionKey = input.required<TranslationKey>();
  readonly scopeKeys = input.required<readonly TranslationKey[]>();
}
