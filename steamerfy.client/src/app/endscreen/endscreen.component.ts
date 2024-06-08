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

  public players: Player[] = [
    new Player('Jim', '', 'https://ih1.redbubble.net/image.1014103287.0145/flat,750x,075,f-pad,750x1000,f8f8f8.jpg', '', true, true, -1, 9),
    new Player('Bob', '', 'https://ih1.redbubble.net/image.1014102848.0108/bg,f8f8f8-flat,750x,075,f-pad,750x1000,f8f8f8.jpg', '', true, true, -1, 10),
    new Player('Joe', '', 'https://aidwiki.com/images/wiki/origin/2021_remy_mars_aidwiki_1.jpg', '', true, true, -1, 8)
  ];

  public podiumPlayers: Player[] = [];

  ngOnInit() {
    this.podiumPlayers = [...this.players].sort((a, b) => b.Score - a.Score).slice(0, 3);
  }

  public returnToLobby() {
    this.router.navigate(['/lobby']);
  }
}
