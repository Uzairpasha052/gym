import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-cash-flow',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet, ToastModule],
  templateUrl: './cash-flow.html',
  styleUrl: './cash-flow.scss',
  providers: [MessageService]
})
export class CashFlow {}
