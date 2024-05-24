import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';

import { LobbyCreationResponse, LobbyJoinResponse, GameEndedResponse, LeaveLobbyResponse } from './models/GameHub/responses';
import { Player } from './models/GameHub/player';
import { Question } from './models/GameHub/question';
import { AnswerData } from './models/GameHub/answerdata';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private hubConnection!: HubConnection;
  private questionStartedSubject: Subject<Question> = new Subject<Question>();
  private questionEndedSubject: Subject<AnswerData[]> = new Subject<AnswerData[]>();
  private playerJoinedSubject: Subject<Player> = new Subject<Player>();
  private playerLeftSubject: Subject<Player> = new Subject<Player>();

  public questionStarted$: Observable<Question> = this.questionStartedSubject.asObservable();
  public questionEnded$: Observable<AnswerData[]> = this.questionEndedSubject.asObservable();
  public playerJoined$: Observable<Player> = this.playerJoinedSubject.asObservable();
  public playerLeft$: Observable<Player> = this.playerLeftSubject.asObservable();

  public userSteamId: string = "";
  public lobbyId: number = 0;
  public Players: Player[] = [];

  constructor() {
    this.initializeSignalRConnection();
  }

  private initializeSignalRConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl("https://your-api-url/gameHub") // Change the URL to your SignalR hub endpoint
      .configureLogging(LogLevel.Information)
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err));

    this.hubConnection.on("QuestionStarted", (questionData: any) => {
      const question: Question = {
        questionText: questionData.questionText,
        imageURLAndOption: questionData.imageURLAndOption,
        answer: questionData.answer,
        expireTime: new Date(questionData.expireTime)
      };
      this.questionStartedSubject.next(question);
    });

    this.hubConnection.on("QuestionEnded", (answerData: any[]) => {
      const parsedAnswerData: AnswerData[] = answerData.map(data => ({
        SteamId: data[0],
        AnswerId: parseInt(data[1], 10),
        score: parseInt(data[2], 10)
      }));
      this.questionEndedSubject.next(parsedAnswerData);
    });

    this.hubConnection.on("PlayerJoined", (player: Player) => {
      this.Players.push(player);
      this.playerJoinedSubject.next(player);
    });

    this.hubConnection.on("PlayerLeft", (player: Player) => {
      const playerIndex = this.Players.findIndex(p => p.steamId === player.steamId);
      if (playerIndex !== -1) {
        this.Players.splice(playerIndex, 1);
      }
      this.playerLeftSubject.next(player);
    });
  }

  // returns the id of the lobby created
  async createLobby(SteamId: string): Promise<number> {
    try {
      const response = await this.hubConnection.invoke<LobbyCreationResponse>('CreateLobby', SteamId);
      return response.lobbyId;
    } catch (error) {
      console.error('Error while creating lobby: ', error);
      throw error;
    }
  }

  async joinLobby(lobbyId: number, steamId: string): Promise<Player> {
    try {
      const response = await this.hubConnection.invoke<LobbyJoinResponse>('JoinLobby', lobbyId, steamId);
      return response.player;
    } catch (error) {
      console.error('Error while joining lobby: ', error);
      throw error;
    }
  }

  async answerQuestion(lobbyId: number, playerId: string, answerId: number): Promise<void> {
    try {
      await this.hubConnection.invoke('AnswerQuestion', lobbyId, playerId, answerId);
    } catch (error) {
      console.error('Error while answering question: ', error);
      throw error;
    }
  }

  async endGame(lobbyId: number): Promise<AnswerData[]> {
    try {
      const response = await this.hubConnection.invoke<GameEndedResponse>('EndGame', lobbyId);
      return response.answerData;
    } catch (error) {
      console.error('Error while ending game: ', error);
      throw error;
    }
  }

  async leaveLobby(lobbyId: number): Promise<void> {
    try {
      await this.hubConnection.invoke<LeaveLobbyResponse>('LeaveLobby', lobbyId);
    } catch (error) {
      console.error('Error while leaving lobby: ', error);
      throw error;
    }
  }
}
