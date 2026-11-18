import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { ButtonModule } from 'primeng/button';
import { DatePicker } from 'primeng/datepicker';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { CashInModel, CashInListModel } from '../../../models/CashFlow/cashIn.model';
import { PlayerModel } from '../../../models/player/player.model';
import { CashService } from '../../../services/cashFlowService/cash-service';
import { PlayerService } from '../../../services/playerService/player';

@Component({
  selector: 'app-cash-in',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, SelectModule, InputNumberModule, DatePicker, ButtonModule, TableModule, ToastModule],
  providers: [MessageService],
  templateUrl: './cash-in.html',
  styleUrl: '../cash-flow/cash-flow.scss'
})
export class CashIn implements OnInit {
  form!: FormGroup;
  players: PlayerModel[] = [];
  records: CashInListModel[] = [];
  selectedMonth = new Date();
  totalRecords = 0;
  pageSize = 10;
  saving = false;
  loadingRecords = false;
  invalidSelectedFee = false;

  constructor(
    private fb: FormBuilder,
    private cashService: CashService,
    private playerService: PlayerService,
    private messages: MessageService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      playerId: [null, Validators.required],
      amount: [{ value: null, disabled: true }],
      date: [new Date(), Validators.required]
    });
    this.loadPlayers();
  }

  loadPlayers(): void {
    this.playerService.getPlayers(1, 100).subscribe({
      next: response => {
        this.players = response.items.filter(player => player.status?.toLowerCase() === 'active');
        this.cdr.detectChanges();
      },
      error: error => this.showError(error, 'Unable to load players.')
    });
  }

  onPlayerChange(playerId: number | null): void {
    this.invalidSelectedFee = false;
    const player = this.players.find(item => item.id === playerId);
    const amount = player ? Number(String(player.fee).replace(/,/g, '').trim()) : 0;

    if (!Number.isFinite(amount) || amount <= 0) {
      this.invalidSelectedFee = playerId !== null;
      this.form.controls['amount'].setValue(null);
      return;
    }

    this.form.controls['amount'].setValue(amount);
  }

  loadRecords(pageNumber = 1, pageSize = this.pageSize): void {
    this.pageSize = pageSize;
    this.loadingRecords = true;
    this.cashService.GetAllCashIn(this.selectedMonth.getFullYear(), 
      this.selectedMonth.getMonth() + 1, pageNumber, pageSize).subscribe({
        next: response => {
          this.records = response.items;
          this.totalRecords = response.totalCount;
          this.loadingRecords = false;
          this.cdr.detectChanges();
      },
      error: error => {
        this.records = [];
        this.totalRecords = 0;
        this.loadingRecords = false;
        this.showError(error, 'Unable to load cash-in records.');
        this.cdr.detectChanges();
      }
    });
  }

  onPage(event: TableLazyLoadEvent): void {
    const size = event.rows ?? this.pageSize;
    this.loadRecords(Math.floor((event.first ?? 0) / size) + 1, size);
  }

  onMonthChange(): void { this.loadRecords(); }

  save(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    const value = this.form.getRawValue();
    if (!Number.isFinite(value.amount) || value.amount <= 0) {
      this.invalidSelectedFee = true;
      this.messages.add({ severity: 'error', summary: 'Invalid fee', detail: 'The selected player does not have a valid monthly fee.' });
      return;
    }

    this.saving = true;
    const request: CashInModel = {
      playerId: value.playerId,
      amount: value.amount,
      paymentDate: this.formatDate(value.date)
    };
    this.cashService.CreateCashIn(request).subscribe({
      next: () => {
        this.saving = false;
        this.form.reset({ playerId: null, amount: null, date: new Date() });
        this.invalidSelectedFee = false;
        this.messages.add({ severity: 'success', summary: 'Saved', detail: 'Cash in saved successfully.' });
        this.loadRecords();
        this.cdr.detectChanges();
      },
      error: error => {
        this.saving = false;
        this.showError(error, 'Unable to save cash in.');
      }
    });
  }

  private formatDate(date: Date): string {
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${date.getFullYear()}-${month}-${day}`;
  }

  private showError(error: any, fallback: string): void {
    this.messages.add({ severity: 'error', summary: 'Error', detail: error?.error?.message || fallback });
  }
}
