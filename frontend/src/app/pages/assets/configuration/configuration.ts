import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { RoleModel } from '../../../models/role.model';
import { SidebarMenuCreateRequest, SidebarMenuItem } from '../../../models/sidebar-menu.model';
import { RoleService } from '../../../services/role-service/role.service';
import { SidebarService } from '../../../services/sidebarService/sidebar.service';
import { UserService } from '../../../services/getUserService/user-service';
import { UserModel } from '../../../models/user.model';
import { forkJoin, map } from 'rxjs';
import { Checkbox } from 'primeng/checkbox';

@Component({
  selector: 'app-configuration',
  standalone: true,
  imports: [DatePipe, FormsModule, TableModule, 
    ButtonModule, DialogModule, InputTextModule, 
    ToastModule, ConfirmDialogModule, Checkbox],
  providers: [ConfirmationService, MessageService],
  templateUrl: './configuration.html',
  styleUrl: './configuration.scss',
})
export class Configuration implements OnInit {
  roles: RoleModel[] = [];
  loading = false;
  loadError = '';
  dialogVisible = false;
  roleName = '';
  roleIsActive = true;  
  editingRole: RoleModel | null = null;
  sidebarMenus: SidebarMenuItem[] = [];
  sidebarParents: SidebarMenuItem[] = [];
  sidebarUsers: UserModel[] = [];
  sidebarUsersLoading = false;
  sidebarMenusLoading = false;
  sidebarMenusError = '';
  sidebarDialogVisible = false;
  sidebarSaving = false;
  editingSidebarMenu: SidebarMenuItem | null = null;
  sidebarForm: SidebarMenuCreateRequest & { isActive: boolean } = this.emptySidebarForm();

  get duplicateDisplayOrder(): boolean {
    const parentId = this.sidebarForm.parentId ?? null;
    const order = Number(this.sidebarForm.displayOrder);
    return this.sidebarMenus.some(menu =>
      menu.id !== this.editingSidebarMenu?.id &&
      (menu.parentId ?? null) === parentId &&
      menu.displayOrder === order
    );
  }

  get isGroupForm(): boolean {
    return !this.sidebarForm.route.trim();
  }

  get selectableSidebarParents(): SidebarMenuItem[] {
    return this.sidebarParents.filter(parent => parent.id !== this.editingSidebarMenu?.id && parent.isActive !== false && !parent.route?.trim());
  }

  constructor(
    private roleService: RoleService,
    private sidebarService: SidebarService,
    private userService: UserService,
    private confirmation: ConfirmationService,
    private messages: MessageService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadRoles();
    this.loadSidebarMenus();
  }

  private emptySidebarForm(): SidebarMenuCreateRequest & { isActive: boolean } {
    return { title: '', route: '', icon: '', parentId: null, displayOrder: 1, roleIds: [], userIds: [], isActive: true };
  }

  loadSidebarMenus(): void {
    this.sidebarMenusLoading = true;
    this.sidebarMenusError = '';
    this.sidebarService.getAllMenus().subscribe({
      next: menus => {
        this.sidebarMenus = menus.sort((a, b) => {
          const aIsRoot = a.parentId == null;
          const bIsRoot = b.parentId == null;
          if (aIsRoot !== bIsRoot) return aIsRoot ? -1 : 1;
          if (aIsRoot && bIsRoot) return a.displayOrder - b.displayOrder || a.id - b.id;
          const parentOrderA = menus.find(menu => menu.id === a.parentId)?.displayOrder ?? 0;
          const parentOrderB = menus.find(menu => menu.id === b.parentId)?.displayOrder ?? 0;
          return parentOrderA - parentOrderB || (a.parentId ?? 0) - (b.parentId ?? 0) || a.displayOrder - b.displayOrder || a.id - b.id;
        });
        this.sidebarParents = this.sidebarMenus.filter(menu => menu.parentId == null);
        this.sidebarMenusLoading = false;
        this.cdr.detectChanges();
      },
      error: error => {
        this.sidebarMenusError = error?.error?.message || 'Unable to load sidebar routes.';
        this.sidebarMenusLoading = false;
        this.cdr.detectChanges();
      }
    });
    this.loadSidebarUsers();
  }

