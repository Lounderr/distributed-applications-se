import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { LoadingComponent } from './components/loading/loading.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule, 
    RouterOutlet,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatSidenavModule,
    MatListModule,
    LoadingComponent
  ],
  template: `
    <app-loading></app-loading>
    <mat-toolbar color="primary">
      <button mat-icon-button (click)="sidenav.toggle()">
        <mat-icon>menu</mat-icon>
      </button>
      <span>Wildlife Tracker</span>
    </mat-toolbar>

    <mat-sidenav-container>
      <mat-sidenav #sidenav mode="side">
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
        <div class="content">
          <router-outlet></router-outlet>
        </div>
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

    .content {
      padding: 20px;
    }

    mat-nav-list {
      a {
        display: flex;
        align-items: center;
        gap: 10px;
      }
    }
  `]
})
export class AppComponent {
  title = 'Wildlife Tracker';
}
