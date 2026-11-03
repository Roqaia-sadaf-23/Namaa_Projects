// Matches Namaa.Application.Projects.Dtos.ProjectDto and ProjectStatus enum.
export type ProjectStatus = 0 | 1 | 2 | 3 | 4;

export interface Project {
  id: number;
  customerId: number;
  sourceQuotationId: number | null;
  projectManagerId: number | null;
  projectCode: string;
  name: string;
  description: string | null;
  location: string | null;
  startDate: string | null;
  endDate: string | null;
  actualCompletionDate: string | null;
  status: ProjectStatus;
  progressPercentage: number;
  budgetAmount: number;
  createdByUserId: number;
  createdAt: string;
  updatedAt: string | null;
  rowVersion: string;
}

export const PROJECT_STATUS_LABELS: Record<ProjectStatus, string> = {
  0: 'مخطط',
  1: 'قيد التنفيذ',
  2: 'متوقف مؤقتًا',
  3: 'مكتمل',
  4: 'ملغي',
};
