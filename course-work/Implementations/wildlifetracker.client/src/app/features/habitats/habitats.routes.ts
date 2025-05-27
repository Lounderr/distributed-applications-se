import { Routes } from '@angular/router';
import { HabitatsComponent } from './habitats.component';
import { authGuard } from '../../guards/auth.guard';

export const HABITATS_ROUTES: Routes = [
    {
        path: '',
        component: HabitatsComponent,
        canActivate: [authGuard]
    }
]; 