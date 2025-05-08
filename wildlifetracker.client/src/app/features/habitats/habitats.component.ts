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
import { HabitatService } from '../../services/habitat.service';
import { Habitat } from '../../models/habitat.model';
import { HabitatDialogComponent } from './habitat-dialog.component';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfirmDialogComponent } from '../../components/confirm-dialog/confirm-dialog.component';

@Component({
    selector: 'app-habitats',
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
                <h1>Habitats</h1>
                <button mat-raised-button color="primary" (click)="openCreateDialog()">
                    <mat-icon>add</mat-icon>
                    Add Habitat
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
                        <mat-label>Location</mat-label>
                        <input matInput [formControl]="locationControl" placeholder="Filter by location">
                    </mat-form-field>

                    <mat-form-field>
                        <mat-label>Climate</mat-label>
                        <input matInput [formControl]="climateControl" placeholder="Filter by climate">
                    </mat-form-field>

                    <mat-form-field>
                        <mat-label>Size Range</mat-label>
                        <div class="range-inputs">
                            <input matInput type="number" [formControl]="minSizeControl" placeholder="Min">
                            <span *ngIf="minSizeControl.value || maxSizeControl.value">-</span>
                            <input matInput type="number" [formControl]="maxSizeControl" placeholder="Max">
                        </div>
                    </mat-form-field>

                    <mat-form-field>
                        <mat-label>Temperature Range</mat-label>
                        <div class="range-inputs">
                            <input matInput type="number" [formControl]="minTempControl" placeholder="Min">
                            <span *ngIf="minTempControl.value || maxTempControl.value">-</span>
                            <input matInput type="number" [formControl]="maxTempControl" placeholder="Max">
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

                    <!-- Location Column -->
                    <ng-container matColumnDef="location">
                        <th mat-header-cell *matHeaderCellDef mat-sort-header>Location</th>
                        <td mat-cell *matCellDef="let row">{{row.location}}</td>
                    </ng-container>

                    <!-- Size Column -->
                    <ng-container matColumnDef="size">
                        <th mat-header-cell *matHeaderCellDef mat-sort-header>Size (km²)</th>
                        <td mat-cell *matCellDef="let row">{{row.size}}</td>
                    </ng-container>

                    <!-- Climate Column -->
                    <ng-container matColumnDef="climate">
                        <th mat-header-cell *matHeaderCellDef mat-sort-header>Climate</th>
                        <td mat-cell *matCellDef="let row">{{row.climate}}</td>
                    </ng-container>

                    <!-- Temperature Column -->
                    <ng-container matColumnDef="averageTemperature">
                        <th mat-header-cell *matHeaderCellDef mat-sort-header>Avg. Temperature (°C)</th>
                        <td mat-cell *matCellDef="let row">{{row.averageTemperature}}</td>
                    </ng-container>

                    <!-- Actions Column -->
                    <ng-container matColumnDef="actions">
                        <th mat-header-cell *matHeaderCellDef>Actions</th>
                        <td mat-cell *matCellDef="let row">
                            <button mat-icon-button color="primary" (click)="openEditDialog(row)">
                                <mat-icon>edit</mat-icon>
                            </button>
                            <button mat-icon-button color="warn" (click)="deleteHabitat(row)">
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

                <mat-paginator [pageSizeOptions]="[5, 10, 25, 100]" aria-label="Select page of habitats"></mat-paginator>
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
export class HabitatsComponent implements OnInit, AfterViewInit {
    displayedColumns: string[] = ['name', 'location', 'size', 'climate', 'averageTemperature', 'actions'];
    dataSource: MatTableDataSource<Habitat>;
    habitats: Habitat[] = [];
    filtersForm: FormGroup<{
        name: FormControl<string | null>;
        location: FormControl<string | null>;
        climate: FormControl<string | null>;
        minSize: FormControl<number | null>;
        maxSize: FormControl<number | null>;
        minTemp: FormControl<number | null>;
        maxTemp: FormControl<number | null>;
    }>;
    globalSearchControl: FormControl<string | null>;

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;
    @ViewChild(MatTable) table!: MatTable<Habitat>;

    constructor(
        private habitatService: HabitatService,
        private dialog: MatDialog,
        private snackBar: MatSnackBar,
        private fb: FormBuilder
    ) {
        this.dataSource = new MatTableDataSource<Habitat>();
        this.globalSearchControl = new FormControl<string | null>('');
        this.filtersForm = this.fb.group({
            name: new FormControl<string | null>(''),
            location: new FormControl<string | null>(''),
            climate: new FormControl<string | null>(''),
            minSize: new FormControl<number | null>(null),
            maxSize: new FormControl<number | null>(null),
            minTemp: new FormControl<number | null>(null),
            maxTemp: new FormControl<number | null>(null)
        });
    }

    ngOnInit() {
        this.loadHabitats();
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

        this.dataSource.filterPredicate = (data: Habitat) => {
            const matchesGlobalSearch = !globalSearch || 
                Object.values(data).some(value => 
                    value?.toString().toLowerCase().includes(globalSearch)
                );

            const matchesName = !filters.name || 
                data.name?.toLowerCase().includes(filters.name.toLowerCase());
            
            const matchesLocation = !filters.location || 
                data.location?.toLowerCase().includes(filters.location.toLowerCase());
            
            const matchesClimate = !filters.climate || 
                data.climate?.toLowerCase().includes(filters.climate.toLowerCase());
            
            const matchesSize = (!filters.minSize || (data.size ?? 0) >= filters.minSize) && 
                               (!filters.maxSize || (data.size ?? 0) <= filters.maxSize);
            
            const matchesTemp = (!filters.minTemp || (data.averageTemperature ?? 0) >= filters.minTemp) && 
                               (!filters.maxTemp || (data.averageTemperature ?? 0) <= filters.maxTemp);

            return matchesGlobalSearch && matchesName && matchesLocation && 
                   matchesClimate && matchesSize && matchesTemp;
        };

        this.dataSource.filter = 'trigger';
    }

    clearFilters() {
        this.globalSearchControl.setValue('');
        this.filtersForm.reset();
    }

    loadHabitats() {
        this.habitatService.getAll().subscribe({
            next: (habitats) => {
                this.habitats = habitats;
                this.dataSource.data = habitats;
            },
            error: (error) => {
                this.snackBar.open('Error loading habitats', 'Close', { duration: 3000 });
                console.error('Error loading habitats:', error);
            }
        });
    }

    openCreateDialog() {
        const dialogRef = this.dialog.open(HabitatDialogComponent, {
            data: {}
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.habitatService.create(result).subscribe({
                    next: (newHabitat) => {
                        this.habitats = [...this.habitats, newHabitat];
                        this.dataSource.data = this.habitats;
                        this.snackBar.open('Habitat created successfully', 'Close', { duration: 3000 });
                    },
                    error: (error) => {
                        this.snackBar.open('Error creating habitat', 'Close', { duration: 3000 });
                        console.error('Error creating habitat:', error);
                    }
                });
            }
        });
    }

    openEditDialog(habitat: Habitat) {
        const dialogRef = this.dialog.open(HabitatDialogComponent, {
            data: { habitat }
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.habitatService.update(habitat.id, result).subscribe({
                    next: (updatedHabitat) => {
                        this.habitats = this.habitats.map(h => h.id === habitat.id ? updatedHabitat : h);
                        this.dataSource.data = this.habitats;
                        this.snackBar.open('Habitat updated successfully', 'Close', { duration: 3000 });
                    },
                    error: (error) => {
                        this.snackBar.open('Error updating habitat', 'Close', { duration: 3000 });
                        console.error('Error updating habitat:', error);
                    }
                });
            }
        });
    }

    deleteHabitat(habitat: Habitat) {
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            width: '600px',
            data: {
                title: 'Confirm Deletion',
                message: 'Are you sure you want to delete this habitat?',
                confirmText: 'Delete',
                cancelText: 'Cancel'
            }
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.habitatService.delete(habitat.id).subscribe({
                    next: () => {
                        this.habitats = this.habitats.filter(h => h.id !== habitat.id);
                        this.dataSource.data = this.habitats;
                        this.snackBar.open('Habitat deleted successfully', 'Close', { duration: 3000 });
                    },
                    error: (error) => {
                        this.snackBar.open('Error deleting habitat', 'Close', { duration: 3000 });
                        console.error('Error deleting habitat:', error);
                    }
                });            
            }
        });
    }

    // Form control getters
    get nameControl() {
        return this.filtersForm.get('name') as FormControl<string | null>;
    }

    get locationControl() {
        return this.filtersForm.get('location') as FormControl<string | null>;
    }

    get climateControl() {
        return this.filtersForm.get('climate') as FormControl<string | null>;
    }

    get minSizeControl() {
        return this.filtersForm.get('minSize') as FormControl<number | null>;
    }

    get maxSizeControl() {
        return this.filtersForm.get('maxSize') as FormControl<number | null>;
    }

    get minTempControl() {
        return this.filtersForm.get('minTemp') as FormControl<number | null>;
    }

    get maxTempControl() {
        return this.filtersForm.get('maxTemp') as FormControl<number | null>;
    }
} 