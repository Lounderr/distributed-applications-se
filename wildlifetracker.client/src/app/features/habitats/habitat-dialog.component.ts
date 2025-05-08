import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Habitat, CreateHabitatDto, UpdateHabitatDto } from '../../models/habitat.model';

@Component({
  selector: 'app-habitat-dialog',
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
    <h2 mat-dialog-title>{{ data.habitat ? 'Edit' : 'Create' }} Habitat</h2>
    <form [formGroup]="habitatForm" (ngSubmit)="onSubmit()">
      <mat-dialog-content>
        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Name</mat-label>
          <input matInput formControlName="name" required>
          <mat-error *ngIf="habitatForm.get('name')?.hasError('required')">Name is required</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Location</mat-label>
          <input matInput formControlName="location" required>
          <mat-error *ngIf="habitatForm.get('location')?.hasError('required')">Location is required</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Size (km²)</mat-label>
          <input matInput type="number" formControlName="size">
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Climate</mat-label>
          <input matInput formControlName="climate" required>
          <mat-error *ngIf="habitatForm.get('climate')?.hasError('required')">Climate is required</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Average Temperature (°C)</mat-label>
          <input matInput type="number" formControlName="averageTemperature">
        </mat-form-field>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button type="button" (click)="onCancel()">Cancel</button>
        <button mat-raised-button color="primary" type="submit" [disabled]="habitatForm.invalid">
          {{ data.habitat ? 'Update' : 'Create' }}
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
export class HabitatDialogComponent {
  habitatForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<HabitatDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { habitat?: Habitat }
  ) {
    this.habitatForm = this.fb.group({
      name: [data.habitat?.name || '', Validators.required],
      location: [data.habitat?.location || '', Validators.required],
      size: [data.habitat?.size || null],
      climate: [data.habitat?.climate || '', Validators.required],
      averageTemperature: [data.habitat?.averageTemperature || null]
    });
  }

  onSubmit() {
    if (this.habitatForm.valid) {
      const formValue = this.habitatForm.value;
      this.dialogRef.close(formValue);
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
} 