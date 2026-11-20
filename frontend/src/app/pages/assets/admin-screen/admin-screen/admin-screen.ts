import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, CUSTOM_ELEMENTS_SCHEMA, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { SidebarMenuItem } from '../../../../models/sidebar-menu.model';
import { SidebarService } from '../../../../services/sidebarService/sidebar.service';
import { Auth } from '../../../../services/authService/auth';

interface SidebarNode extends SidebarMenuItem {
  children: SidebarNode[];
  isGroup: boolean;
}

@Component({
  selector: 'app-admin-screen',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './admin-screen.html',
  styleUrl: './admin-screen.scss'
})
export class AdminScreen implements OnInit {
  menuItems: SidebarNode[] = [];
  expandedMenuIds = new Set<number>();
  loadingMenu = true;
  menuError = '';

  userName: string = '';
  userRole: string = '';

  constructor(
    private sidebar: SidebarService,
    private auth: Auth,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.sidebar.getMyMenu().subscribe({
      next: items => {
        this.menuItems = this.buildMenuTree(items);
        const currentUrl = this.normalizeRoute(this.router.url);
        for (const group of this.menuItems) {
          if (group.children.some(child => this.normalizeRoute(child.route) === currentUrl)) {
            this.expandedMenuIds.add(group.id);
          }
        }
        this.loadingMenu = false;
        this.cdr.detectChanges();
      },
      error: error => {
        this.menuError = error?.error?.message || 'Unable to load pages assigned to this account.';
        this.loadingMenu = false;
        this.cdr.detectChanges();
      }
    });
  }

  private buildMenuTree(items: SidebarMenuItem[]): SidebarNode[] {
    const nodes = new Map<number, SidebarNode>();
    for (const item of items) nodes.set(item.id, { ...item, children: [], isGroup: item.isGroup ?? false });

    const roots: SidebarNode[] = [];
    for (const node of nodes.values()) {
      const parent = node.parentId == null ? undefined : nodes.get(node.parentId);
      if (parent) parent.children.push(node);
      else roots.push(node);
    }

    const sort = (itemsToSort: SidebarNode[]) => {
      itemsToSort.sort((a, b) => a.displayOrder - b.displayOrder || a.id - b.id);
      itemsToSort.forEach(item => sort(item.children));
    };
    sort(roots);
    return roots.filter(item => item.children.length > 0 || !item.isGroup);
  }

  routeFor(item: SidebarMenuItem): string {
    const route = this.normalizeRoute(item.route);
    return route.startsWith('/admin/') ? route.slice('/admin'.length) : route;
  }

  normalizeRoute(route: string): string {
    const path = (route || '').split(/[?#]/, 1)[0].trim().replace(/^\/+|\/+$/g, '');
    const normalized = `/${path}`.toLowerCase();
    const withoutLegacyAdmin = normalized.startsWith('/admin/') ? normalized.slice('/admin'.length) : normalized;
    return withoutLegacyAdmin === '/homescreen' ? '/take-attendance' : withoutLegacyAdmin;
  }

  isFontIcon(icon: string): boolean {
    return icon.startsWith('fa-') || icon.startsWith('pi ');
  }

  toggleGroup(item: SidebarNode): void {
    if (this.expandedMenuIds.has(item.id)) this.expandedMenuIds.delete(item.id);
    else this.expandedMenuIds.add(item.id);
  }

  isExpanded(item: SidebarNode): boolean {
    return this.expandedMenuIds.has(item.id);
  }

  isGroupActive(item: SidebarNode): boolean {
    const currentUrl = this.normalizeRoute(this.router.url);
    return item.children.some(child => this.normalizeRoute(child.route) === currentUrl);
  }

  logout(): void {
    this.auth.logout();
  }
}
