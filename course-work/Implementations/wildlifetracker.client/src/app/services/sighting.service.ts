import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Sighting, CreateSightingDto, UpdateSightingDto } from '../models/sighting.model';
import { BaseService } from './base.service';

@Injectable({
    providedIn: 'root'
})
export class SightingService extends BaseService<Sighting, CreateSightingDto, UpdateSightingDto> {
    constructor(http: HttpClient) {
        super(http, '/sighting');
    }
} 