  private loadSidebarUsers(): void {
    this.sidebarUsersLoading = true;
    const pageSize = 100;
    this.userService.getUsers(1, pageSize, '', 'fullName', 1).subscribe({
      next: firstPage => {
        const pageCount = Math.ceil(firstPage.totalCount / pageSize);
        if (pageCount <= 1) {
          this.sidebarUsers = firstPage.items;
          this.sidebarUsersLoading = false;
          this.cdr.detectChanges();
          return;
        }
        const remainingPages = Array.from({ length: pageCount - 1 }, (_, index) =>
          this.userService.getUsers(index + 2, pageSize, '', 'fullName', 1));
        forkJoin(remainingPages).pipe(map(pages => [firstPage, ...pages])).subscribe({
          next: pages => {
            this.sidebarUsers = pages.flatMap(page => page.items);
            this.sidebarUsersLoading = false;
            this.cdr.detectChanges();
          },
          error: error => {
            this.sidebarUsersLoading = false;
            this.showError(error, 'Unable to load users for route access.');
            this.cdr.detectChanges();
          }
        });
      },
      error: error => {
        this.sidebarUsersLoading = false;
        this.showError(error, 'Unable to load users for route access.');
        this.cdr.detectChanges();
      }
    });
  }

  openCreateSidebarMenu(): void {
    this.editingSidebarMenu = null;
    this.sidebarForm = this.emptySidebarForm();
    this.sidebarDialogVisible = true;
  }

  onSidebarRouteChange(): void {
    if (!this.sidebarForm.route.trim()) {
      this.sidebarForm.roleIds = [];
      this.sidebarForm.userIds = [];
    }
  }

  toggleRole(roleId: number, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.sidebarForm.roleIds = checked
      ? [...new Set([...this.sidebarForm.roleIds, roleId])]
      : this.sidebarForm.roleIds.filter(id => id !== roleId);
  }

  toggleUser(userId: number, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.sidebarForm.userIds = checked
      ? [...new Set([...this.sidebarForm.userIds, userId])]
      : this.sidebarForm.userIds.filter(id => id !== userId);
  }

  openEditSidebarMenu(menu: SidebarMenuItem): void {
    this.sidebarService.getMenu(menu.id).subscribe({
      next: item => {
        this.editingSidebarMenu = item;
        this.sidebarForm = {
          title: item.title,
          route: item.route,
          icon: item.icon,
          parentId: item.parentId ?? null,
          displayOrder: item.displayOrder,
          roleIds: item.route?.trim() ? [...(item.roleIds ?? [])] : [],
          userIds: item.route?.trim() ? [...(item.userIds ?? [])] : [],
          isActive: item.isActive ?? true
        };
        this.sidebarDialogVisible = true;
        this.cdr.detectChanges();
      },
      error: error => {
        this.showError(error, 'Unable to load sidebar route details.');
        this.cdr.detectChanges();
      }
    });
  }

  saveSidebarMenu(): void {
    const request: SidebarMenuCreateRequest = {
      ...this.sidebarForm,
      title: this.sidebarForm.title.trim(),
      route: this.sidebarForm.route.trim(),
      icon: this.sidebarForm.icon.trim(),
      roleIds: [...this.sidebarForm.roleIds]
    };
    if (!request.title || (request.parentId !== null && !request.route)) {
      this.messages.add({ severity: 'warn', summary: 'Required fields', detail: 'Enter a title. A route is required for a page inside a group; leave it blank for a top-level group heading.' });
      return;
    }
    if (!Number.isInteger(Number(request.displayOrder)) || Number(request.displayOrder) < 1) {
      this.messages.add({ severity: 'warn', summary: 'Invalid display order', detail: 'Use a whole number starting at 1.' });
      return;
    }
    if (this.duplicateDisplayOrder) {
      this.messages.add({ severity: 'warn', summary: 'Order already used', detail: 'Choose an order not used by another item at this same level.' });
      return;
    }

    this.sidebarSaving = true;
    const operation = this.editingSidebarMenu
      ? this.sidebarService.updateMenu(this.editingSidebarMenu.id, { ...request, isActive: this.sidebarForm.isActive })
      : this.sidebarService.createMenu(request);
    operation.subscribe({
      next: () => {
        this.sidebarSaving = false;
        this.sidebarDialogVisible = false;
        this.messages.add({ severity: 'success', summary: 'Saved', detail: 'Sidebar route saved.' });
        this.loadSidebarMenus();
        this.cdr.detectChanges();
      },
      error: error => {
        this.sidebarSaving = false;
        this.showError(error, 'Unable to save sidebar route.');
        this.cdr.detectChanges();
      }
    });
  }

