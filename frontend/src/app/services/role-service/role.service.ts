import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { RoleModel } from '../../models/role.model';

@Injectable({ providedIn: 'root' })
export class RoleService {
  private readonly url = `${environment.apiUrl}/roles`;

  constructor(private http: HttpClient) {}

  getRoles(): Observable<RoleModel[]> 
  {
     return this.http.get<RoleModel[]>(this.url);
  }
  createRole(name: string): Observable<RoleModel> 
  {
     return this.http.post<RoleModel>(this.url, { name }); 
  }
  updateRole(id: number, name: string): Observable<RoleModel>
  {
     return this.http.put<RoleModel>(`${this.url}/${id}`, { name }); 
  }
  deleteRole(id: number): Observable<void> 
  {
     return this.http.delete<void>(`${this.url}/${id}`); 
  }
}
