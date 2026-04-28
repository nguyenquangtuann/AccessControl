import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { UrlApiService } from './url-api.service';
import { SystemConstants } from '../common/system.constants';
import { LoggedInUser } from '../common/loggedIn.user';
import { jwtDecode } from 'jwt-decode';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';

@Injectable({
  providedIn: 'root'
})

export class AuthenService {
  user: any = null;
  stringArray = new BehaviorSubject<string[]>([]);
  constructor(private http: HttpClient, private urlApi: UrlApiService, private router: Router) { }

  login(data: any) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*');

    return this.http.post(this.urlApi.getUrlApi() + 'AccountLogin/login', data, { 'headers': headers })
      .pipe(map(res => {
        this.user = res;
        if (this.user) {
          localStorage.removeItem(SystemConstants.CURRENT_USER);
          localStorage.removeItem(SystemConstants.CURRENT_USER_ROLE);
          localStorage.setItem(SystemConstants.CURRENT_USER, JSON.stringify(this.user));
          this.userRoleValue();
        }
      }));
  }

  logOutAuto() {
    this.router.navigate(['/login']);
  }

  logOut() {
    localStorage.removeItem(SystemConstants.CURRENT_USER);
    localStorage.removeItem(SystemConstants.CURRENT_USER_ROLE);
    localStorage.removeItem(SystemConstants.USERS_PIPE);
    localStorage.removeItem(SystemConstants.USER_MENUS);
    localStorage.removeItem(SystemConstants.DEP_ID_LIST);
    localStorage.removeItem('checked');
    this.router.navigate(['/login']);
  }

  userRoleValue() {
    const id = this.getLoggedInUser().id;
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*').delete("Authorization").append("Authorization", this.getLoggedInUser().access_token);
    this.http.get(this.urlApi.getUrlApi() + 'AppUserRole/getuserroleid?userId=' + id, { 'headers': headers }).subscribe((val: any) => {
      localStorage.removeItem(SystemConstants.CURRENT_USER_ROLE);
      localStorage.setItem(SystemConstants.CURRENT_USER_ROLE, JSON.stringify(val));
    });
  }

  get UserRole(): string[] {
    return JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER_ROLE) as string);
  }

  isUserAuthenticated(): boolean {
    const userStr = localStorage.getItem(SystemConstants.CURRENT_USER);

    if (!userStr) return false;

    try {
      const user = JSON.parse(userStr);

      if (!user.access_token) return false;

      const token = user.access_token.replace('Bearer ', '');

      return !this.isTokenExpiredRaw(token);
    } catch {
      return false;
    }
  }

  isTokenExpiredRaw(token: string): boolean {
    try {
      const decoded: any = jwtDecode(token);

      if (!decoded.exp) return false;

      const now = Math.floor(Date.now() / 1000);

      return decoded.exp < now;
    } catch {
      return true;
    }
  }

  getLoggedInUser(): LoggedInUser {
    let user: LoggedInUser | null;
    if (this.isUserAuthenticated()) {
      var userData = JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER) as string);
      user = new LoggedInUser(userData.id, userData.access_token, userData.userName, userData.fullName, userData.email, userData.phone, userData.image);
    }
    else {
      user = null;
    }
    return user!;
  }

  // check hết hạn token
  getTokenExpireTime(): number | null {
    try {
      const userStr = localStorage.getItem(SystemConstants.CURRENT_USER);
      if (!userStr) return null;

      const user = JSON.parse(userStr);

      if (!user.access_token) return null;

      const token = user.access_token.replace('Bearer ', '');
      const decoded: any = jwtDecode(token);

      return decoded.exp * 1000;
    } catch {
      return null;
    }
  }

  setupAutoLogout(callback: () => void) {
    const expireTime = this.getTokenExpireTime();

    if (!expireTime) return;

    const now = Date.now();
    const timeout = expireTime - now;

    if (timeout <= 0) {
      callback();
      return;
    }

    setTimeout(() => {
      callback();
    }, timeout);
  }
}