  parentTitle(menu: SidebarMenuItem): string {
    return menu.parentId == null
      ? (this.hasChildren(menu) || !menu.route ? 'Sidebar group' : 'Top-level page')
      : this.sidebarParents.find(parent => parent.id === menu.parentId)?.title ?? `Menu ${menu.parentId}`;
  }

  hasChildren(menu: SidebarMenuItem): boolean {
    return this.sidebarMenus.some(candidate => candidate.parentId === menu.id);
  }

  accessSummary(menu: SidebarMenuItem): string {
    if (!menu.route?.trim()) return 'Inherited from child pages';
    const roleNames = (menu.roleIds ?? [])
      .map(id => this.roles.find(role => role.id === id)?.name)
      .filter((name): name is string => Boolean(name));
    const userNames = (menu.userIds ?? [])
      .map(id => this.sidebarUsers.find(user => user.id === id)?.fullName)
      .filter((name): name is string => Boolean(name));
    const all = [...roleNames, ...userNames];
    return all.length ? all.join(', ') : 'No direct grants';
  }

  loadRoles(): void {
    this.loading = true;
    this.loadError = '';
    this.roleService.getRoles().subscribe({
      next: roles => {
        this.roles = roles;
        console.log(roles)
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: error => {
        this.loading = false;
        this.loadError = error.status === 404
          ? 'The roles API returned 404. Restart the backend to load the new roles endpoint.'
          : `Unable to load roles (HTTP ${error.status}). Check the backend connection and your admin login.`;
        this.showError(error, 'Unable to load roles.');
        this.cdr.detectChanges();
      }
    });
  }

  openCreate(): void { this.editingRole = null; this.roleName = ''; this.dialogVisible = true; }
  openEdit(role: RoleModel): void 
  { 
    this.editingRole = role; 
    this.roleName = role.name; 
    this.roleIsActive = role.isActive;
    this.dialogVisible = true; 
  }

  saveRole(): void {
    const name = this.roleName.trim();
    if (!name) { this.messages.add({ severity: 'warn', summary: 'Role name required', detail: 'Enter a role name.' }); return; }
    const request = this.editingRole
      ? this.roleService.updateRole(this.editingRole.id, name, this.roleIsActive )
      : this.roleService.createRole(name);
    request.subscribe({
      next: () => {
        this.dialogVisible = false;
        this.messages.add({ severity: 'success', summary: 'Saved', detail: `Role ${this.editingRole ? 'updated' : 'created'}.` });
        this.cdr.detectChanges();
        this.loadRoles();
      },
      error: error => { this.showError(error, 'Unable to save role.'); this.cdr.detectChanges(); }
    });
  }

  confirmDelete(role: RoleModel): void {
    this.confirmation.confirm({
      header: 'Delete Role', message: `Delete the ${role.name} role?`, icon: 'pi pi-exclamation-triangle',
      accept: () => this.roleService.deleteRole(role.id).subscribe({
        next: () => { this.messages.add({ severity: 'success', summary: 'Deleted', detail: 'Role deleted.' }); this.cdr.detectChanges(); this.loadRoles(); },
        error: error => { this.showError(error, 'Unable to delete role.'); this.cdr.detectChanges(); }
      })
    });
  }

  private showError(error: any, fallback: string): void {
    this.messages.add({ severity: 'error', summary: 'Error', detail: error?.error?.message || fallback });
  }
}
