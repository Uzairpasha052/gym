import { ChangeDetectorRef, Component, OnInit, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import Swal from 'sweetalert2';

import { Auth } from '../../../../services/authService/auth';
import { registerModel } from '../../../../models/register.model';
import { RoleModel } from '../../../../models/role.model';
import { RoleService } from '../../../../services/role-service/role.service';

@Component({
  selector: 'app-register-popup',
  imports: [FormsModule, CommonModule],
  templateUrl: './register-popup.html',
  styleUrl: './register-popup.scss',
})
export class RegisterPopup implements OnInit {

  // @output() closee = new EventEmitter<void>();

  close = output<void>();
  
  user: registerModel ={
    fullName: '',
    email: '',
    password: '',
    roleId: null,
  }
  roles: RoleModel[] = [];
  rolesLoading = true;
  rolesError = '';

  constructor(
    private auth: Auth,
    private roleService: RoleService,
    private cdr: ChangeDetectorRef
    
  ){}

  ngOnInit(): void {
    this.roleService.getRoles().subscribe({
      next: roles => {
        this.roles = roles;
        this.rolesLoading = false;
        this.cdr.detectChanges();
      },
      error: error => {
        this.rolesError = error.status === 404
          ? 'The roles API was not found. Restart the backend so it loads the roles endpoint.'
          : `Unable to load roles (HTTP ${error.status}). Check the backend connection and your admin login.`;
        this.rolesLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  register(form: NgForm){

    // Stop here if required fields are empty 
    if (form.invalid || this.user.roleId === null || this.rolesLoading || this.roles.length === 0) {
      form.control.markAllAsTouched(); 
      
      Swal.fire({
        icon: 'warning', 
        title: 'Missing Information', 
        text: 'Please fill in all required fields.', 
        confirmButtonText: 'OK', 
        confirmButtonColor: '#3085d6' 
        });
        return; 
    }

    this.auth.register(this.user).subscribe({
      next: (response) => {
            // console.log('User',this.user);
            // console.log('User Registered', response);

            Swal.fire({
              icon: 'success',
              title: 'User Added!',
              text: 'The User has been registered successfully.',
              confirmButtonText: 'Done',
              confirmButtonColor: '#3085d6'
            }).then(() => {
              this.close.emit();
            });
        
      },

      error: (err) => {
        // console.error(err);

        Swal.fire({
        icon: 'error',
        title: 'Registration Failed',
        text: err.error?.message || 'Something went wrong. Please try again.',
        confirmButtonText: 'OK',
        confirmButtonColor: '#d33'
      });
      }
    })

  }

  closePopup(){
    this.close.emit();
  }
}
