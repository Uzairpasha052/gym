import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuditLogModel } from '../../models/audit-log.model';
import { PagedResult } from '../../models/paged-result.model';

@Injectable({ providedIn: 'root' })
export class AuditLogService {
  private readonly apiUrl = `${environment.apiUrl}/auditLog`;

  constructor(private http: HttpClient) {}

  getLogs(pageNumber = 1, pageSize = 10): Observable<PagedResult<AuditLogModel>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);
    return this.http.get<PagedResult<AuditLogModel>>(this.apiUrl, { params });
  }
}
