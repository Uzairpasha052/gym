import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginModel } from '../../models/login.model';
import { Router } from '@angular/router';
import { registerModel } from '../../models/register.model';

// @Service()
@Injectable({
    providedIn: 'root'
})
export class Auth {

    private apiUrl = environment.apiUrl;

    constructor(
        private http: HttpClient,
        private router: Router
    ){}

    login(user: LoginModel): Observable<any>{
        return this.http.post(
            `${this.apiUrl}/auth/login`,
            user
        );
    }

    register(user: registerModel): Observable<any>{
        return this.http.post(
            `${this.apiUrl}/auth/register`,
            user
        )
    }

    logout(): void{
        localStorage.removeItem('user')
        sessionStorage.removeItem('user')
        this.router.navigate(['/']);
    }

}
