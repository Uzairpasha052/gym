import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { ExpiringMember } from '../../../models/membership/membership.model';
import { MembershipService } from '../../../services/membershipService/membership.service';

@Component({
  selector: 'app-expiring-members',
  standalone: true,
  imports: [CommonModule, DatePipe, TableModule, ToastModule],
  providers: [MessageService],
  templateUrl: './expiring-members.html',
  styleUrl: './expiring-members.scss'
})
export class ExpiringMembers implements OnInit {
  members: ExpiringMember[] = [];
  totalRecords = 0;
  rows = 10;
  loading = false;
  readonly reminderDays = 7;

  constructor(private memberships: MembershipService, private messages: MessageService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void { this.load(1, this.rows); }

  onPage(event: TableLazyLoadEvent): void {
    const rows = event.rows ?? this.rows;
    this.load(Math.floor((event.first ?? 0) / rows) + 1, rows);
  }

  private load(page: number, rows: number): void {
    this.loading = true;
    this.rows = rows;
    this.memberships.getExpiringMembers(this.reminderDays, page, rows).subscribe({
      next: result => { this.members = result.items; this.totalRecords = result.totalCount; this.loading = false; this.cdr.detectChanges(); },
      error: error => {
        this.loading = false;
        this.messages.add({ severity: 'error', summary: 'Error', detail: error?.error?.message || 'Unable to load expiring memberships.' });
        this.cdr.detectChanges();
      }
    });
  }
}
