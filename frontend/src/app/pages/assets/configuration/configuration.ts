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
import { RoleService } from '../../../services/role-service/role.service';

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

  constructor(
    private roleService: RoleService,
    private confirmation: ConfirmationService,
    private messages: MessageService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void { this.loadRoles(); }

  loadRoles(): void {
    this.loading = true;
    this.loadError = '';
    this.roleService.getRoles().subscribe({
      next: roles => {
        this.roles = roles;
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
