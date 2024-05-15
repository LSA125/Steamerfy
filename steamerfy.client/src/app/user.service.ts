import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  steamId: string = '';
  constructor() { }

  // Store username and steamId in localStorage
  storeUserData(steamId: string): void {
    localStorage.setItem('steamId', steamId);
  }

  // Retrieve username and steamId from localStorage if available
  getUserData(): string {
    return this.steamId == '' ? localStorage.getItem('steamId') || '' : this.steamId;
  }



}
