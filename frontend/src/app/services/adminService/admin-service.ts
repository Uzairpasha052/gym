import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

// @Service()
@Injectable({
    providedIn: 'root'
})
export class AdminService {

    getToken(): string | null{
        const storedUser = localStorage.getItem('user') || sessionStorage.getItem('user');

        if (!storedUser) return null;

        try {
            return JSON.parse(storedUser)?.token ?? null;
        } catch {
            return null;
        }
    }

    getUser() {
        const token = this.getToken();

        if (!token) return null;

        try{
            return jwtDecode<Record<string, unknown>>(token);
        }catch{
            return null
        }

    }

    getRole(): string | null {
        const user = this.getUser();
        const roleClaim = user?.['role']
            ?? user?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
        const role = Array.isArray(roleClaim) ? roleClaim[0] : roleClaim;

        return typeof role === 'string' ? role : null;
    }

    isAdmin(): boolean {
        return this.getRole() === 'Admin';
    }
}
