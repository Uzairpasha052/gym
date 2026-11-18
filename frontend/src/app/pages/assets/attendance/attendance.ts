import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Attendance } from '../../../services/attendanceService/attendance';
import { AdminAttendance } from '../../../models/attendance/attAdmin.model';
import { UpdateAttendance } from '../../../models/attendance/attUpdate.model';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { DatePicker } from 'primeng/datepicker';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [DatePipe, FormsModule, TableModule, InputTextModule, ButtonModule, IconFieldModule, InputIconModule, DialogModule, SelectModule, ToastModule, DatePicker, BaseChartDirective],
  templateUrl: './attendance.html',
  styleUrl: '../admin-screen/admin-screen/admin-screen.scss',
  providers: [MessageService]
})
export class AttendancePage implements OnInit {
  attendanceList: AdminAttendance[] = [];
  attendanceTotal = 0;
  attendancePageSize = 10;
  searchTerm = '';
  sortField = '';
  sortOrder = 1;
  attendanceloading = false;
  selectedMonth = new Date();
  searchPlayerName = '';
  selectedPlayer: { name: string } | null = null;
  presentDays = 0;
  absentDays = 0;
  displayAttEditDialog = false;
  selectedAttendance!: AdminAttendance;
  updateAttendance: UpdateAttendance = { status: '', remarks: null };
  statusOptions = [
    { label: 'Present', value: 'Present' },
    { label: 'Absent', value: 'Absent' }
  ];
  pieChartType: 'pie' = 'pie';
  pieChartData: ChartData<'pie'> = {
    labels: ['Present', 'Absent'],
    datasets: [{ data: [0, 0], backgroundColor: ['#22c55e', '#ef4444'], borderWidth: 0 }]
  };
  pieChartOptions: ChartOptions<'pie'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'right' },
      tooltip: {
        callbacks: {
          label: context => {
            const value = Number(context.raw);
            const total = context.dataset.data.map(Number).reduce((a, b) => a + b, 0);
            const percentage = total === 0 ? 0 : ((value / total) * 100).toFixed(1);
            return `${context.label}: ${percentage}%`;
          }
        }
      }
    }
  };

  constructor(private attendanceService: Attendance, private messageService: MessageService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void { this.loadAttendance(); }

  loadAttendance(pageNumber = 1, pageSize = this.attendancePageSize): void {
    this.attendancePageSize = pageSize;
    this.attendanceloading = true;
    const year = this.selectedMonth.getFullYear();
    const month = this.selectedMonth.getMonth() + 1;
    this.attendanceService.getAttendanceHistory(
      year, month, pageNumber, pageSize, this.searchTerm, this.sortField, this.sortOrder
    ).subscribe({
      next: result => {
        this.attendanceList = result.items;
        this.attendanceTotal = result.totalCount;
        this.attendanceloading = false;
        this.cdr.detectChanges();
      },
      error: error => {
        this.attendanceloading = false;
        this.showError(error, 'Unable to load attendance.');
      }
    });
  }

  onMonthChange(): void {
    this.loadAttendance();
    if (this.searchPlayerName.trim()) this.searchPlayer();
  }

  onPage(event: TableLazyLoadEvent): void {
    const size = event.rows ?? this.attendancePageSize;
    if (event.globalFilter !== undefined) {
      const filter = event.globalFilter;
      this.searchTerm = (Array.isArray(filter) ? filter[0] : filter ?? '').trim();
    }
    if (event.sortField !== undefined) {
      this.sortField = (Array.isArray(event.sortField) ? event.sortField[0] : event.sortField) ?? '';
      this.sortOrder = event.sortOrder ?? 1;
    }
    this.loadAttendance(Math.floor((event.first ?? 0) / size) + 1, size);
  }

  searchPlayer(): void {
    const name = this.searchPlayerName.trim();
    if (!name) { this.clearPlayer(); return; }
    this.attendanceService.getPlayerAttendanceSummary(
      this.selectedMonth.getFullYear(), this.selectedMonth.getMonth() + 1, name
    ).subscribe({
      next: summary => {
        this.selectedPlayer = { name: summary.playerName };
        this.presentDays = summary.presentCount;
        this.absentDays = summary.absentCount;
        this.updateChart();
      },
      error: error => {
        this.clearPlayer();
        if (error.status !== 404) this.showError(error, 'Unable to load attendance summary.');
        this.cdr.detectChanges();
      }
    });
  }

  clearPlayer(): void {
    this.selectedPlayer = null;
    this.presentDays = 0;
    this.absentDays = 0;
    this.updateChart();
  }

  updateChart(): void {
    this.pieChartData = {
      labels: ['Present', 'Absent'],
      datasets: [{ data: [this.presentDays, this.absentDays], backgroundColor: ['#22c55e', '#ef4444'], borderWidth: 0 }]
    };
    this.cdr.detectChanges();
  }

  openAttEditDialog(attendance: AdminAttendance): void {
    this.selectedAttendance = attendance;
    this.updateAttendance = { remarks: attendance.remarks, status: attendance.status };
    this.displayAttEditDialog = true;
  }

  saveEditAttendance(): void {
    this.attendanceService.updateAttendance(this.selectedAttendance.attendanceId, this.updateAttendance).subscribe({
      next: response => {
        this.messageService.add({ severity: 'success', summary: 'Success', detail: response.message });
        this.displayAttEditDialog = false;
        this.loadAttendance();
      },
      error: error => this.showError(error, 'Unable to update attendance.')
    });
  }

  private showError(error: any, fallback: string): void {
    this.messageService.add({ severity: 'error', summary: 'Error', detail: error?.error?.message || fallback });
  }
}
