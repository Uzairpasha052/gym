import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PagedResult } from '../../models/paged-result.model';
import { CreateMembershipRequest, MembershipPurchaseResponse, MembershipRecord, ExpiringMember } from '../../models/membership/membership.model';

@Injectable({ providedIn: 'root' })
export class MembershipService {
  private readonly apiUrl = `${environment.apiUrl}/membership`;

  constructor(private http: HttpClient) {}

  purchase(request: CreateMembershipRequest): Observable<MembershipPurchaseResponse> {
    return this.http.post<MembershipPurchaseResponse>(`${this.apiUrl}/purchase`, request);
  }

  getPlayerMemberships(playerId: number, pageNumber = 1, pageSize = 10): Observable<PagedResult<MembershipRecord>> {
    return this.http.get<PagedResult<MembershipRecord>>(`${this.apiUrl}/player/${playerId}`, { params: { pageNumber, pageSize } });
  }

  getExpiringMembers(days = 7, pageNumber = 1, pageSize = 10): Observable<PagedResult<ExpiringMember>> {
    return this.http.get<PagedResult<ExpiringMember>>(`${this.apiUrl}/expiring`, { params: { days, pageNumber, pageSize } });
  }
}
