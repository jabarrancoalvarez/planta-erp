import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface AuditEntryDto {
  id: string;
  userId: string;
  empresaId: string;
  action: string;
  entityType: string;
  entityId: string;
  oldValues: string | null;
  newValues: string | null;
  timestamp: string;
  ipAddress: string | null;
}

export interface AuditPageDto {
  data: AuditEntryDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface AuditFilter {
  entityType?: string;
  userId?: string;
  from?: string;
  to?: string;
  page?: number;
  pageSize?: number;
}

@Injectable({ providedIn: 'root' })
export class AuditoriaService {
  private api = inject(ApiService);

  list(filter: AuditFilter = {}): Observable<AuditPageDto> {
    return this.api.get<AuditPageDto>('/auditoria', {
      entityType: filter.entityType,
      userId: filter.userId,
      from: filter.from,
      to: filter.to,
      page: filter.page ?? 1,
      pageSize: filter.pageSize ?? 50,
    });
  }
}
