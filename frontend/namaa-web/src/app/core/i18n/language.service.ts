import { DOCUMENT } from '@angular/common';
import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { AR_TRANSLATIONS, EN_TRANSLATIONS, TranslationKey } from './translations';

export type AppLanguage = 'ar' | 'en';

@Injectable({ providedIn: 'root' })
export class LanguageService {
  private readonly document = inject(DOCUMENT);
  private readonly selectedLanguage = signal<AppLanguage>('ar');

  readonly language = this.selectedLanguage.asReadonly();
  readonly direction = computed(() => (this.language() === 'ar' ? 'rtl' : 'ltr'));
  readonly isRtl = computed(() => this.direction() === 'rtl');

  private readonly dictionary = computed<Record<TranslationKey, string>>(() =>
    this.language() === 'ar' ? AR_TRANSLATIONS : EN_TRANSLATIONS
  );

  constructor() {
    effect(() => {
      const root = this.document.documentElement;
      root.lang = this.language();
      root.dir = this.direction();
    });
  }

  translate(key: TranslationKey): string {
    return this.dictionary()[key];
  }

  toggle(): void {
    this.selectedLanguage.update((language) => (language === 'ar' ? 'en' : 'ar'));
  }
}
