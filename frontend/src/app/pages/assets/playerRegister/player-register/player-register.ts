import { ChangeDetectorRef, Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { PlayerService } from '../../../../services/playerService/player';
import { PlayerModel, UpdatePlayerRequest } from '../../../../models/player/player.model';
// import { ConfirmationService } from 'primeng/api';

import { DatePipe } from '@angular/common';
import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DialogModule } from 'primeng/dialog';
import { FloatLabelModule } from 'primeng/floatlabel';
import { SelectModule } from 'primeng/select';
import { FormsModule } from '@angular/forms';
import { DatePicker } from "primeng/datepicker";

import { PlayerRegisterPopUp } from '../../player-register-pop-up/player-register-pop-up';
import { TableLazyLoadEvent } from 'primeng/table';

@Component({
  selector: 'app-player-register',
  imports: [
    DatePipe,
    TableModule, InputTextModule,
    ButtonModule, IconFieldModule,
    InputIconModule, ConfirmDialogModule,
    DialogModule, FloatLabelModule,
    SelectModule, FormsModule,
    PlayerRegisterPopUp //For adding a player
    // DatePicker,
  ],
  providers: [ConfirmationService, MessageService],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],

  templateUrl: './player-register.html',
  styleUrl: './player-register.scss',
})
export class PlayerRegister {

  showRegister = false;
  openRegister() {
    this.showRegister = true;
  }
  closeRegister() {
    this.showRegister = false;
    this.loadUsers();
  }

  users: PlayerModel[] = [];
  totalRecords = 0;
  pageSize = 8;

  constructor(
    private getPlayer: PlayerService,
    private confirmationService: ConfirmationService,
    private cdr: ChangeDetectorRef, 
    
        private messageService: MessageService

  ){}

  ngOnInit(){
    this.loadUsers();

  }

  loadUsers(pageNumber = 1, pageSize = this.pageSize): void {
    this.pageSize = pageSize;
    this.getPlayer.getPlayers(pageNumber, pageSize).subscribe({
      next: (data) => {
        this.users = data.items;
        this.totalRecords = data.totalCount;
        this.cdr.detectChanges();
        console.log("LoadPlayer",data);
      },
      error: (error) => {
        this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: error?.error?.message || 'Unable to generate club Cash-In report.'
          });
      }
    })
  }

  onPage(event: TableLazyLoadEvent): void {
    const pageSize = event.rows ?? this.pageSize;
    this.loadUsers(Math.floor((event.first ?? 0) / pageSize) + 1, pageSize);
  }

  confirmDelete(user: any){
    this.confirmationService.confirm({
      header: 'Delete User',
      message: `Are you sure you want to delete: ${user.name}?`,
      icon: 'pi pi-exclamation-triangle',

      accept: () => {
        this.deleteUser(user.id);
      }
    })
  }

  deleteUser(id: number){
    // this.users = this.users.filter(user => user.id !== id)

    this.getPlayer.deletePlayer(id).subscribe({
      next: () => {
        this.loadUsers();
      }
    });

  }

  displayEditDialog = false;

  selectedUser: PlayerModel =
  {
    id: 0,
    name: '',
    fName: '',
    age: '',
    gender: '',
    contact: '',
    history: '',
    reason: '',
    fee: '',
    status: '',
    createdAt: '',
  };

  statuses = [
    { label: 'Active', value: 'Active' },
    { label: 'InActive', value: 'InActive' },
  ];

  editUser(user: PlayerModel){
    this.selectedUser = { ...user };
    this.displayEditDialog = true;
  }

  saveUser() {
    const update: UpdatePlayerRequest = {
      name: this.selectedUser.name,
      fName: this.selectedUser.fName,
      age: this.selectedUser.age,
      gender: this.selectedUser.gender,
      contact: this.selectedUser.contact,
      history: this.selectedUser.history,
      reason: this.selectedUser.reason,
      fee: this.selectedUser.fee,
      status: this.selectedUser.status,
    };

    this.getPlayer.updatePlayer(this.selectedUser.id, update).subscribe({
      next: () => {
        this.loadUsers();
        this.displayEditDialog = false;
      },
      error: (error) => {
        this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: error?.error?.message || 'Unable to generate club Cash-In report.'
          });
      }
      
    });
  }
}
