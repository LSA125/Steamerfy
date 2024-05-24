import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { GameService } from '../game.service';
import { Player } from '../models/GameHub/player';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.css']
})
export class LobbyComponent implements OnInit {
  username: string = "";
  steamId: string = "";
  lobbyId: string = "";

  constructor(private _gs: GameService, private router: Router, private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    // Retrieve username and steamId from localStorage if available
    this.username = localStorage.getItem('username') || '';
    this.steamId = localStorage.getItem('steamId') || '';
  }

  createLobby(): void {
    this._gs.createLobby(this.steamId).then((lobbyId: number) => {
      console.log('Lobby created: ', lobbyId);
      this.router.navigate([`/game/${lobbyId}`]);
    }).catch((error) => {
      console.error('Error while creating lobby: ', error);
      this.snackBar.open('Error while creating lobby', 'Close', { duration: 3000 });
    });

    console.log('Create Lobby clicked');
  }

  joinLobby(): void {
    this._gs.joinLobby(+this.lobbyId, this.steamId).then((player: Player) => {
      console.log('Joined lobby: ', this.lobbyId);
      this.router.navigate([`/game/${this.lobbyId}`]);
    }).catch((error) => {
      console.error('Error while joining lobby: ', error);
      this.snackBar.open('Error while joining lobby', 'Close', { duration: 3000 });
    });

    console.log('Join Lobby clicked');
  }
}
