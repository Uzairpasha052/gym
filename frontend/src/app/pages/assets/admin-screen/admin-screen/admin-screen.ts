import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { SidebarMenuItem } from '../../../../models/sidebar-menu.model';
import { SidebarService } from '../../../../services/sidebarService/sidebar.service';

@Component({
  selector: 'app-admin-screen',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './admin-screen.html',
  styleUrl: './admin-screen.scss'
})
export class AdminScreen implements OnInit {
  menuItems: SidebarMenuItem[] = [];
  loadingMenu = true;
  menuError = '';

  constructor(private sidebar: SidebarService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.sidebar.getParentMenus().subscribe({
      next: parents => {
        const adminParent = parents.find(menu => this.normalizeRoute(menu.route) === '/admin');

        if (!adminParent) {
          this.finishWithError('No Admin menu is assigned to this role.');
          return;
        }

        this.sidebar.getChildMenus(adminParent.id).subscribe({
          next: children => {
            this.menuItems = children;
            this.loadingMenu = false;
            this.cdr.detectChanges();
          },
          error: error => this.finishWithError(error?.error?.message || 'Unable to load Admin pages.')
        });
      },
      error: error => this.finishWithError(error?.error?.message || 'Unable to load navigation.')
    });
  }

  private normalizeRoute(route: string): string {
    const normalized = `/${route.trim().replace(/^\/+|\/+$/g, '')}`;
    return normalized === '/' ? '/' : normalized.toLowerCase();
  }

  routeFor(item: SidebarMenuItem): string {
    const route = this.normalizeRoute(item.route);
    return route === '/admin' ? '/users'
      : route.startsWith('/admin/') ? route.slice('/admin'.length)
      : route;
  }

  private finishWithError(message: string): void {
    this.menuError = message;
    this.loadingMenu = false;
    this.cdr.detectChanges();
  }
}
