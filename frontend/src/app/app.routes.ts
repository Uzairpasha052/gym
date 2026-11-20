import { Routes } from '@angular/router';
import { authGuard } from './guard/auth-guard/auth-guard';
import { sidebarPageGuard } from './guard/sidebar-page-guard/sidebar-page-guard';
import { HomeScreen } from './pages/assets/home-screen/home-screen';
import { AdminScreen } from './pages/assets/admin-screen/admin-screen/admin-screen';
import { CashIn } from './pages/assets/cashIn/cash-in';
import { CashOut } from './pages/assets/cashOut/cash-out';
import { NLogin } from './pages/n-login/n-login';
import { Users } from './pages/assets/users/users';
import { AttendancePage } from './pages/assets/attendance/attendance';
import { PlayerRegister } from './pages/assets/playerRegister/player-register/player-register';
import { Reporting } from './pages/assets/reporting/reporting';
import { AuditLog } from './pages/assets/audit-log/audit-log';
import { Configuration } from './pages/assets/configuration/configuration';
import { Membership } from './pages/assets/membership/membership';
import { ExpiringMembers } from './pages/assets/expiring-members/expiring-members';
import { Dashboard } from './pages/dashboard/dashboard/dashboard';

export const routes: Routes = [
  { path: '', component: NLogin },
  // Reserve the Dashboard component for later; use AdminScreen as the current landing page.
  { path: 'dashboard', redirectTo: 'users', pathMatch: 'full' },
  // Keep the attendance URL inside the AdminScreen layout below.

  { path: 'extra', component: Dashboard},

  // Compatibility redirects for URLs that included the old /admin prefix.
  { path: 'admin', redirectTo: 'users', pathMatch: 'full' },
  {
    path: 'admin',
    children: [
      { path: 'users', redirectTo: '/users', pathMatch: 'full' },
      { path: 'attendance', redirectTo: '/attendance', pathMatch: 'full' },
      { path: 'players', redirectTo: '/players', pathMatch: 'full' },
      { path: 'reports', redirectTo: '/reports', pathMatch: 'full' },
      { path: 'audit-logs', redirectTo: '/audit-logs', pathMatch: 'full' },
      { path: 'configuration', redirectTo: '/configuration', pathMatch: 'full' },
      { path: 'membership', redirectTo: '/membership', pathMatch: 'full' },
      { path: 'expiring-members', redirectTo: '/expiring-members', pathMatch: 'full' },
      {
        path: 'cash',
        children: [
          { path: 'in', redirectTo: '/cash/in', pathMatch: 'full' },
          { path: 'out', redirectTo: '/cash/out', pathMatch: 'full' }
        ]
      }
    ]
  },

  // Legacy dashboard entry points.
//   { path: 'take-attendance', redirectTo: 'homeScreen', pathMatch: 'full' },
  { path: 'adminScreen', redirectTo: 'users', pathMatch: 'full' },
  { path: 'cashScreen', redirectTo: 'cash/in', pathMatch: 'full' },

  {
    path: '',
    component: AdminScreen,
    canActivate: [authGuard],
    canActivateChild: [sidebarPageGuard],
    children: [
      { path: '', redirectTo: 'users', pathMatch: 'full' },
      { path: 'take-attendance', component: HomeScreen },
      { path: 'homeScreen', redirectTo: 'take-attendance', pathMatch: 'full' },
      { path: 'users', component: Users },
      { path: 'attendance', component: AttendancePage },
      { path: 'players', component: PlayerRegister },
      { path: 'reports', component: Reporting },
      { path: 'audit-logs', component: AuditLog },
      { path: 'configuration', component: Configuration },
      { path: 'membership', component: Membership },
      { path: 'expiring-members', component: ExpiringMembers },
      {
        path: 'cash',
        children: [
          { path: 'in', component: CashIn },
          { path: 'out', component: CashOut }/*  */
        ]
      }
    ]
  },

  { path: '**', redirectTo: 'users' }
];
