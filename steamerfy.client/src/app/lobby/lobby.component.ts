import { Component } from '@angular/core';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrl: './lobby.component.css'
})
export class LobbyComponent {
  username: string = "";
  steamId: string = "";

  constructor() { }

  ngOnInit(): void {
    // Retrieve username and steamId from localStorage if available
    this.username = localStorage.getItem('username') || '';
    this.steamId = localStorage.getItem('steamId') || '';
  }

  createLobby(): void {
    // Store username and steamId in localStorage
    localStorage.setItem('username', this.username);
    localStorage.setItem('steamId', this.steamId);
    // Logic for creating a lobby
    console.log('Create Lobby clicked');
  }

  joinLobby(): void {
    // Logic for joining a lobby
    localStorage.setItem('username', this.username);
    localStorage.setItem('steamId', this.steamId);
    console.log('Join Lobby clicked');
  }
}
