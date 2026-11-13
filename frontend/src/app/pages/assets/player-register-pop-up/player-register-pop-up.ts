import { Component, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import Swal from 'sweetalert2';

import { PlayerService } from '../../../services/playerService/player';
import { CreatePlayerRequest } from '../../../models/player/player.model';

@Component({
  selector: 'app-player-register-pop-up',
  imports: [CommonModule, FormsModule],
  templateUrl: './player-register-pop-up.html',
  styleUrl: './player-register-pop-up.scss',
})
export class PlayerRegisterPopUp {

  close = output<void>();
  
  player: CreatePlayerRequest = {
    name: '',
    fName: '',
    age: '',
    gender: '',
    contact: '',
    history: '',
    reason: '',
    fee: '',
  }

  constructor(
    private auth: PlayerService
    
  ){}

  register(form: NgForm){

    // Stop here if required fields are empty 
    if (form.invalid) {
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

    this.auth.createPlayer(this.player).subscribe({
      next: (response) => {
            Swal.fire({
              icon: 'success',
              title: 'Player Added!',
              text: 'The player has been registered successfully.',
              confirmButtonText: 'Done',
              confirmButtonColor: '#3085d6'
            }).then(() => {
              this.close.emit();
            });

        
        // alert("Player Added Successfully")
        // this.close.emit();
        
      },

      error: (err) => {
        // console.error(err);
        // alert(err.error.message);
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
