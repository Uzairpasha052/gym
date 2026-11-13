import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';

import { TodayAttendance } from '../../models/attendance/att_today.model';
import { AttendanceHistory } from '../../models/attendance/attHistory.model';
import { AttendanceResponse } from '../../models/attendance/attendance.model';
import { AdminAttendance } from '../../models/attendance/attAdmin.model';
import { UpdateAttendance } from '../../models/attendance/attUpdate.model';
import { MarkAttendance } from '../../models/attendance/markAttendance.model';
import { PagedResult } from '../../models/paged-result.model';


// @Service()
@Injectable({
    providedIn: 'root'
})
export class Attendance {

    private apiUrl = environment.apiUrl;

    constructor(
        private http: HttpClient
    ){}

    getTodayAttendance(pageNumber = 1, pageSize = 10): Observable<PagedResult<TodayAttendance>> {
        return this.http.get<PagedResult<TodayAttendance>>(
            `${this.apiUrl}/attendance/today`, { params: { pageNumber, pageSize } }
        );
    }
 
    saveAttendance(data: MarkAttendance[]): Observable<AttendanceResponse> {
        return this.http.post<AttendanceResponse>(
            `${this.apiUrl}/attendance/save`,
            data
        )
    }

    getAttendanceHistory(year?: number, month?: number, pageNumber = 1, pageSize = 10): Observable<PagedResult<AdminAttendance>> {
        let params = new HttpParams();

        if (year != null) {
            params = params.set('year', year);
        }

        if (month != null) {
            params = params.set('month', month);
        }
        params = params.set('pageNumber', pageNumber).set('pageSize', pageSize);

        return this.http.get<PagedResult<AdminAttendance>>(
            `${this.apiUrl}/attendance/history`,
            { params }
        );
    }

    // getMyAttendance(): Observable<AttendanceHistory[]>{
    //     return this.http.get<AttendanceHistory[]>(
    //         `${this.apiUrl}/attendance/my-attendance`
    //     );
    // }

    getPlayerAttendanceHistory(playerId: number): Observable<AttendanceHistory[]> {
        return this.http.get<AttendanceHistory[]>(
        `${this.apiUrl}/attendance/player/${playerId}`
        );
    }

    updateAttendance(
        attendanceId: number,
        attendance: UpdateAttendance
    ): Observable<AttendanceResponse> {

        return this.http.put<AttendanceResponse>(
            `${this.apiUrl}/attendance/update/${attendanceId}`,
            attendance
        );

    }
    
}
