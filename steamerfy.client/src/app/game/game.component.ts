import { AnswerData } from './../models/GameHub/answerdata';
import { Player } from '../models/GameHub/player';
import { Question } from '../models/GameHub/question';
import { GameService } from './../game.service';
import { Component } from '@angular/core';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrl: './game.component.css'
})
export class GameComponent {
  private _gameService: GameService;
  constructor(GameService: GameService) {
    this._gameService = GameService
  }
  public question: Question = new Question();
  public players: Player[] = [new Player("Barthalamule the third", "", "https://upload.wikimedia.org/wikipedia/commons/0/05/Cow-bw.JPG", "asdfa", false, false, 13),
                            new Player("Bob", "", "https://upload.wikimedia.org/wikipedia/commons/0/05/Cow-bw.JPG", "asdfa", false, true, 13),
    new Player("Bartha", "", "https://c8.alamy.com/comp/FEN55R/close-up-of-raccoon-FEN55R.jpg", "asdfa", true, true, 13)];
  public showAnswers: boolean = false;
  public AnswerData: AnswerData[] = [];

  ngOnInit() {
    this._gameService.questionStarted$.subscribe((question) => {
      console.log('Question Started: ', question);
      this.question = question;
      this.showAnswers = false;
    });

    this._gameService.questionEnded$.subscribe((answerData) => {
      console.log('Question Ended: ', answerData);
      this.AnswerData = answerData;
      this.showAnswers = true;
    });

    this._gameService.playerJoined$.subscribe((player) => {
      console.log('Player Joined: ', player);
      this.players.push(player);
    });

    this._gameService.playerLeft$.subscribe((player) => {
      console.log('Player Left: ', player);
      this.players = this.players.filter(p => p.steamId !== player.steamId);
    });
  }
}
