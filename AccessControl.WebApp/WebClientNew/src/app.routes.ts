import { Routes } from '@angular/router';
import { AppLayout } from './app/layout/component/app.layout';
import { Login } from '@/pages/login/login';
import { Dashboard } from '@/pages/dashboard/dashboard';

export const appRoutes: Routes = [
    // Login độc lập không dùng layout
    { path: 'login', component: Login },
    {
        path: '',
        component: AppLayout,
        children: [
            { path: '', redirectTo: '/login', pathMatch: 'full' },
            { path: 'dashboard', component: Dashboard },
            { path: 'system', loadChildren: () => import('./app/pages/system/system.routes') },
            { path: 'manage', loadChildren: () => import('./app/pages/manage/manage.routes') },
            { path: 'statistic', loadChildren: () => import('./app/pages/statistic/statistic.routes') }
        ]
    }
];
