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
export class LobbyComponent {
  username: string = "";
  public steamId: string = "";
  public lobbyId: string = "";

  constructor(private _gs: GameService, private router: Router, private snackBar: MatSnackBar) {
    if (this._gs.connected) {
      this._gs.leaveLobby(this._gs.lobbyId)
    }
  }

  createLobby(): void {
    this._gs.createLobby(this.steamId).then((lobbyId: number) => {
      console.log('Lobby created: ', Number(lobbyId));
      this.lobbyId = lobbyId.toString();
      this.joinLobby();
    }).catch((error) => {
      console.error('Error while creating lobby: ', error);
      this.snackBar.open('Error while creating lobby', 'Close', { duration: 3000 });
    });
  }

  joinLobby(): void {
    this._gs.userSteamId = this.steamId;
    this.router.navigate(['/game', this.lobbyId]);
  }
}
