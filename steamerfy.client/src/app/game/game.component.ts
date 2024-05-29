import { AnswerData } from './../models/GameHub/answerdata';
import { Player } from '../models/GameHub/player';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { OnInit } from '@angular/core';
import { Question } from '../models/GameHub/question';
import { GameService } from './../game.service';
import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { delay } from 'rxjs';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrl: './game.component.css'
})
export class GameComponent{
  public gs: GameService;
  constructor(GameService: GameService, private snackBar: MatSnackBar, private ActivatedRoute: ActivatedRoute, private router:Router) {
    this.gs = GameService
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
  public question: Question = new Question();
  public showAnswers: boolean = false;
  public AnswerData: AnswerData[] = [];

  public showSidebar: boolean = true;
  public disableButtons: boolean = false;
  public selectedAnswer: number = -1;

  ngOnInit() {
    this.gs.questionStarted$.subscribe((question) => {
      console.log('Question Started: ', question);
      this.question = question;
      this.showAnswers = false;
      this.disableButtons = false;
      this.selectedAnswer = -1;
    });

    this.gs.questionEnded$.subscribe((answerData) => {
      console.log('Question Ended: ', answerData);
      this.AnswerData = answerData;
      this.updatePlayerScores(answerData);
      this.showAnswers = true;
    });

    this.gs.error$.subscribe((error) => {
      this.snackBar.open(error, 'Close', {
        duration: 5000,
        horizontalPosition: 'left',
        verticalPosition: 'bottom'
      });
    });

    this.gs.newGameState$.subscribe((state) => {
      this.question = state.CurrentQuestion;
      if (this.question == null) {
        this.showAnswers = false;
        this.disableButtons = false;
        this.selectedAnswer = -1;
      } else {
        this.showAnswers = new Date(this.question.ExpireTime) >= new Date();
        this.disableButtons = this.showAnswers;
        this.selectedAnswer = -1;
      }
      this.AnswerData = state.AnswerData;
    });
  }
  onAnswer(i: number) {
    console.log('Answered: ', i);
    this.disableButtons = true;
    this.selectedAnswer = i;
    this.gs.answerQuestion(i);
  }

  getPlayerAnswer(player: Player): number | undefined {
    const answer = this.AnswerData.find(a => a.SteamId === player.SteamId);
    return answer ? answer.AnswerId : undefined;
  }

  isPlayerAnswerCorrect(player: Player): boolean {
    const playerAnswer = this.getPlayerAnswer(player);
    return playerAnswer === this.question.Answer;
  }

  updatePlayerScores(answerData: AnswerData[]) {
    this.gs.Players.forEach(player => {
      const playerAnswer = answerData.find(a => a.SteamId === player.SteamId);
      if (playerAnswer) {
        player.Score = playerAnswer.score;
      }
    });
  }
}
