import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { PlayerModel } from '../../../models/player/player.model';
import { MembershipRecord } from '../../../models/membership/membership.model';
import { PlayerService } from '../../../services/playerService/player';
import { MembershipService } from '../../../services/membershipService/membership.service';

@Component({
  selector: 'app-membership',
  standalone: true,
  imports: [CommonModule, DatePipe, DecimalPipe, ReactiveFormsModule, ButtonModule, InputNumberModule, SelectModule, TableModule, ToastModule],
  providers: [MessageService],
  templateUrl: './membership.html',
  styleUrl: './membership.scss'
})
export class Membership implements OnInit {
  players: PlayerModel[] = [];
  memberships: MembershipRecord[] = [];
  totalMemberships = 0;
  membershipPageSize = 10;
  loadingPlayers = false;
  loadingMemberships = false;
  purchasing = false;
  selectedPlayerId: number | null = null;

  form: FormGroup;

  constructor(
    private formBuilder: FormBuilder,
    private playerService: PlayerService,
    private membershipService: MembershipService,
    private messages: MessageService,
    private cdr: ChangeDetectorRef
  ) {
    this.form = this.formBuilder.group({
      playerId: [null as number | null, Validators.required],
      durationMonths: [3, [Validators.required, Validators.min(3), Validators.max(120)]]
    });
  }

  ngOnInit(): void {
    this.loadPlayers();
  }

  loadPlayers(): void {
    this.loadingPlayers = true;
    this.playerService.getPlayers(1, 100).subscribe({
      next: result => {
        this.players = result.items.filter(player => player.status?.toLowerCase() === 'active');
        this.loadingPlayers = false;
        this.cdr.detectChanges();
      },
      error: error => {
        this.loadingPlayers = false;
        this.showError(error, 'Unable to load active players.');
        this.cdr.detectChanges();
      }
    });
  }

  onPlayerChange(playerId: number | null): void {
    this.selectedPlayerId = playerId;
    this.memberships = [];
    this.totalMemberships = 0;
    if (playerId == null) {
      this.cdr.detectChanges();
      return;
    }

    this.loadMemberships(playerId, 1, this.membershipPageSize);
  }

  loadMemberships(playerId: number, page = 1, pageSize = this.membershipPageSize): void {
    this.membershipPageSize = pageSize;
    this.loadingMemberships = true;
    this.membershipService.getPlayerMemberships(playerId, page, pageSize).subscribe({
      next: result => {
        this.memberships = result.items;
        this.totalMemberships = result.totalCount;
        this.loadingMemberships = false;
        this.cdr.detectChanges();
      },
      error: error => {
        this.loadingMemberships = false;
        this.showError(error, "Unable to load this player's membership history.");
        this.cdr.detectChanges();
      }
    });
  }

  onMembershipPage(event: TableLazyLoadEvent): void {
    if (this.selectedPlayerId === null) return;
    const size = event.rows ?? this.membershipPageSize;
    this.loadMemberships(this.selectedPlayerId, Math.floor((event.first ?? 0) / size) + 1, size);
  }

  purchase(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const values = this.form.getRawValue();
    const playerId = values['playerId'] as number | null;
    const durationMonths = values['durationMonths'] as number | null;
    if (playerId == null || durationMonths == null) return;

    this.purchasing = true;
    this.membershipService.purchase({ playerId, durationMonths }).subscribe({
      next: response => {
        this.purchasing = false;
        this.messages.add({ severity: 'success', summary: 'Membership purchased', detail: response.message });
        this.form.controls['durationMonths'].setValue(3);
        this.onPlayerChange(playerId);
        this.cdr.detectChanges();
      },
      error: error => {
        this.purchasing = false;
        this.showError(error, 'Unable to purchase membership.');
        this.cdr.detectChanges();
      }
    });
  }

  private showError(error: any, fallback: string): void {
    this.messages.add({ severity: 'error', summary: 'Error', detail: error?.error?.message || fallback });
  }
}
