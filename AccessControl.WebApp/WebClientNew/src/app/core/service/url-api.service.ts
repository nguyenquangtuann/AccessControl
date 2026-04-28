import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UrlApiService {
  getUrlApi(): string {
    let filePath: string = 'assets/config/urlApi.json';
    let result: any;
    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", filePath, false);
    rawFile.onreadystatechange = function () {
      if (rawFile.readyState === 4) {
        if (rawFile.status === 200 || rawFile.status == 0) {
          result = JSON.parse(rawFile.responseText).url_api + 'api/access/';
        }
      }
    }
    rawFile.send(null);
    return result;
  }
}
