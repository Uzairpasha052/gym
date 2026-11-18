import { Component, CUSTOM_ELEMENTS_SCHEMA, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Auth } from '../../../services/authService/auth';
import { AdminService } from '../../../services/adminService/admin-service';
import { SidebarMenuItem } from '../../../models/sidebar-menu.model';
import { SidebarService } from '../../../services/sidebarService/sidebar.service';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {
  constructor(private auth: Auth, private sidebar: SidebarService, private adminService: AdminService) {}

  hideApp = true;
  parentMenus: SidebarMenuItem[] = [];
  loadingMenu = true;
  menuError = '';

  toggleApps(): void {
    this.hideApp = !this.hideApp;
  }

  iconifyName(icon: string): string {
    const value = icon?.trim();
    if (!value) return 'material-symbols:apps';
    if (value.includes(':')) return value;

    const tokens = value.split(/\s+/);
    const iconClass = tokens.find(token => /^fa-[a-z0-9-]+$/i.test(token) && !['fa-solid', 'fa-regular', 'fa-brands', 'fa-classic'].includes(token));
    if (!iconClass) return 'material-symbols:apps';

    const iconName = iconClass.replace(/^fa-/, '');
    const collection = tokens.includes('fa-brands') ? 'fa6-brands'
      : tokens.includes('fa-regular') && iconName !== 'user' ? 'fa6-regular'
      : 'fa6-solid';
    return `${collection}:${iconName}`;
  }

  logOut(): void{
    this.auth.logout();
    // this.router.navigate(['/']);
  }


  userName: string = '';
  userRole:string = '';
  ngOnInit(): void {
    const storedData = localStorage.getItem('user') || sessionStorage.getItem('user')
    const userData = storedData ? JSON.parse(storedData) : {};
    this.userName = userData.fullName || 'Guest';
    this.userRole = userData.role || userData.roleName || this.adminService.getRole() || 'noRole';

    this.sidebar.getParentMenus().subscribe({
      next: menus => {
        this.parentMenus = menus;
        this.loadingMenu = false;
      },
      error: error => {
        this.menuError = error?.error?.message || 'Unable to load dashboard shortcuts.';
        this.loadingMenu = false;
      }
    });
  }
}
