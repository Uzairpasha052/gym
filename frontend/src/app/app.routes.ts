import { Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard/dashboard';
import { authGuard } from './guard/auth-guard/auth-guard';
import { adminGuard } from './guard/admin-guard/admin-guard';
import { HomeScreen } from './pages/assets/home-screen/home-screen';
import { AdminScreen } from './pages/assets/admin-screen/admin-screen/admin-screen';
import { CashFlow } from './pages/assets/cash-flow/cash-flow';
import { CashIn } from './pages/assets/cashIn/cash-in';
import { CashOut } from './pages/assets/cashOut/cash-out';
import { NLogin } from './pages/n-login/n-login';
import { Users } from './pages/assets/users/users';
import { AttendancePage } from './pages/assets/attendance/attendance';
import { PlayerRegister } from './pages/assets/playerRegister/player-register/player-register';
import { Reporting } from './pages/assets/reporting/reporting';
import { AuditLog } from './pages/assets/audit-log/audit-log';
import { Configuration } from './pages/assets/configuration/configuration';

export const routes: Routes = [

    {path: '', component: NLogin},

    {path: 'dashboard', component: Dashboard, canActivate: [authGuard]},

    {path: 'homeScreen', component: HomeScreen, canActivate: [authGuard]},

    { path: 'adminScreen', redirectTo: 'admin/users', pathMatch: 'full' },
    { path: 'cashScreen', redirectTo: 'cash/in', pathMatch: 'full' },

    {
        path: 'admin',
        component: AdminScreen,
        canActivate: [adminGuard],
        children: [
            { path: '', redirectTo: 'users', pathMatch: 'full' },
            { path: 'users', component: Users },
            { path: 'attendance', component: AttendancePage },
            { path: 'players', component: PlayerRegister },
            { path: 'reports', component: Reporting },
            { path: 'audit-logs', component: AuditLog },
            { path: 'configuration', component: Configuration }
        ]
    },

    {
        path: 'cash',
        component: CashFlow,
        canActivate: [adminGuard],
        children: [
            { path: '', redirectTo: 'in', pathMatch: 'full' },
            { path: 'in', component: CashIn },
            { path: 'out', component: CashOut }
        ]
    },

    // {
    //     path: 'admin',
    //     canActivate: [adminGuard],
    //     children: [
    //         {
    //         path: 'users',
    //         component: AdminUsersComponent
    //         },
    //         {
    //         path: 'settings',
    //         component: AdminSettingsComponent
    //         }
    //     ]
    // }
];
