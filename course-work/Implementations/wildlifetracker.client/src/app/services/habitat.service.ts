import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Habitat, CreateHabitatDto, UpdateHabitatDto } from '../models/habitat.model';
import { BaseService } from './base.service';

@Injectable({
    providedIn: 'root'
})
export class HabitatService extends BaseService<Habitat, CreateHabitatDto, UpdateHabitatDto> {
    constructor(http: HttpClient) {
        super(http, '/habitat');
    }
} 