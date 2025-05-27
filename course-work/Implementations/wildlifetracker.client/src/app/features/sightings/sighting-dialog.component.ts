import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { Sighting, CreateSightingDto, UpdateSightingDto } from '../../models/sighting.model';
import { Animal } from '../../models/animal.model';
import { Habitat } from '../../models/habitat.model';
import { AnimalService } from '../../services/animal.service';
import { HabitatService } from '../../services/habitat.service';
import { Observable, map, startWith } from 'rxjs';

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
    MatButtonModule,
    MatSelectModule,
    MatAutocompleteModule
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
          <mat-label>Animal</mat-label>
          <input matInput
                 formControlName="animalSearch"
                 [matAutocomplete]="animalAuto"
                 placeholder="Search for an animal...">
          <mat-autocomplete #animalAuto="matAutocomplete" [displayWith]="displayAnimalFn">
            <mat-option *ngFor="let animal of filteredAnimals$ | async" [value]="animal">
              {{animal.name}} ({{animal.species}})
            </mat-option>
          </mat-autocomplete>
          <mat-error *ngIf="sightingForm.get('animalId')?.hasError('required')">Animal is required</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Habitat</mat-label>
          <input matInput
                 formControlName="habitatSearch"
                 [matAutocomplete]="habitatAuto"
                 placeholder="Search for a habitat...">
          <mat-autocomplete #habitatAuto="matAutocomplete" [displayWith]="displayHabitatFn">
            <mat-option *ngFor="let habitat of filteredHabitats$ | async" [value]="habitat">
              {{habitat.name}} ({{habitat.location}})
            </mat-option>
          </mat-autocomplete>
          <mat-error *ngIf="sightingForm.get('habitatId')?.hasError('required')">Habitat is required</mat-error>
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
export class SightingDialogComponent implements OnInit {
  sightingForm: FormGroup;
  animals: Animal[] = [];
  habitats: Habitat[] = [];
  filteredAnimals$: Observable<Animal[]>;
  filteredHabitats$: Observable<Habitat[]>;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<SightingDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { sighting?: Sighting },
    private animalService: AnimalService,
    private habitatService: HabitatService
  ) {
    this.sightingForm = this.fb.group({
      weatherConditions: [data.sighting?.weatherConditions || '', Validators.required],
      sightingDateTime: [data.sighting?.sightingDateTime || new Date().toISOString().slice(0, 16), Validators.required],
      animalId: [data.sighting?.animalId || null, Validators.required],
      animalSearch: [''],
      habitatId: [data.sighting?.habitatId || null, Validators.required],
      habitatSearch: [''],
      notes: [data.sighting?.notes || '']
    });

    // Initialize filtered observables
    this.filteredAnimals$ = this.sightingForm.get('animalSearch')!.valueChanges.pipe(
      startWith(''),
      map(value => this._filterAnimals(value))
    );

    this.filteredHabitats$ = this.sightingForm.get('habitatSearch')!.valueChanges.pipe(
      startWith(''),
      map(value => this._filterHabitats(value))
    );
  }

  ngOnInit() {
    // Load animals and habitats
    this.animalService.getAll().subscribe(animals => {
      this.animals = animals;
      if (this.data.sighting?.animalId) {
        const animal = animals.find(a => a.id === this.data.sighting?.animalId);
        if (animal) {
          this.sightingForm.get('animalSearch')?.setValue(animal);
        }
      }
    });

    this.habitatService.getAll().subscribe(habitats => {
      this.habitats = habitats;
      if (this.data.sighting?.habitatId) {
        const habitat = habitats.find(h => h.id === this.data.sighting?.habitatId);
        if (habitat) {
          this.sightingForm.get('habitatSearch')?.setValue(habitat);
        }
      }
    });

    // Update IDs when selections change
    this.sightingForm.get('animalSearch')?.valueChanges.subscribe(animal => {
      if (animal && typeof animal === 'object') {
        this.sightingForm.get('animalId')?.setValue(animal.id);
      }
    });

    this.sightingForm.get('habitatSearch')?.valueChanges.subscribe(habitat => {
      if (habitat && typeof habitat === 'object') {
        this.sightingForm.get('habitatId')?.setValue(habitat.id);
      }
    });
  }

  private _filterAnimals(value: string | Animal): Animal[] {
    const filterValue = typeof value === 'string' ? value.toLowerCase() : value.name.toLowerCase();
    return this.animals.filter(animal => 
      animal.name.toLowerCase().includes(filterValue) || 
      animal.species.toLowerCase().includes(filterValue)
    );
  }

  private _filterHabitats(value: string | Habitat): Habitat[] {
    const filterValue = typeof value === 'string' ? value.toLowerCase() : value.name.toLowerCase();
    return this.habitats.filter(habitat => 
      habitat.name.toLowerCase().includes(filterValue) || 
      habitat.location.toLowerCase().includes(filterValue)
    );
  }

  displayAnimalFn(animal: Animal): string {
    return animal ? `${animal.name} (${animal.species})` : '';
  }

  displayHabitatFn(habitat: Habitat): string {
    return habitat ? `${habitat.name} (${habitat.location})` : '';
  }

  onSubmit() {
    if (this.sightingForm.valid) {
      const formValue = this.sightingForm.value;
      // Only send the necessary fields to the parent component
      const result = {
        weatherConditions: formValue.weatherConditions,
        sightingDateTime: formValue.sightingDateTime,
        animalId: formValue.animalId,
        habitatId: formValue.habitatId,
        notes: formValue.notes
      };
      this.dialogRef.close(result);
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
} 