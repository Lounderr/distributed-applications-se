import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Animal, CreateAnimalDto, UpdateAnimalDto } from '../models/animal.model';
import { BaseService } from './base.service';

@Injectable({
    providedIn: 'root'
})
export class AnimalService extends BaseService<Animal, CreateAnimalDto, UpdateAnimalDto> {
    private readonly API_URL = '/api/v1/animal';

    constructor(http: HttpClient) {
        super(http, '/api/v1/animal');
    }

    uploadImage(id: number, file: File): Observable<void> {
        const formData = new FormData();
        formData.append('file', file);
        return this.http.put<void>(`${this.API_URL}/${id}/image`, formData);
    }

    deleteImage(id: number): Observable<void> {
        return this.http.delete<void>(`${this.API_URL}/${id}/image`);
    }
} 