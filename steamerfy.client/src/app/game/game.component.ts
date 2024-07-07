import { Player } from '../models/GameHub/player';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { Question } from '../models/GameHub/question';
import { GameService } from './../game.service';
import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrl: './game.component.css'
})
export class GameComponent{
  public gs: GameService;
  constructor(GameService: GameService, private snackBar: MatSnackBar, private ActivatedRoute: ActivatedRoute, private router:Router) {
    this.gs = GameService
    if (this.gs.userSteamId === "") {
      this.router.navigate(['/']);
    }
    //Ok so you get the lobby id from the query params, and then if its not connected u subscribe otherswise u just join the lobby
    this.ActivatedRoute.params.subscribe((params: Params) => {
      if (!params['id']) { this.router.navigate(['/']); }
      console.log('Lobby ID: ', params);
      if (!this.gs.connected) {
        this.gs.connected$.subscribe(() => {
          const lobbyId = params['id'];
          this.gs.joinLobby(Number(lobbyId), this.gs.userSteamId)
        });
      }
      else {
        const lobbyId = params['id'];
        this.gs.joinLobby(Number(lobbyId), this.gs.userSteamId)
      }
    })
  }
  public question: Question = new Question("", "", [], -1, new Date());
  public showAnswers: boolean = false;
  public showSidebar: boolean = true;
  public disableButtons: boolean = false;
  public selectedAnswer: number = -1;
  public canStart: boolean = true;
  public canSkip: boolean = false;

  ngOnInit() {
    this.gs.questionStarted$.subscribe((question) => {
      console.log('Question Started: ', question);
      this.canSkip = true; 
      this.question = question || new Question("", "https://www.pngmart.com/files/22/White-Background-PNG.png", [], -1, new Date());
      this.showAnswers = false;
      this.disableButtons = false;
      this.selectedAnswer = -1;
    });

    this.gs.questionEnded$.subscribe((answerData) => {
      console.log('Question Ended: ', answerData);
      this.showAnswers = true;
      this.canSkip = false;
    });

    this.gs.error$.subscribe((error) => {
      this.snackBar.open(error, 'Close', {
        duration: 5000,
        horizontalPosition: 'left',
        verticalPosition: 'bottom'
      });
    });

    this.gs.newGameState$.subscribe((state) => {
      this.question = state.CurrentQuestion || new Question("", "https://www.pngmart.com/files/22/White-Background-PNG.png", [], -1, new Date());
      if (this.question == null) {
        this.showAnswers = false;
        this.disableButtons = false;
        this.selectedAnswer = -1;
      } else {
        this.showAnswers = new Date(this.question.ExpireTime) <= new Date();
        this.disableButtons = this.showAnswers;
        this.selectedAnswer = -1;
      }
    });

    this.gs.GameEnded$.subscribe(() => {
      this.router.navigate(['/end']);
    })
  }
  onAnswer(i: number) {
    console.log('Answered: ', i);
    this.disableButtons = true;
    this.selectedAnswer = i;
    this.gs.answerQuestion(i);
  }

  isPlayerCorrect(player: Player): boolean {
    return player.SelectedAnswer === this.question.Answer;
  }

  StartNextQuestion() {
    if (this.canSkip) {
      this.gs.EndQuestion();
    }
    else if (this.canStart) {
      this.canStart = false;
      setTimeout(() => {
        this.canStart = true;
      }, 3000);
      this.gs.StartQuestion();
    }
  }
}
