import { ChangeDetectionStrategy, Component, computed, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

interface Project {
  id: number;
  name: string;
  client: string;
  status: 'Active' | 'Planning' | 'Completed';
}

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Dashboard {
  readonly projects = signal<Project[]>([
    {
      id: 1,
      name: 'تطوير موقع إلكتروني',
      client: 'شركة الأفق',
      status: 'Active',
    },
    {
      id: 2,
      name: 'تطبيق إدارة المهام',
      client: 'مؤسسة البناء',
      status: 'Planning',
    },
    {
      id: 3,
      name: 'نظام إدارة المخزون',
      client: 'شركة التقنية',
      status: 'Completed',
    },
  ]);

  readonly totalProjects = computed(() => this.projects().length);

  readonly activeProjects = computed(
    () => this.projects().filter((project) => project.status === 'Active').length,
  );

  readonly completedProjects = computed(
    () => this.projects().filter((project) => project.status === 'Completed').length,
  );
}
