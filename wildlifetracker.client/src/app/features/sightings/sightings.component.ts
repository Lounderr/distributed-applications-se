import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';

@Component({
    selector: 'app-sightings',
    standalone: true,
    imports: [CommonModule, MatCardModule],
    template: `
        <mat-card>
            <mat-card-header>
                <mat-card-title>Sightings</mat-card-title>
            </mat-card-header>
            <mat-card-content>
                <p>Sightings list will be displayed here</p>
            </mat-card-content>
        </mat-card>
    `
})
export class SightingsComponent {} 