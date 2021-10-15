import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../models/User';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  
  model:any={}
 
  constructor(public accountservice: AccountService, private router: Router,
    private toastr: ToastrService) { }
  ngOnInit(): void {
  
  }
  login(){
    this.accountservice.login(this.model).subscribe(response=>{
      console.log(response);
      this.router.navigateByUrl('/members');
    },error=>{
      this.toastr.error(error.error);
      console.log(error);
      
    })
    console.log(this.model);
    
  }
  Logout(){

  this.accountservice.logout();
  this.router.navigateByUrl('/');
  }

  

}
