import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UrlApiService } from './url-api.service';
import { AuthenService } from './authen.service';
import { map, timeout } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  constructor(private http: HttpClient, private authenService: AuthenService, private config: UrlApiService) { }

  get(url: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*').delete("Authorization").append("Authorization", this.authenService.getLoggedInUser().access_token);
    return this.http
      .get(this.config.getUrlApi() + url, { headers: headers })
      .pipe(map(data => data));
  }

  getTimeout(url: string, ms: number) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*').delete("Authorization").append("Authorization", this.authenService.getLoggedInUser().access_token);
    return this.http
      .get(this.config.getUrlApi() + url, { headers: headers })
      .pipe(map(data => data), timeout(ms));
  }

  post(uri: string, data?: any) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*').delete("Authorization").append("Authorization", this.authenService.getLoggedInUser().access_token);
    return this.http
      .post(this.config.getUrlApi() + uri, data, { headers: headers })
      .pipe(map(data => data));
  }

  put(uri: string, data?: any) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('responseType', 'text')
      .set('Access-Control-Allow-Origin', '*').delete("Authorization").append("Authorization", this.authenService.getLoggedInUser().access_token);
    return this.http
      .put(this.config.getUrlApi() + uri, data, { headers: headers })
      .pipe(map(data => data));
  }

  delete(uri: string, key: string, id: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*').delete("Authorization").append("Authorization", this.authenService.getLoggedInUser().access_token);
    return this.http
      .delete(`${this.config.getUrlApi()}${uri}/?${key}=${id}`, { headers: headers })
      .pipe(map(data => data));
  }

  deleteData(uri: string, data?: any) {
    let headers = new HttpHeaders().set('content-type', 'application/json').set('Access-Control-Allow-Origin', '*').delete("Authorization").append("Authorization", this.authenService.getLoggedInUser().access_token);
    return this.http
      .delete(this.config.getUrlApi() + uri, { headers: headers, body: data })
      .pipe(map(data => data));
  }

  lockMulti(uri: string, id: string, userId: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .delete("Authorization")
      .append("Authorization", this.authenService.getLoggedInUser().access_token);
    const url = `${this.config.getUrlApi()}${uri}?checkedList=${id}&userId=${userId}`;
    return this.http.delete(url, { headers: headers }).pipe(map(data => data));
  }

  deleteById(uri: string) {
    let headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*').delete("Authorization").append("Authorization", this.authenService.getLoggedInUser().access_token);
    return this.http
      .delete(this.config.getUrlApi() + uri, { headers: headers })
      .pipe(map(data => data));
  }

  postFile(uri: string, data?: any) {
    const fileHeaders = new HttpHeaders().set(
      'Authorization',
      this.authenService.getLoggedInUser()?.access_token || '');
    return this.http
      .post(this.config.getUrlApi() + uri, data, { headers: fileHeaders })
      .pipe(map(data => data));
  }
}
