import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Sighting, CreateSightingDto, UpdateSightingDto } from '../../models/sighting.model';

@Component({
  selector: 'app-sighting-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  template: `
    <h2 mat-dialog-title>{{ data.sighting ? 'Edit' : 'Create' }} Sighting</h2>
    <form [formGroup]="sightingForm" (ngSubmit)="onSubmit()">
      <mat-dialog-content>
        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Weather Conditions</mat-label>
          <input matInput formControlName="weatherConditions" required>
          <mat-error *ngIf="sightingForm.get('weatherConditions')?.hasError('required')">Weather conditions are required</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Date & Time</mat-label>
          <input matInput type="datetime-local" formControlName="sightingDateTime" required>
          <mat-error *ngIf="sightingForm.get('sightingDateTime')?.hasError('required')">Date and time are required</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Animal ID</mat-label>
          <input matInput type="number" formControlName="animalId" required>
          <mat-error *ngIf="sightingForm.get('animalId')?.hasError('required')">Animal ID is required</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Habitat ID</mat-label>
          <input matInput type="number" formControlName="habitatId" required>
          <mat-error *ngIf="sightingForm.get('habitatId')?.hasError('required')">Habitat ID is required</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Observer ID</mat-label>
          <input matInput formControlName="observerId" required>
          <mat-error *ngIf="sightingForm.get('observerId')?.hasError('required')">Observer ID is required</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Notes</mat-label>
          <textarea matInput formControlName="notes" rows="3"></textarea>
        </mat-form-field>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button type="button" (click)="onCancel()">Cancel</button>
        <button mat-raised-button color="primary" type="submit" [disabled]="sightingForm.invalid">
          {{ data.sighting ? 'Update' : 'Create' }}
        </button>
      </mat-dialog-actions>
    </form>
  `,
  styles: [`
    .full-width {
      width: 100%;
      margin-bottom: 15px;
    }
    mat-dialog-content {
      min-width: 400px;
    }
  `]
})
export class SightingDialogComponent {
  sightingForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<SightingDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { sighting?: Sighting }
  ) {
    this.sightingForm = this.fb.group({
      weatherConditions: [data.sighting?.weatherConditions || '', Validators.required],
      sightingDateTime: [data.sighting?.sightingDateTime || new Date().toISOString().slice(0, 16), Validators.required],
      animalId: [data.sighting?.animalId || null, Validators.required],
      habitatId: [data.sighting?.habitatId || null, Validators.required],
      observerId: [data.sighting?.observerId || '', Validators.required],
      notes: [data.sighting?.notes || '']
    });
  }

  onSubmit() {
    if (this.sightingForm.valid) {
      const formValue = this.sightingForm.value;
      this.dialogRef.close(formValue);
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
} 