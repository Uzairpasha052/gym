import { inject } from '@angular/core';
import { CanActivateChildFn, Router } from '@angular/router';
import { map } from 'rxjs';
import { SidebarService } from '../../services/sidebarService/sidebar.service';

function normalize(route: string): string {
  const path = (route || '').split(/[?#]/, 1)[0].trim().replace(/^\/+|\/+$/g, '').toLowerCase();
  const normalized = `/${path}`;
  const withoutLegacyAdmin = normalized.startsWith('/admin/') ? normalized.slice('/admin'.length) : normalized;
  return withoutLegacyAdmin === '/homescreen' ? '/take-attendance' : withoutLegacyAdmin;
}

export const sidebarPageGuard: CanActivateChildFn = (_childRoute, state) => {
  const sidebar = inject(SidebarService);
  const router = inject(Router);
  const requestedPath = normalize(state.url);

  return sidebar.getMyMenu().pipe(map(items => {
    const parentIds = new Set(items.map(item => item.parentId).filter((id): id is number => id != null));
    const pageRoutes = items.filter(item => item.route?.trim() && !item.isGroup && !parentIds.has(item.id));
    if (pageRoutes.some(item => normalize(item.route) === requestedPath)) return true;

    const firstAvailablePage = pageRoutes
      .sort((a, b) => a.displayOrder - b.displayOrder || a.id - b.id)
      .find(item => normalize(item.route) !== requestedPath);

    return firstAvailablePage
      ? router.parseUrl(normalize(firstAvailablePage.route))
      : router.parseUrl('/');
  }));
};
