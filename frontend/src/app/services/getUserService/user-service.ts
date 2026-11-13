import { Injectable, Service } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserModel } from '../../models/user.model';
import { UserUpdateModel } from '../../models/UserUpdate.model';
import { PagedResult } from '../../models/paged-result.model';

import { HttpHeaders } from '@angular/common/http';

// @Service()
@Injectable({
    providedIn: 'root'
})
export class UserService {

    private apiUrl = environment.apiUrl;

    constructor(
        private http: HttpClient
    ){}

    getUsers(pageNumber = 1, pageSize = 5): Observable<PagedResult<UserModel>>{
        return this.http.get<PagedResult<UserModel>>(
            `${this.apiUrl}/users/allUsers`,
            { params: { pageNumber, pageSize } }
        )
    }

    //useless
    // getUser(id: number): Observable<UserModel>{
    //     return this.http.get<UserModel>(
    //         `${this.apiUrl}/users/allUsers/${id}`
    //     )
    // }

    deleteUser(id: number): Observable<void>{
        return this.http.delete<void>(
            `${this.apiUrl}/users/deleteUser/${id}`
        );
    }

    updateUser(id: number, user: UserUpdateModel): Observable<UserUpdateModel>{
        return this.http.put<UserUpdateModel>(
            `${this.apiUrl}/users/updateUser/${id}`,
            user
        )
    }
}
