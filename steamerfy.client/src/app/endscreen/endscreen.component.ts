import { Component, OnInit } from '@angular/core';
import { GameService } from '../game.service';
import { Player } from '../models/GameHub/player';
import { Router } from '@angular/router';

@Component({
  selector: 'app-endscreen',
  templateUrl: './endscreen.component.html',
  styleUrl: './endscreen.component.css'
})
export class EndScreenComponent implements OnInit {

  constructor(public gs: GameService, private router: Router) { }

  public podiumPlayers: Player[] = [];

  ngOnInit() {
    if (!this.gs.lobbyId) {
      this.returnToLobby()
    }
    this.podiumPlayers = [...this.gs.Players].sort((a, b) => b.Score - a.Score).slice(0, 3);
  }

  public returnToLobby() {
    this.router.navigate(['/lobby']);
  }
}
