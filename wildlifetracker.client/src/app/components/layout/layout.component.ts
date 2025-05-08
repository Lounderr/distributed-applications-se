import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';

@Component({
    selector: 'app-layout',
    standalone: true,
    imports: [
        CommonModule,
        RouterOutlet,
        MatToolbarModule,
        MatSidenavModule,
        MatListModule,
        MatIconModule
    ],
    template: `
        <mat-toolbar color="primary">
            <span>Wildlife Tracker</span>
        </mat-toolbar>
        <mat-sidenav-container>
            <mat-sidenav mode="side" opened>
                <mat-nav-list>
                    <a mat-list-item routerLink="/animals">
                        <mat-icon>pets</mat-icon>
                        <span>Animals</span>
                    </a>
                    <a mat-list-item routerLink="/habitats">
                        <mat-icon>landscape</mat-icon>
                        <span>Habitats</span>
                    </a>
                    <a mat-list-item routerLink="/sightings">
                        <mat-icon>visibility</mat-icon>
                        <span>Sightings</span>
                    </a>
                </mat-nav-list>
            </mat-sidenav>
            <mat-sidenav-content>
                <router-outlet></router-outlet>
            </mat-sidenav-content>
        </mat-sidenav-container>
    `,
    styles: [`
        mat-sidenav-container {
            height: calc(100vh - 64px);
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
    `]
})
export class LayoutComponent {} 