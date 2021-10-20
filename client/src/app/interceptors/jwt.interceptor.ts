import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountService } from '../services/account.service';
import { User } from '../models/User';
import { take } from 'rxjs/operators';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private acountservice: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let currentser! : User;
    this.acountservice.currentUser$.pipe(take(1)).subscribe(user=> currentser=user);
    if(currentser){
      request =request.clone({
        setHeaders :{
          Authorization: `Bearer ${currentser.token}`
        }
      })
    }
    return next.handle(request);
  }
}
