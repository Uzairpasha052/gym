import { ChangeDetectorRef, Component, CUSTOM_ELEMENTS_SCHEMA, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserModel } from '../../../models/user.model';
import { UserService } from '../../../services/getUserService/user-service';
import { RegisterPopup } from '../register_pop-up/register-popup/register-popup';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { FloatLabelModule } from 'primeng/floatlabel';
import { SelectModule } from 'primeng/select';
import { ToastModule } from 'primeng/toast';
import { RoleModel } from '../../../models/role.model';
import { RoleService } from '../../../services/role-service/role.service';
import { PlayerService } from '../../../services/playerService/player';
import { PlayerModel } from '../../../models/player/player.model';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [DatePipe, FormsModule, RegisterPopup, TableModule, InputTextModule, ButtonModule, IconFieldModule, InputIconModule, ConfirmDialogModule, DialogModule, FloatLabelModule, SelectModule, ToastModule],
  providers: [ConfirmationService, MessageService],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './users.html',
  styleUrl: '../admin-screen/admin-screen/admin-screen.scss'
})
export class Users implements OnInit {
  users: UserModel[] = [];
  usersTotal = 0;
  usersPageSize = 5;
  searchTerm = '';
  sortField = '';
  sortOrder = 1;
  showRegister = false;
  displayEditDialog = false;
  selectedUser: any = {};
  roles: RoleModel[] = [];
  players: PlayerModel[] = [];

  constructor(
    private userService: UserService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private cdr: ChangeDetectorRef,
    private roleService: RoleService,
    private playerService: PlayerService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.roleService.getRoles().subscribe({
      next: roles => { this.roles = roles; this.cdr.detectChanges(); },
      error: error => this.showError(error, 'Unable to load roles.')
    });
    this.loadPlayers();
  }

  openRegister(): void { this.showRegister = true; }
  closeRegister(): void { this.showRegister = false; this.loadUsers(); }

  loadUsers(pageNumber = 1, pageSize = this.usersPageSize): void {
    this.usersPageSize = pageSize;
    this.userService.getUsers(pageNumber, pageSize, this.searchTerm, this.sortField, this.sortOrder).subscribe({
      next: result => { 
        this.users = result.items; 
        this.usersTotal = result.totalCount; 
        this.cdr.detectChanges(); 
      },
      error: error => this.showError(error, 'Unable to load users.')
    });
  }

  onPage(event: TableLazyLoadEvent): void {
    const size = event.rows ?? this.usersPageSize;
    if (event.globalFilter !== undefined) {
      const filter = event.globalFilter;
      this.searchTerm = (Array.isArray(filter) ? filter[0] : filter ?? '').trim();
    }
    if (event.sortField !== undefined) {
      this.sortField = (Array.isArray(event.sortField) ? event.sortField[0] : event.sortField) ?? '';
      this.sortOrder = event.sortOrder ?? 1;
    }
    this.loadUsers(Math.floor((event.first ?? 0) / size) + 1, size);
  }

  confirmDelete(user: UserModel): void {
    this.confirmationService.confirm({
      header: 'Delete User',
      message: `Are you sure you want to delete: ${user.fullName}?`,
      icon: 'pi pi-exclamation-triangle',
      accept: () => this.deleteUser(user.id)
    });
  }

  deleteUser(id: number): void {
    this.userService.deleteUser(id).subscribe({ next: () => this.loadUsers(), error: error => this.showError(error, 'Unable to delete user.') });
  }

  editUser(user: UserModel): void { this.selectedUser = { ...user, roleId: (user as any).roleId, playerId: user.playerId ?? null }; this.displayEditDialog = true; }

  get isEditingMember(): boolean { return this.roles.find(role => role.id === this.selectedUser.roleId)?.name?.toLowerCase() === 'member'; }

  private loadPlayers(): void {
    this.playerService.getPlayers(1, 100).subscribe({ next: result => { this.players = result.items.filter(player => player.status === 'Active'); this.cdr.detectChanges(); } });
  }

  saveUser(): void {
    if (!this.isEditingMember) this.selectedUser.playerId = null;
    this.userService.updateUser(this.selectedUser.id, this.selectedUser).subscribe({
      next: () => { this.loadUsers(); this.displayEditDialog = false; },
      error: error => this.showError(error, 'Unable to update user.')
    });
  }

  private showError(error: any, fallback: string): void {
    this.messageService.add({ severity: 'error', summary: 'Error', detail: error?.error?.message || fallback });
  }
}
