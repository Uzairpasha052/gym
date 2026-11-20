import { Component } from '@angular/core';
import { LoginModel } from '../../models/login.model';
import { Auth } from '../../services/authService/auth';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-n-login',
  imports: [FormsModule, CommonModule],
  templateUrl: './n-login.html',
  styleUrl: './n-login.scss',
})
export class NLogin {

  n_loginData: LoginModel ={
    email: '',
    password: '',
    rememberMe: false,
  }

  errMsg = "";
  sucmsg = "";

  constructor(
    private auth: Auth,
    private router: Router,
  ){}

  login(){
    this.auth.login(this.n_loginData).subscribe({
      next: (response) => {
        // localStorage.setItem('user', JSON.stringify(response));
        const userData = JSON.stringify(response)

        if(this.n_loginData.rememberMe){
          localStorage.setItem('user', userData)
        }
        else{
          sessionStorage.setItem('user', userData)
        }

        this.sucmsg = 'Login Success'
        this.router.navigate(['/users'])
      },
      error: (error) => {
        // console.error(error);
        this.errMsg = error.error.message ?? "Login Failed"
      }
    })
  }
}
