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
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { RouterModule } from '@angular/router';
import { AnimalService } from '../../services/animal.service';
import { Animal } from '../../models/animal.model';
import { AnimalDialogComponent } from './animal-dialog.component';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfirmDialogComponent } from '../../components/confirm-dialog/confirm-dialog.component';

@Component({
    selector: 'app-animals',
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
        FormsModule,
        ReactiveFormsModule,
        RouterModule
    ],
    template: `
        <div class="container">
            <div class="header">
                <h1>Animals</h1>
                <button mat-raised-button color="primary" (click)="openCreateDialog()">
                    <mat-icon>add</mat-icon>
                    Add Animal
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
                        <mat-label>Name</mat-label>
                        <input matInput [formControl]="nameControl" placeholder="Filter by name">
                    </mat-form-field>

                    <mat-form-field>
                        <mat-label>Species</mat-label>
                        <input matInput [formControl]="speciesControl" placeholder="Filter by species">
                    </mat-form-field>

                    <mat-form-field>
                        <mat-label>Age Range</mat-label>
                        <div class="range-inputs">
                            <input matInput type="number" [formControl]="minAgeControl" placeholder="Min">
                            <span>-</span>
                            <input matInput type="number" [formControl]="maxAgeControl" placeholder="Max">
                        </div>
                    </mat-form-field>

                    <mat-form-field>
                        <mat-label>Weight Range (kg)</mat-label>
                        <div class="range-inputs">
                            <input matInput type="number" [formControl]="minWeightControl" placeholder="Min">
                            <span>-</span>
                            <input matInput type="number" [formControl]="maxWeightControl" placeholder="Max">
                        </div>
                    </mat-form-field>

                    <mat-form-field>
                        <mat-label>Height Range (cm)</mat-label>
                        <div class="range-inputs">
                            <input matInput type="number" [formControl]="minHeightControl" placeholder="Min">
                            <span>-</span>
                            <input matInput type="number" [formControl]="maxHeightControl" placeholder="Max">
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
                    <!-- Name Column -->
                    <ng-container matColumnDef="name">
                        <th mat-header-cell *matHeaderCellDef mat-sort-header>Name</th>
                        <td mat-cell *matCellDef="let row">{{row.name}}</td>
                    </ng-container>

                    <!-- Species Column -->
                    <ng-container matColumnDef="species">
                        <th mat-header-cell *matHeaderCellDef mat-sort-header>Species</th>
                        <td mat-cell *matCellDef="let row">{{row.species}}</td>
                    </ng-container>

                    <!-- Age Column -->
                    <ng-container matColumnDef="age">
                        <th mat-header-cell *matHeaderCellDef mat-sort-header>Age</th>
                        <td mat-cell *matCellDef="let row">{{row.age}}</td>
                    </ng-container>

                    <!-- Weight Column -->
                    <ng-container matColumnDef="weight">
                        <th mat-header-cell *matHeaderCellDef mat-sort-header>Weight (kg)</th>
                        <td mat-cell *matCellDef="let row">{{row.weight}}</td>
                    </ng-container>

                    <!-- Height Column -->
                    <ng-container matColumnDef="height">
                        <th mat-header-cell *matHeaderCellDef mat-sort-header>Height (cm)</th>
                        <td mat-cell *matCellDef="let row">{{row.height}}</td>
                    </ng-container>

                    <!-- Actions Column -->
                    <ng-container matColumnDef="actions">
                        <th mat-header-cell *matHeaderCellDef>Actions</th>
                        <td mat-cell *matCellDef="let row">
                            <button mat-icon-button color="primary" (click)="openEditDialog(row)">
                                <mat-icon>edit</mat-icon>
                            </button>
                            <button mat-icon-button color="warn" (click)="deleteAnimal(row)">
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

                <mat-paginator [pageSizeOptions]="[5, 10, 25, 100]" aria-label="Select page of animals"></mat-paginator>
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
            width: 80px;
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
export class AnimalsComponent implements OnInit, AfterViewInit {
    displayedColumns: string[] = ['name', 'species', 'age', 'weight', 'height', 'actions'];
    dataSource: MatTableDataSource<Animal>;
    animals: Animal[] = [];
    filtersForm: FormGroup<{
        name: FormControl<string | null>;
        species: FormControl<string | null>;
        minAge: FormControl<number | null>;
        maxAge: FormControl<number | null>;
        minWeight: FormControl<number | null>;
        maxWeight: FormControl<number | null>;
        minHeight: FormControl<number | null>;
        maxHeight: FormControl<number | null>;
    }>;
    globalSearchControl: FormControl<string | null>;

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;
    @ViewChild(MatTable) table!: MatTable<Animal>;

    constructor(
        private animalService: AnimalService,
        private dialog: MatDialog,
        private snackBar: MatSnackBar,
        private fb: FormBuilder
    ) {
        this.dataSource = new MatTableDataSource<Animal>();
        this.globalSearchControl = new FormControl<string | null>('');
        this.filtersForm = this.fb.group({
            name: new FormControl<string | null>(''),
            species: new FormControl<string | null>(''),
            minAge: new FormControl<number | null>(null),
            maxAge: new FormControl<number | null>(null),
            minWeight: new FormControl<number | null>(null),
            maxWeight: new FormControl<number | null>(null),
            minHeight: new FormControl<number | null>(null),
            maxHeight: new FormControl<number | null>(null)
        });
    }

    ngOnInit() {
        this.loadAnimals();
        this.setupFilters();
    }

    ngAfterViewInit() {
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
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
    }

    applyFilters() {
        const globalSearch = this.globalSearchControl.value?.toLowerCase() || '';
        const filters = this.filtersForm.value;

        this.dataSource.filterPredicate = (data: Animal) => {
            const matchesGlobalSearch = !globalSearch || 
                Object.values(data).some(value => 
                    value?.toString().toLowerCase().includes(globalSearch)
                );

            const matchesName = !filters.name || 
                data.name?.toLowerCase().includes(filters.name.toLowerCase());
            
            const matchesSpecies = !filters.species || 
                data.species?.toLowerCase().includes(filters.species.toLowerCase());
            
            const matchesAge = (!filters.minAge || (data.age ?? 0) >= filters.minAge) && 
                             (!filters.maxAge || (data.age ?? 0) <= filters.maxAge);
            
            const matchesWeight = (!filters.minWeight || (data.weight ?? 0) >= filters.minWeight) && 
                                (!filters.maxWeight || (data.weight ?? 0) <= filters.maxWeight);
            
            const matchesHeight = (!filters.minHeight || (data.height ?? 0) >= filters.minHeight) && 
                                (!filters.maxHeight || (data.height ?? 0) <= filters.maxHeight);

            return matchesGlobalSearch && matchesName && matchesSpecies && 
                   matchesAge && matchesWeight && matchesHeight;
        };

        this.dataSource.filter = 'trigger';
    }

    clearFilters() {
        this.globalSearchControl.setValue('');
        this.filtersForm.reset();
    }

    loadAnimals() {
        this.animalService.getAll().subscribe({
            next: (animals) => {
                this.animals = animals;
                this.dataSource.data = animals;
            },
            error: (error) => {
                this.snackBar.open('Error loading animals', 'Close', { duration: 3000 });
                console.error('Error loading animals:', error);
            }
        });
    }

    openCreateDialog() {
        const dialogRef = this.dialog.open(AnimalDialogComponent, {
            data: {}
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.animalService.create(result).subscribe({
                    next: (newAnimal) => {
                        this.animals = [...this.animals, newAnimal];
                        this.dataSource.data = this.animals;
                        this.snackBar.open('Animal created successfully', 'Close', { duration: 3000 });
                    },
                    error: (error) => {
                        this.snackBar.open('Error creating animal', 'Close', { duration: 3000 });
                        console.error('Error creating animal:', error);
                    }
                });
            }
        });
    }

    openEditDialog(animal: Animal) {
        const dialogRef = this.dialog.open(AnimalDialogComponent, {
            data: { animal }
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.animalService.update(animal.id, result).subscribe({
                    next: (updatedAnimal) => {
                        this.animals = this.animals.map(a => a.id === animal.id ? updatedAnimal : a);
                        this.dataSource.data = this.animals;
                        this.snackBar.open('Animal updated successfully', 'Close', { duration: 3000 });
                    },
                    error: (error) => {
                        this.snackBar.open('Error updating animal', 'Close', { duration: 3000 });
                        console.error('Error updating animal:', error);
                    }
                });
            }
        });
    }

    deleteAnimal(animal: Animal) {
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            width: '600px',
            data: {
                title: 'Confirm Deletion',
                message: 'Are you sure you want to delete this animal?',
                confirmText: 'Delete',
                cancelText: 'Cancel'
            }
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.animalService.delete(animal.id).subscribe({
                    next: () => {
                        this.animals = this.animals.filter(a => a.id !== animal.id);
                        this.dataSource.data = this.animals;
                        this.snackBar.open('Animal deleted successfully', 'Close', { duration: 3000 });
                    },
                    error: (error) => {
                        this.snackBar.open('Error deleting animal', 'Close', { duration: 3000 });
                        console.error('Error deleting animal:', error);
                    }
                });            
            }
        });
    }

    // Form control getters
    get nameControl() {
        return this.filtersForm.get('name') as FormControl<string | null>;
    }

    get speciesControl() {
        return this.filtersForm.get('species') as FormControl<string | null>;
    }

    get minAgeControl() {
        return this.filtersForm.get('minAge') as FormControl<number | null>;
    }

    get maxAgeControl() {
        return this.filtersForm.get('maxAge') as FormControl<number | null>;
    }

    get minWeightControl() {
        return this.filtersForm.get('minWeight') as FormControl<number | null>;
    }

    get maxWeightControl() {
        return this.filtersForm.get('maxWeight') as FormControl<number | null>;
    }

    get minHeightControl() {
        return this.filtersForm.get('minHeight') as FormControl<number | null>;
    }

    get maxHeightControl() {
        return this.filtersForm.get('maxHeight') as FormControl<number | null>;
    }
} 