import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTable } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { RouterModule } from '@angular/router';
import { SightingService } from '../../services/sighting.service';
import { AnimalService } from '../../services/animal.service';
import { HabitatService } from '../../services/habitat.service';
import { Sighting } from '../../models/sighting.model';
import { Animal } from '../../models/animal.model';
import { Habitat } from '../../models/habitat.model';
import { SightingDialogComponent } from './sighting-dialog.component';
import { debounceTime, distinctUntilChanged, map, startWith } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { ConfirmDialogComponent } from '../../components/confirm-dialog/confirm-dialog.component';

@Component({
    selector: 'app-sightings',
    standalone: true,
    imports: [
        CommonModule,
        MatTableModule,
        MatPaginatorModule,
        MatSortModule,
        MatInputModule,
        MatFormFieldModule,
        MatButtonModule,
        MatIconModule,
        MatDialogModule,
        MatSnackBarModule,
        MatSelectModule,
        MatAutocompleteModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule
    ],
    template: `
        <div class="container">
            <div class="header">
                <h1>Sightings</h1>
                <button mat-raised-button color="primary" (click)="openCreateDialog()">
                    <mat-icon>add</mat-icon>
                    Add Sighting
                </button>
            </div>

            <div class="filters-container">
                <!-- Global Search -->
                <mat-form-field class="search-field">
                    <mat-label>Global Search</mat-label>
                    <input matInput [formControl]="globalSearchControl" placeholder="Search across all fields...">
                    <mat-icon matSuffix>search</mat-icon>
                </mat-form-field>

                <!-- Property Filters -->
                <div class="property-filters">
                    <mat-form-field>
                        <mat-label>Weather Conditions</mat-label>
                        <input matInput [formControl]="weatherControl" placeholder="Filter by weather">
                    </mat-form-field>

                    <mat-form-field>
                        <mat-label>Animal</mat-label>
                        <input matInput
                               [formControl]="animalSearchControl"
                               [matAutocomplete]="animalAuto"
                               placeholder="Search for an animal...">
                        <mat-autocomplete #animalAuto="matAutocomplete" [displayWith]="displayAnimalFn">
                            <mat-option *ngFor="let animal of filteredAnimals$ | async" [value]="animal">
                                {{animal.name}} ({{animal.species}})
                            </mat-option>
                        </mat-autocomplete>
                    </mat-form-field>

                    <mat-form-field>
                        <mat-label>Habitat</mat-label>
                        <input matInput
                               [formControl]="habitatSearchControl"
                               [matAutocomplete]="habitatAuto"
                               placeholder="Search for a habitat...">
                        <mat-autocomplete #habitatAuto="matAutocomplete" [displayWith]="displayHabitatFn">
                            <mat-option *ngFor="let habitat of filteredHabitats$ | async" [value]="habitat">
                                {{habitat.name}} ({{habitat.location}})
                            </mat-option>
                        </mat-autocomplete>
                    </mat-form-field>

                    <mat-form-field>
                        <mat-label>Date Range</mat-label>
                        <div class="range-inputs">
                            <input matInput type="datetime-local" [formControl]="minDateControl" placeholder="From">
                            <span>-</span>
                            <input matInput type="datetime-local" [formControl]="maxDateControl" placeholder="To">
                        </div>
                    </mat-form-field>

                    <button mat-button color="primary" (click)="clearFilters()">
                        <mat-icon>clear</mat-icon>
                        Clear Filters
                    </button>
                </div>
            </div>

            <div class="mat-elevation-z8">
                <table mat-table [dataSource]="dataSource" matSort>
                    <!-- Weather Column -->
                    <ng-container matColumnDef="weatherConditions">
                        <th mat-header-cell *matHeaderCellDef mat-sort-header>Weather</th>
                        <td mat-cell *matCellDef="let row">{{row.weatherConditions}}</td>
                    </ng-container>

                    <!-- Date Column -->
                    <ng-container matColumnDef="sightingDateTime">
                        <th mat-header-cell *matHeaderCellDef mat-sort-header>Date & Time</th>
                        <td mat-cell *matCellDef="let row">{{row.sightingDateTime | date:'medium'}}</td>
                    </ng-container>

                    <!-- Animal Column -->
                    <ng-container matColumnDef="animalId">
                        <th mat-header-cell *matHeaderCellDef mat-sort-header>Animal</th>
                        <td mat-cell *matCellDef="let row">{{getAnimalName(row.animalId)}}</td>
                    </ng-container>

                    <!-- Habitat Column -->
                    <ng-container matColumnDef="habitatId">
                        <th mat-header-cell *matHeaderCellDef mat-sort-header>Habitat</th>
                        <td mat-cell *matCellDef="let row">{{getHabitatName(row.habitatId)}}</td>
                    </ng-container>

                    <!-- Notes Column -->
                    <ng-container matColumnDef="notes">
                        <th mat-header-cell *matHeaderCellDef mat-sort-header>Notes</th>
                        <td mat-cell *matCellDef="let row">{{row.notes}}</td>
                    </ng-container>

                    <!-- Actions Column -->
                    <ng-container matColumnDef="actions">
                        <th mat-header-cell *matHeaderCellDef>Actions</th>
                        <td mat-cell *matCellDef="let row">
                            <button mat-icon-button color="primary" (click)="openEditDialog(row)">
                                <mat-icon>edit</mat-icon>
                            </button>
                            <button mat-icon-button color="warn" (click)="deleteSighting(row)">
                                <mat-icon>delete</mat-icon>
                            </button>
                        </td>
                    </ng-container>

                    <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
                    <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>

                    <!-- Row shown when there is no matching data. -->
                    <tr class="mat-row" *matNoDataRow>
                        <td class="mat-cell" colspan="6">No data matching the filters</td>
                    </tr>
                </table>

                <mat-paginator [pageSizeOptions]="[5, 10, 25, 100]" aria-label="Select page of sightings"></mat-paginator>
            </div>
        </div>
    `,
    styles: [`
        .container {
            padding: 20px;
        }
        .header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
        }
        .filters-container {
            margin-bottom: 20px;
        }
        .search-field {
            width: 100%;
            margin-bottom: 15px;
        }
        .property-filters {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin-bottom: 15px;
        }
        .range-inputs {
            display: flex;
            align-items: center;
            gap: 8px;
        }
        .range-inputs input {
            width: 150px;
        }
        .mat-elevation-z8 {
            overflow: auto;
        }
        table {
            width: 100%;
        }
        .mat-column-actions {
            width: 120px;
            text-align: center;
        }
        mat-form-field {
            width: 100%;
        }
    `]
})
export class SightingsComponent implements OnInit, AfterViewInit {
    displayedColumns: string[] = ['weatherConditions', 'sightingDateTime', 'animalId', 'habitatId', 'notes', 'actions'];
    dataSource: MatTableDataSource<Sighting>;
    sightings: Sighting[] = [];
    animals: Animal[] = [];
    habitats: Habitat[] = [];
    filteredAnimals$: Observable<Animal[]>;
    filteredHabitats$: Observable<Habitat[]>;

