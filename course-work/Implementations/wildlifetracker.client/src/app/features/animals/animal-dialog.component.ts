import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Animal, CreateAnimalDto, UpdateAnimalDto } from '../../models/animal.model';

@Component({
  selector: 'app-animal-dialog',
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
    <h2 mat-dialog-title>{{ data.animal ? 'Edit' : 'Create' }} Animal</h2>
    <form [formGroup]="animalForm" (ngSubmit)="onSubmit()">
      <mat-dialog-content>
        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Name</mat-label>
          <input matInput formControlName="name" required>
          <mat-error *ngIf="animalForm.get('name')?.hasError('required')">Name is required</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Species</mat-label>
          <input matInput formControlName="species" required>
          <mat-error *ngIf="animalForm.get('species')?.hasError('required')">Species is required</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Age</mat-label>
          <input matInput type="number" formControlName="age" required>
          <mat-error *ngIf="animalForm.get('age')?.hasError('required')">Age is required</mat-error>
          <mat-error *ngIf="animalForm.get('age')?.hasError('min')">Age must be positive</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Weight (kg)</mat-label>
          <input matInput type="number" formControlName="weight" required>
          <mat-error *ngIf="animalForm.get('weight')?.hasError('required')">Weight is required</mat-error>
          <mat-error *ngIf="animalForm.get('weight')?.hasError('min')">Weight must be positive</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Height (cm)</mat-label>
          <input matInput type="number" formControlName="height" required>
          <mat-error *ngIf="animalForm.get('height')?.hasError('required')">Height is required</mat-error>
          <mat-error *ngIf="animalForm.get('height')?.hasError('min')">Height must be positive</mat-error>
        </mat-form-field>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button type="button" (click)="onCancel()">Cancel</button>
        <button mat-raised-button color="primary" type="submit" [disabled]="animalForm.invalid">
          {{ data.animal ? 'Update' : 'Create' }}
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
export class AnimalDialogComponent {
  animalForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<AnimalDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { animal?: Animal }
  ) {
    this.animalForm = this.fb.group({
      name: [data.animal?.name || '', Validators.required],
      species: [data.animal?.species || '', Validators.required],
      age: [data.animal?.age || null, [Validators.required, Validators.min(0)]],
      weight: [data.animal?.weight || null, [Validators.required, Validators.min(0)]],
      height: [data.animal?.height || null, [Validators.required, Validators.min(0)]]
    });
  }

  onSubmit() {
    if (this.animalForm.valid) {
      const formValue = this.animalForm.value;
      this.dialogRef.close(formValue);
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
} 