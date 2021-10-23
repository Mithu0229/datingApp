import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../models/User';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
baseUrl= environment.apiUrl ;

 private currentUserSource = new ReplaySubject<User>(1); //like buffer object
 currentUser$=this.currentUserSource.asObservable(); //$ for observable
  constructor(private http: HttpClient) { }
  
  login(model:any){
    return this.http.post<User>(this.baseUrl+'account/login',model).pipe(
      map((response: User) => { // set user interface but problem this problem solve by post<User> use it
      const user=response as User;
      if(user){
        this.setCurrentUser(user);
      }
    }))
  }
  
  register(model :any){
    return this.http.post<User>(this.baseUrl+'account/register',model).pipe(
      map((response: User) => { // set user interface but problem this problem solve by post<User> use it
      const user=response as User;
      if(user){
        this.setCurrentUser(user);
      }
     
    }))
  }

  setCurrentUser(user:User){
    localStorage.setItem('user',JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(undefined); // set null but problem
  }
}
