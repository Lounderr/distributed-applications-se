import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';

export interface BaseEntity {
    id: number;
}

export interface PaginatedResponse<T> {
    data: T[];
    total: number;
    page: number;
    size: number;
}

@Injectable()
export abstract class BaseService<T extends BaseEntity, CreateDto, UpdateDto> {
    protected constructor(
        protected http: HttpClient,
        protected endpoint: string
    ) {}

    protected get baseUrl(): string {
        return `${environment.apiUrl}${this.endpoint}`;
    }

    getAll(page?: number, size?: number, filters?: string, fields?: string, orderBy?: string): Observable<T[]> {
        let params = new HttpParams();
        if (page) params = params.set('page', page.toString());
        if (size) params = params.set('size', size.toString());
        if (filters) params = params.set('filters', filters);
        if (fields) params = params.set('fields', fields);
        if (orderBy) params = params.set('orderBy', orderBy);

        return this.http.get<PaginatedResponse<T>>(this.baseUrl, { params }).pipe(
            map(response => response.data)
        );
    }

    getById(id: number): Observable<T> {
        return this.http.get<T>(`${this.baseUrl}/${id}`);
    }

    create(data: CreateDto): Observable<T> {
        return this.http.post<T>(this.baseUrl, data);
    }

    update(id: number, data: UpdateDto): Observable<T> {
        return this.http.put<T>(`${this.baseUrl}/${id}`, data);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
} 