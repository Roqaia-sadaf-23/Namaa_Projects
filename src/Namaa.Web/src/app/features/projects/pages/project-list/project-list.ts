import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { ProjectApi } from '../../../../core/services/project-api';
import { Project, PROJECT_STATUS_LABELS } from '../../../../core/models/project.model';

@Component({
  selector: 'app-project-list',
  templateUrl: './project-list.html',
  styleUrl: './project-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectList {
  private readonly api = inject(ProjectApi);

  readonly projects = signal<Project[]>([]);
  readonly searchTerm = signal('');
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly statusLabels = PROJECT_STATUS_LABELS;

  readonly filteredProjects = computed(() => {
    const keyword = this.searchTerm().trim().toLocaleLowerCase();
    return this.projects().filter(p =>
      p.name.toLocaleLowerCase().includes(keyword) ||
      p.projectCode.toLocaleLowerCase().includes(keyword),
    );
  });

  constructor() {
    this.loadProjects();
  }

  loadProjects(): void {
    this.loading.set(true);
    this.error.set(null);
    this.api.getProjects().subscribe({
      next: projects => {
        this.projects.set(projects);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('تعذر تحميل المشاريع. تحققي من تشغيل الـ API وصلاحية الوصول.');
        this.loading.set(false);
      },
    });
  }

  updateSearch(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchTerm.set(target.value);
  }
}
