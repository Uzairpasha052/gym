import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DatePicker } from 'primeng/datepicker';
import { InputNumberModule } from 'primeng/inputnumber';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { CashOutListModel, CashOutModel } from '../../../models/CashFlow/cashOut.model';
import { CashService } from '../../../services/cashFlowService/cash-service';

@Component({
  selector: 'app-cash-out',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, InputNumberModule, DatePicker, ButtonModule, TableModule],
  templateUrl: './cash-out.html',
  styleUrl: '../cash-flow/cash-flow.scss'
})
export class CashOut implements OnInit {
  form!: FormGroup;
  records: CashOutListModel[] = [];
  selectedMonth = new Date();
  totalRecords = 0;
  pageSize = 10;
  saving = false;

  constructor(
    private fb: FormBuilder,
    private cashService: CashService,
    private messages: MessageService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      description: [null, Validators.required],
      amount: [null, [Validators.required, Validators.min(1)]],
      date: [new Date(), Validators.required]
    });
    this.loadRecords();
  }

  loadRecords(pageNumber = 1, pageSize = this.pageSize): void {
    this.pageSize = pageSize;
    this.cashService.GetAllCashOut(this.selectedMonth.getFullYear(), this.selectedMonth.getMonth() + 1, pageNumber, pageSize).subscribe({
      next: response => {
        this.records = response.items;
        this.totalRecords = response.totalCount;
        this.cdr.detectChanges();
      },
      error: error => this.showError(error, 'Unable to load cash-out records.')
    });
  }

  onPage(event: TableLazyLoadEvent): void {
    const size = event.rows ?? this.pageSize;
    this.loadRecords(Math.floor((event.first ?? 0) / size) + 1, size);
  }

  onMonthChange(): void { this.loadRecords(); }

  save(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving = true;
    const value = this.form.value;
    const request: CashOutModel = {
      amount: value.amount,
      description: value.description,
      paymentDate: this.formatDate(value.date)
    };
    this.cashService.CreateCashOut(request).subscribe({
      next: () => {
        this.saving = false;
        this.form.reset({ amount: null, description: null, date: new Date() });
        this.messages.add({ severity: 'success', summary: 'Saved', detail: 'Cash out saved successfully.' });
        this.loadRecords();
        this.cdr.detectChanges();
      },
      error: error => {
        this.saving = false;
        this.showError(error, 'Unable to save cash out.');
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
