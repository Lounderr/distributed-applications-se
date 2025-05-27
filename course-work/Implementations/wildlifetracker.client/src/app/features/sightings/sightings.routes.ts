import { Routes } from '@angular/router';
import { SightingsComponent } from './sightings.component';
import { authGuard } from '../../guards/auth.guard';

export const SIGHTINGS_ROUTES: Routes = [
    {
        path: '',
        component: SightingsComponent,
        canActivate: [authGuard]
    }
]; 