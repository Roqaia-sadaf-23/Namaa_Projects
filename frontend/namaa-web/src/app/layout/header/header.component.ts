import { Component, computed, inject, input, output } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslationKey } from '../../core/i18n/translations';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  protected readonly language = inject(LanguageService);
  private readonly router = inject(Router);
  private readonly activeRoute = inject(ActivatedRoute);

  readonly sidebarCollapsed = input(false);
  readonly mobileSidebarOpen = input(false);
  readonly isMobile = input(false);
  readonly menuToggle = output<void>();

  private readonly navigationEnd = toSignal(
    this.router.events.pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd)),
    { initialValue: null }
  );

  protected readonly pageTitle = computed(() => {
    this.navigationEnd();

    let route: ActivatedRoute | null = this.activeRoute;
    let titleKey: TranslationKey = 'nav.dashboard';

    while (route) {
      const routeTitleKey = route.snapshot.data['titleKey'];
      if (typeof routeTitleKey === 'string') {
        titleKey = routeTitleKey as TranslationKey;
      }
      route = route.firstChild;
    }

    return this.language.translate(titleKey);
  });

  protected menuLabel(): string {
    if (this.isMobile()) {
      return this.mobileSidebarOpen()
        ? this.language.translate('common.close')
        : this.language.translate('common.menu');
    }

    return this.sidebarCollapsed()
      ? this.language.translate('common.expandSidebar')
      : this.language.translate('common.collapseSidebar');
  }
}
