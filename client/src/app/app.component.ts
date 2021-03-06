import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './models/User';
import { AccountService } from './services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'The dating App';
  users: any;
  constructor( private accountService : AccountService) { }
  
  ngOnInit() {
 
    this.setCurrentUser();
  }

  setCurrentUser(){ //same name in service class must be
    const getUserStore=localStorage.getItem('user')!;
    const usr=JSON.parse(getUserStore);
   // const user: User = JSON.parse(getUserStore !== null ?JSON.parse(getUserStore) :{}); //problem but it work
    this.accountService.setCurrentUser(usr);
  }

 
}