    filtersForm: FormGroup<{
        weather: FormControl<string | null>;
        animalId: FormControl<number | null>;
        habitatId: FormControl<number | null>;
        minDate: FormControl<string | null>;
        maxDate: FormControl<string | null>;
    }>;
    globalSearchControl: FormControl<string | null>;
    animalSearchControl: FormControl<Animal | null>;
    habitatSearchControl: FormControl<Habitat | null>;

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;
    @ViewChild(MatTable) table!: MatTable<Sighting>;

    constructor(
        private sightingService: SightingService,
        private animalService: AnimalService,
        private habitatService: HabitatService,
        private dialog: MatDialog,
        private snackBar: MatSnackBar,
        private fb: FormBuilder
    ) {
        this.dataSource = new MatTableDataSource<Sighting>();
        this.globalSearchControl = new FormControl<string | null>('');
        this.animalSearchControl = new FormControl<Animal | null>(null);
        this.habitatSearchControl = new FormControl<Habitat | null>(null);
        
        this.filtersForm = this.fb.group({
            weather: new FormControl<string | null>(''),
            animalId: new FormControl<number | null>(null),
            habitatId: new FormControl<number | null>(null),
            minDate: new FormControl<string | null>(null),
            maxDate: new FormControl<string | null>(null)
        });

        // Initialize filtered observables
        this.filteredAnimals$ = this.animalSearchControl.valueChanges.pipe(
            startWith(''),
            map(value => this._filterAnimals(value))
        );

        this.filteredHabitats$ = this.habitatSearchControl.valueChanges.pipe(
            startWith(''),
            map(value => this._filterHabitats(value))
        );
    }

    ngOnInit() {
        this.loadSightings();
        this.loadAnimals();
        this.loadHabitats();
        this.setupFilters();
    }

    ngAfterViewInit() {
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
    }

    loadAnimals() {
        this.animalService.getAll().subscribe(animals => {
            this.animals = animals;
        });
    }

    loadHabitats() {
        this.habitatService.getAll().subscribe(habitats => {
            this.habitats = habitats;
        });
    }

    setupFilters() {
        // Setup global search
        this.globalSearchControl.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        ).subscribe(value => {
            this.applyFilters();
        });

