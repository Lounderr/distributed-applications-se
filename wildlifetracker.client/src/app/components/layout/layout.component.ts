import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AuthService } from '../../services/auth.service';
import { ConfirmDialogComponent } from '../../components/confirm-dialog/confirm-dialog.component';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { OnlineUsersComponent } from '../online-users/online-users.component';

@Component({
    selector: 'app-layout',
    standalone: true,
    imports: [
        CommonModule,
        RouterOutlet,
        RouterLink,
        MatSidenavModule,
        MatListModule,
        MatIconModule,
        MatDialogModule,
        MatButtonModule,
        MatToolbarModule,
        OnlineUsersComponent
    ],
    template: `
        <mat-sidenav-container>
            <mat-sidenav #sidenav [mode]="isMobile ? 'over' : 'side'" [opened]="!isMobile">
                <mat-nav-list>
                    <a mat-list-item [routerLink]="['/animals']">
                        <mat-icon>pets</mat-icon>
                        <span>Animals</span>
                    </a>
                    <a mat-list-item [routerLink]="['/habitats']">
                        <mat-icon>landscape</mat-icon>
                        <span>Habitats</span>
                    </a>
                    <a mat-list-item [routerLink]="['/sightings']">
                        <mat-icon>visibility</mat-icon>
                        <span>Sightings</span>
                    </a>
                    <a mat-list-item (click)="onLogout()">
                        <mat-icon>logout</mat-icon>
                        <span>Logout</span>
                    </a>
                </mat-nav-list>
                <app-online-users></app-online-users>
            </mat-sidenav>
            <mat-sidenav-content>
                <mat-toolbar *ngIf="isMobile">
                    <button mat-icon-button (click)="sidenav.toggle()">
                        <mat-icon>menu</mat-icon>
                    </button>
                    <span>Wildlife Tracker</span>
                </mat-toolbar>
                <div class="content">
                    <router-outlet></router-outlet>
                </div>
            </mat-sidenav-content>
        </mat-sidenav-container>
    `,
    styles: [`
        mat-sidenav-container {
            height: 100vh;
        }
        mat-sidenav {
            width: 250px;
        }
        mat-nav-list {
            padding-top: 20px;
        }
        mat-icon {
            margin-right: 10px;
        }
        .content {
            padding: 20px;
        }
    `]
})
export class LayoutComponent implements OnInit {
    isMobile = false;

    constructor(
        private authService: AuthService,
        private dialog: MatDialog,
        private breakpointObserver: BreakpointObserver
    ) {}

    ngOnInit() {
        this.breakpointObserver.observe([
            Breakpoints.Handset
        ]).subscribe(result => {
            this.isMobile = result.matches;
        });
    }

    onLogout(): void {
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            width: '300px',
            data: {
                title: 'Confirm Logout',
                message: 'Are you sure you want to logout?',
                confirmText: 'Logout',
                cancelText: 'Cancel'
            }
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.authService.logout();
            }
        });
    }
} 