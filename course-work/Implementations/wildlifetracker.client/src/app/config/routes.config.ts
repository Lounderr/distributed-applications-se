import { Routes } from '@angular/router';
import { LayoutComponent } from '../components/layout/layout.component';
import { LoginComponent } from '../pages/login/login.component';
import { RegisterComponent } from '../pages/register/register.component';
import { authGuard } from '../guards/auth.guard';

export const ROUTES_CONFIG = {
    auth: {
        login: 'login',
        register: 'register'
    },
    main: {
        root: '',
        animals: 'animals',
        habitats: 'habitats',
        sightings: 'sightings'
    }
};

export const routes: Routes = [
    {
        path: ROUTES_CONFIG.auth.login,
        component: LoginComponent
    },
    {
        path: ROUTES_CONFIG.auth.register,
        component: RegisterComponent
    },
    {
        path: ROUTES_CONFIG.main.root,
        component: LayoutComponent,
        canActivate: [authGuard],
        children: [
            {
                path: ROUTES_CONFIG.main.animals,
                loadChildren: () => import('../features/animals/animals.routes').then(m => m.ANIMALS_ROUTES)
            },
            {
                path: ROUTES_CONFIG.main.habitats,
                loadChildren: () => import('../features/habitats/habitats.routes').then(m => m.HABITATS_ROUTES)
            },
            {
                path: ROUTES_CONFIG.main.sightings,
                loadChildren: () => import('../features/sightings/sightings.routes').then(m => m.SIGHTINGS_ROUTES)
            },
            {
                path: ROUTES_CONFIG.main.root,
                redirectTo: ROUTES_CONFIG.main.animals,
                pathMatch: 'full'
            }
        ]
    }
]; 