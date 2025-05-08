import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';

@Component({
    selector: 'app-animals',
    standalone: true,
    imports: [CommonModule, MatCardModule],
    template: `
        <mat-card>
            <mat-card-header>
                <mat-card-title>Animals</mat-card-title>
            </mat-card-header>
            <mat-card-content>
                <p>Animals list will be displayed here</p>
            </mat-card-content>
        </mat-card>
    `
})
export class AnimalsComponent {} 