        // Setup individual filters
        this.filtersForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        ).subscribe(() => {
            this.applyFilters();
        });

        // Setup animal search
        this.animalSearchControl.valueChanges.subscribe(animal => {
            if (animal && typeof animal === 'object') {
                this.filtersForm.get('animalId')?.setValue(animal.id);
            } else {
                this.filtersForm.get('animalId')?.setValue(null);
            }
        });

        // Setup habitat search
        this.habitatSearchControl.valueChanges.subscribe(habitat => {
            if (habitat && typeof habitat === 'object') {
                this.filtersForm.get('habitatId')?.setValue(habitat.id);
            } else {
                this.filtersForm.get('habitatId')?.setValue(null);
            }
        });
    }

    private _filterAnimals(value: string | Animal | null): Animal[] {
        if (!value) return this.animals;
        const filterValue = typeof value === 'string' ? value.toLowerCase() : value.name.toLowerCase();
        return this.animals.filter(animal => 
            animal.name.toLowerCase().includes(filterValue) || 
            animal.species.toLowerCase().includes(filterValue)
        );
    }

    private _filterHabitats(value: string | Habitat | null): Habitat[] {
        if (!value) return this.habitats;
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

    getAnimalName(animalId: number): string {
        const animal = this.animals.find(a => a.id === animalId);
        return animal ? `${animal.name} (${animal.species})` : `Animal ${animalId}`;
    }

    getHabitatName(habitatId: number): string {
        const habitat = this.habitats.find(h => h.id === habitatId);
        return habitat ? `${habitat.name} (${habitat.location})` : `Habitat ${habitatId}`;
    }

    applyFilters() {
        const globalSearch = this.globalSearchControl.value?.toLowerCase() || '';
        const filters = this.filtersForm.value;

        this.dataSource.filterPredicate = (data: Sighting) => {
            const matchesGlobalSearch = !globalSearch || 
                Object.values(data).some(value => 
                    value?.toString().toLowerCase().includes(globalSearch)
                );

            const matchesWeather = !filters.weather || 
                data.weatherConditions?.toLowerCase().includes(filters.weather.toLowerCase());
            
            const matchesAnimalId = !filters.animalId || data.animalId === filters.animalId;
            const matchesHabitatId = !filters.habitatId || data.habitatId === filters.habitatId;
            
            const sightingDate = new Date(data.sightingDateTime);
            const matchesDate = (!filters.minDate || sightingDate >= new Date(filters.minDate)) && 
                              (!filters.maxDate || sightingDate <= new Date(filters.maxDate));

            return matchesGlobalSearch && matchesWeather && matchesAnimalId && 
                   matchesHabitatId && matchesDate;
        };

        this.dataSource.filter = 'trigger';
    }

    clearFilters() {
        this.globalSearchControl.setValue('');
        this.animalSearchControl.setValue(null);
        this.habitatSearchControl.setValue(null);
        this.filtersForm.reset();
    }

    loadSightings() {
        this.sightingService.getAll().subscribe({
            next: (sightings) => {
                this.sightings = sightings;
                this.dataSource.data = sightings;
            },
            error: (error) => {
                this.snackBar.open('Error loading sightings', 'Close', { duration: 3000 });
                console.error('Error loading sightings:', error);
            }
        });
    }

    openCreateDialog() {
        const dialogRef = this.dialog.open(SightingDialogComponent, {
            data: {}
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.sightingService.create(result).subscribe({
                    next: (newSighting) => {
                        this.sightings = [...this.sightings, newSighting];
                        this.dataSource.data = this.sightings;
                        this.snackBar.open('Sighting created successfully', 'Close', { duration: 3000 });
                    },
                    error: (error) => {
                        this.snackBar.open('Error creating sighting', 'Close', { duration: 3000 });
                        console.error('Error creating sighting:', error);
                    }
                });
            }
        });
    }

    openEditDialog(sighting: Sighting) {
        const dialogRef = this.dialog.open(SightingDialogComponent, {
            data: { sighting }
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.sightingService.update(sighting.id, result).subscribe({
                    next: (updatedSighting) => {
                        this.sightings = this.sightings.map(s => s.id === sighting.id ? updatedSighting : s);
                        this.dataSource.data = this.sightings;
                        this.snackBar.open('Sighting updated successfully', 'Close', { duration: 3000 });
                    },
                    error: (error) => {
                        this.snackBar.open('Error updating sighting', 'Close', { duration: 3000 });
                        console.error('Error updating sighting:', error);
                    }
                });
            }
        });
    }

    deleteSighting(sighting: Sighting) {
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            width: '600px',
            data: {
                title: 'Confirm Deletion',
                message: 'Are you sure you want to delete this sighting?',
                confirmText: 'Delete',
                cancelText: 'Cancel'
            }
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.sightingService.delete(sighting.id).subscribe({
                    next: () => {
                        this.sightings = this.sightings.filter(s => s.id !== sighting.id);
                        this.dataSource.data = this.sightings;
                        this.snackBar.open('Sighting deleted successfully', 'Close', { duration: 3000 });
                    },
                    error: (error) => {
                        this.snackBar.open('Error deleting sighting', 'Close', { duration: 3000 });
                        console.error('Error deleting sighting:', error);
                    }
                });            
            }
        });
    }

    // Form control getters
    get weatherControl() {
        return this.filtersForm.get('weather') as FormControl<string | null>;
    }

    get animalIdControl() {
        return this.filtersForm.get('animalId') as FormControl<number | null>;
    }

    get habitatIdControl() {
        return this.filtersForm.get('habitatId') as FormControl<number | null>;
    }

    get minDateControl() {
        return this.filtersForm.get('minDate') as FormControl<string | null>;
    }

    get maxDateControl() {
        return this.filtersForm.get('maxDate') as FormControl<string | null>;
    }
} 