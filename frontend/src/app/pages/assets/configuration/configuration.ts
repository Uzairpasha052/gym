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
import { catchError, forkJoin, of } from 'rxjs';

@Component({
  selector: 'app-configuration',
  standalone: true,
  imports: [DatePipe, FormsModule, TableModule, ButtonModule, DialogModule, InputTextModule, ToastModule, ConfirmDialogModule],
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
  editingRole: RoleModel | null = null;
  sidebarMenus: SidebarMenuItem[] = [];
  sidebarParents: SidebarMenuItem[] = [];
  sidebarMenusLoading = false;
  sidebarMenusError = '';
  sidebarDialogVisible = false;
  sidebarSaving = false;
  editingSidebarMenu: SidebarMenuItem | null = null;
  sidebarForm: SidebarMenuCreateRequest & { isActive: boolean } = this.emptySidebarForm();

  get selectableSidebarParents(): SidebarMenuItem[] {
    return this.sidebarParents.filter(parent => parent.id !== this.editingSidebarMenu?.id);
  }

  constructor(
    private roleService: RoleService,
    private sidebarService: SidebarService,
    private confirmation: ConfirmationService,
    private messages: MessageService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadRoles();
    this.loadSidebarMenus();
  }

  private emptySidebarForm(): SidebarMenuCreateRequest & { isActive: boolean } {
    return { title: '', route: '', icon: '', parentId: null, displayOrder: 1, roleIds: [], isActive: true };
  }

  loadSidebarMenus(): void {
    this.sidebarMenusLoading = true;
    this.sidebarMenusError = '';
    this.sidebarService.getParentMenus().subscribe({
      next: parents => {
        this.sidebarParents = parents;
        if (!parents.length) {
          this.sidebarMenus = [];
          this.sidebarMenusLoading = false;
          this.cdr.detectChanges();
          return;
        }

        forkJoin(parents.map(parent => this.sidebarService.getChildMenus(parent.id).pipe(catchError(() => of([] as SidebarMenuItem[])))))
          .subscribe(childLists => {
            this.sidebarMenus = [...parents, ...childLists.flat()]
              .sort((a, b) => a.displayOrder - b.displayOrder || a.id - b.id);
            this.sidebarMenusLoading = false;
            this.cdr.detectChanges();
          });
      },
      error: error => {
        this.sidebarMenusError = error?.error?.message || 'Unable to load sidebar routes.';
        this.sidebarMenusLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  openCreateSidebarMenu(): void {
    this.editingSidebarMenu = null;
    this.sidebarForm = this.emptySidebarForm();
    this.sidebarDialogVisible = true;
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
          roleIds: [...(item.roleIds ?? [])],
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
    if (!request.title || !request.route || !request.icon || !request.roleIds.length) {
      this.messages.add({ severity: 'warn', summary: 'Required fields', detail: 'Enter a title, route, icon, and assign at least one role.' });
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
      ? 'Dashboard'
      : this.sidebarParents.find(parent => parent.id === menu.parentId)?.title ?? `Menu ${menu.parentId}`;
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
  openEdit(role: RoleModel): void { this.editingRole = role; this.roleName = role.name; this.dialogVisible = true; }

  saveRole(): void {
    const name = this.roleName.trim();
    if (!name) { this.messages.add({ severity: 'warn', summary: 'Role name required', detail: 'Enter a role name.' }); return; }
    const request = this.editingRole
      ? this.roleService.updateRole(this.editingRole.id, name)
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
