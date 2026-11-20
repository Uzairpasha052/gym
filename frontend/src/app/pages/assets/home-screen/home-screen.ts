import { ChangeDetectorRef, Component } from '@angular/core';
import { Attendance } from '../../../services/attendanceService/attendance';
import { TodayAttendance } from '../../../models/attendance/att_today.model';
import { MarkAttendance } from '../../../models/attendance/markAttendance.model';

// import { AttendanceHistory } from '../../../models/attendance/attHistory.model';
// import { finalize } from 'rxjs';

import { FormsModule } from '@angular/forms';
import { CommonModule, DatePipe, NgClass } from '@angular/common';


import { TableModule } from 'primeng/table';
import { CheckboxModule } from 'primeng/checkbox';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

@Component({
  selector: 'app-home-screen',
  imports: [CommonModule, FormsModule, ToastModule, TableModule,
    CheckboxModule, ButtonModule, TagModule,
    InputTextModule, ProgressSpinnerModule],
  templateUrl: './home-screen.html',
  styleUrl: './home-screen.scss',
  providers: [MessageService]
})
export class HomeScreen {

  attendanceList: TodayAttendance[] = [];

  loading: boolean = false;
  saving: boolean = false;

  constructor(
    private attendanceService: Attendance,
    private messageService: MessageService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadTodayAttendance();
  }

  loadTodayAttendance(): void {
    this.loading = true;
    this.cdr.detectChanges();

    const startedAt = Date.now();

    this.attendanceService.getTodayAttendance(1, 100).subscribe({
      next: (response) => {
        this.attendanceList = response.items.map(player => ({
          ...player,
          status: player.status === 'Present'
            ? 'Present'
            : 'Absent'
        }));

        this.hideLoaderAfterDelay(startedAt);
      },

      error: (error) => {
        // console.error('Error loading attendance:', error);

        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Unable to load today attendance.'
        });

        this.hideLoaderAfterDelay(startedAt);
      }
    });
  }

  private hideLoaderAfterDelay(startedAt: number): void {
    const remaining = Math.max(0, 3000 - (Date.now() - startedAt));

    setTimeout(() => {
      this.loading = false;
      this.cdr.detectChanges();
    }, remaining);
  }

  toggleAttendance(player: TodayAttendance): void {
    if (player.status === 'Present') {
      player.status = 'Absent';
    } else {
      player.status = 'Present';
    }

    this.cdr.detectChanges();
  }

  saveAttendance(): void {

    if (this.attendanceList.length === 0) {
      this.messageService.add({
        severity: 'warn',
        summary: 'No Players',
        detail: 'There are no players available.'
      });

      return;
    }

    const attendanceData: MarkAttendance[] =
      this.attendanceList.map(player => ({
        playerId: player.playerId,
        status: player.status,
        remarks: null
      }));

    this.saving = true;
    this.cdr.detectChanges();

    this.attendanceService.saveAttendance(attendanceData).subscribe({

      next: (response) => {

        this.saving = false;
        this.cdr.detectChanges();

        if (response.success) {

          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: response.message || 'Attendance saved successfully.'
          });

        } else {

          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: response.message || 'Unable to save attendance.'
          });
        }
      },

      error: (error) => {

        // console.error('Error saving attendance:', error);

        this.saving = false;
        this.cdr.detectChanges();

        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Unable to save attendance.'
        });
      }
    });
  }

  getPresentCount(): number {
    return this.attendanceList.filter(
      player => player.status === 'Present'
    ).length;
  }

  getAbsentCount(): number {
    return this.attendanceList.filter(
      player => player.status !== 'Present'
    ).length;
  }

}
