import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CashInModel } from '../../models/CashFlow/cashIn.model';
import { environment } from '../../environments/environment';
import { CashOutModel } from '../../models/CashFlow/cashOut.model';
import { PagedResult } from '../../models/paged-result.model';
import { CashInListModel } from '../../models/CashFlow/cashIn.model';
import { CashOutListModel } from '../../models/CashFlow/cashOut.model';

@Injectable({
    providedIn: 'root'
})
export class CashService {

    private apiUrl = `${environment.apiUrl}/cashFlow`

    constructor(
        private http: HttpClient
    ){}

    CreateCashIn(data: CashInModel): Observable<any>{
        return this.http.post<any>(
            `${this.apiUrl}/cashin`,
            data
        )
    }

    CreateCashOut(data: CashOutModel): Observable<any>{
        return this.http.post<any>(
            `${this.apiUrl}/cashout`,
            data
        )
    }

    GetAllCashIn(year?: number, month?: number, pageNumber = 1, pageSize = 10): Observable<PagedResult<CashInListModel>> {
        let params = new HttpParams();

        if (year != null) {
            params = params.set('year', year);
        }

        if (month != null) {
            params = params.set('month', month);
        }
        params = params.set('pageNumber', pageNumber).set('pageSize', pageSize);

        return this.http.get<PagedResult<CashInListModel>>(
            `${this.apiUrl}/cashin`,
            { params }
        );
    }

    GetAllCashOut(year?: number, month?: number, pageNumber = 1, pageSize = 10): Observable<PagedResult<CashOutListModel>> {
        let params = new HttpParams();

        if (year != null) {
            params = params.set('year', year);
        }

        if (month != null) {
            params = params.set('month', month);
        }
        params = params.set('pageNumber', pageNumber).set('pageSize', pageSize);

        return this.http.get<PagedResult<CashOutListModel>>(
            `${this.apiUrl}/cashout`,
            { params }
        );
    }
}
