import { CommonModule } from '@angular/common';
import { Component, CUSTOM_ELEMENTS_SCHEMA, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Auth } from '../../../services/authService/auth';
import { AdminService } from '../../../services/adminService/admin-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, FormsModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {

  adminservice = inject(AdminService);
  // console.log(localStorage.getItem('token'));

  constructor(
    private auth: Auth,
    private router: Router
  ){}

  logOut(): void{
    this.auth.logout();
    // this.router.navigate(['/']);
  }


  hideApp:boolean = true;

  toggleApps(){
    this.hideApp = !this.hideApp;
  }

  openHomeScreen(event: MouseEvent){

    event.preventDefault();
    event.stopPropagation();

    const url = this.router.serializeUrl(
      this.router.createUrlTree(['/homeScreen'])
    )

    window.open(window.location.origin + url, '_blank');
  }

  openAdminScreen(event: MouseEvent){

    event.preventDefault();
    event.stopPropagation();

    this.router.navigate(['/adminScreen'])
    // const url = this.router.serializeUrl(
    //   this.router.createUrlTree(['/adminScreen'])
    // )

    // window.open(window.location.origin + url, '_blank');
  }

  openCashScreen(event: MouseEvent){
    event.preventDefault();
    event.stopPropagation();

    const url = this.router.serializeUrl(
      this.router.createUrlTree(['/cashScreen'])
    )

    window.open(window.location.origin + url, '_blank')
  }

  userName: string = '';
  userRole:string = '';
  ngOnInit(){
    const storedData = localStorage.getItem('user') || sessionStorage.getItem('user')
    const userData = storedData ? JSON.parse(storedData) : {};
    this.userName = userData.fullName || 'Guest';
    this.userRole = userData.role || this.adminservice.getRole() || 'noRole';
  }
}
