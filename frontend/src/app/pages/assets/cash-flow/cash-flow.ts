import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { SidebarMenuItem } from '../../../models/sidebar-menu.model';
import { SidebarService } from '../../../services/sidebarService/sidebar.service';

@Component({
  selector: 'app-cash-flow',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet, ToastModule],
  templateUrl: './cash-flow.html',
  styleUrl: './cash-flow.scss',
  providers: [MessageService]
})
export class CashFlow implements OnInit {
  cashMenus: SidebarMenuItem[] = [];
  loadingMenu = true;
  menuError = '';

  constructor(private sidebar: SidebarService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.sidebar.getParentMenus().subscribe({
      next: parents => {
        const cashParent = parents.find(menu => menu.route.trim().toLowerCase() === '/cash');
        if (!cashParent) {
          this.cashMenus = [];
          this.loadingMenu = false;
          this.cdr.detectChanges();
          return;
        }

        this.sidebar.getChildMenus(cashParent.id).subscribe({
          next: items => {
            this.cashMenus = items;
            this.loadingMenu = false;
            this.cdr.detectChanges();
          },
          error: error => {
            this.menuError = error?.error?.message || 'Unable to load cash navigation.';
            this.loadingMenu = false;
            this.cdr.detectChanges();
          }
        });
      },
      error: error => {
        this.menuError = error?.error?.message || 'Unable to load cash navigation.';
        this.loadingMenu = false;
        this.cdr.detectChanges();
      }
    });
  }
}
