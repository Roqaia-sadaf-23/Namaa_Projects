import { Component, inject, input, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslationKey } from '../../core/i18n/translations';

interface NavigationItem {
  readonly labelKey: TranslationKey;
  readonly path: string;
  readonly iconPath: string;
}

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  protected readonly language = inject(LanguageService);

  readonly collapsed = input(false);
  readonly mobileOpen = input(false);
  readonly navigationSelected = output<void>();
  readonly closeRequested = output<void>();

  protected readonly navigationItems: readonly NavigationItem[] = [
    {
      labelKey: 'nav.dashboard',
      path: '/dashboard',
      iconPath: 'M4 4h6v6H4V4Zm10 0h6v6h-6V4ZM4 14h6v6H4v-6Zm10 0h6v6h-6v-6Z'
    },
    {
      labelKey: 'nav.projects',
      path: '/projects',
      iconPath: 'M4 7.5h16v11H4v-11Zm5-3h6l1.5 3h-9L9 4.5Z'
    },
    {
      labelKey: 'nav.crm',
      path: '/crm',
      iconPath: 'M16 20v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2m7-10a4 4 0 1 0 0-8 4 4 0 0 0 0 8Zm8-3v6m3-3h-6'
    },
    {
      labelKey: 'nav.finance',
      path: '/finance',
      iconPath: 'M3 10h18M5 10v8m4-8v8m6-8v8m4-8v8M3 21h18M12 3l9 4H3l9-4Z'
    },
    {
      labelKey: 'nav.hr',
      path: '/hr',
      iconPath: 'M8 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8Zm8 1a3 3 0 1 0 0-6m-8 8c-4 0-6 2-6 5v2h12v-2c0-3-2-5-6-5Zm8 1c3 0 5 1.5 5 4v2h-4'
    },
    {
      labelKey: 'nav.procurement',
      path: '/procurement',
      iconPath: 'M3 4h2l2.2 10.2a2 2 0 0 0 2 1.6h7.9a2 2 0 0 0 2-1.6L20.5 8H6m4 12a1 1 0 1 1-2 0 1 1 0 0 1 2 0Zm9 0a1 1 0 1 1-2 0 1 1 0 0 1 2 0Z'
    }
  ];
}
