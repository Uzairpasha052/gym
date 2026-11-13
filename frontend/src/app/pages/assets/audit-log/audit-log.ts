import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { AuditLogModel } from '../../../models/audit-log.model';
import { AuditLogService } from '../../../services/auditLogService/audit-log.service';

@Component({
  selector: 'app-audit-log',
  standalone: true,
  imports: [CommonModule, DatePipe, TableModule, ToastModule, DialogModule, ButtonModule],
  providers: [MessageService],
  templateUrl: './audit-log.html',
  styleUrl: './audit-log.scss',
})
export class AuditLog implements OnInit {
  logs: AuditLogModel[] = [];
  totalRecords = 0;
  pageSize = 10;
  loading = false;
  detailsVisible = false;
  selectedLog: AuditLogModel | null = null;

  constructor(
    private auditLogService: AuditLogService,
    private messageService: MessageService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void { this.loadLogs(); }

  loadLogs(pageNumber = 1, pageSize = this.pageSize): void {
    this.pageSize = pageSize;
    this.loading = true;
    this.auditLogService.getLogs(pageNumber, pageSize).subscribe({
      next: result => {
        this.logs = result.items;
        this.totalRecords = result.totalCount;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: error => {
        this.loading = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Unable to load audit logs',
          detail: error?.error?.message || 'Check that you are signed in with an administrator account.'
        });
        this.cdr.detectChanges();
      }
    });
  }

  onPage(event: TableLazyLoadEvent): void {
    const pageSize = event.rows ?? this.pageSize;
    this.loadLogs(Math.floor((event.first ?? 0) / pageSize) + 1, pageSize);
  }

  showDetails(log: AuditLogModel): void {
    this.selectedLog = log;
    this.detailsVisible = true;
  }

  formatValues(values: string | null): string {
    if (!values) return 'No values recorded';

    try {
      return JSON.stringify(JSON.parse(values), null, 2);
    } catch {
      return values;
    }
  }
}
