import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { SidebarMenuCreateRequest, SidebarMenuItem, SidebarMenuUpdateRequest } from '../../models/sidebar-menu.model';

@Injectable({ providedIn: 'root' })
export class SidebarService {
  private readonly apiUrl = `${environment.apiUrl}/sidebar`;

  constructor(private http: HttpClient) {}

  getParentMenus(): Observable<SidebarMenuItem[]> {
    return this.http.get<SidebarMenuItem[]>(`${this.apiUrl}/parents`);
  }

  getChildMenus(parentId: number): Observable<SidebarMenuItem[]> {
    return this.http.get<SidebarMenuItem[]>(`${this.apiUrl}/children/${parentId}`);
  }

  getMenu(id: number): Observable<SidebarMenuItem> {
    return this.http.get<SidebarMenuItem>(`${this.apiUrl}/${id}`);
  }

  createMenu(request: SidebarMenuCreateRequest): Observable<{ id: number; message: string }> {
    return this.http.post<{ id: number; message: string }>(this.apiUrl, request);
  }

  updateMenu(id: number, request: SidebarMenuUpdateRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}`, request);
  }
}
