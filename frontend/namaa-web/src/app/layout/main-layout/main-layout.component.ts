import { afterNextRender, Component, DestroyRef, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../header/header.component';
import { SidebarComponent } from '../sidebar/sidebar.component';

@Component({
  selector: 'app-main-layout',
  imports: [HeaderComponent, RouterOutlet, SidebarComponent],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss'
})
export class MainLayoutComponent {
  private readonly destroyRef = inject(DestroyRef);

  protected readonly sidebarCollapsed = signal(false);
  protected readonly mobileSidebarOpen = signal(false);
  protected readonly isMobile = signal(false);

  constructor() {
    afterNextRender(() => {
      const mediaQuery = window.matchMedia('(max-width: 60rem)');
      const updateViewport = (event: MediaQueryList | MediaQueryListEvent): void => {
        this.isMobile.set(event.matches);
        if (!event.matches) {
          this.mobileSidebarOpen.set(false);
        }
      };

      updateViewport(mediaQuery);
      mediaQuery.addEventListener('change', updateViewport);
      this.destroyRef.onDestroy(() => mediaQuery.removeEventListener('change', updateViewport));
    });
  }

  protected toggleSidebar(): void {
    if (this.isMobile()) {
      this.mobileSidebarOpen.update((isOpen) => !isOpen);
      return;
    }

    this.sidebarCollapsed.update((isCollapsed) => !isCollapsed);
  }

  protected closeMobileSidebar(): void {
    this.mobileSidebarOpen.set(false);
  }
}
