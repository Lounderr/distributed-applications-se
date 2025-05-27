import { Routes } from '@angular/router';
import { AnimalsComponent } from './animals.component';
import { authGuard } from '../../guards/auth.guard';

export const ANIMALS_ROUTES: Routes = [
    {
        path: '',
        component: AnimalsComponent,
        canActivate: [authGuard]
    }
]; 