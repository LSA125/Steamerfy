import { Injectable } from '@angular/core';
import signalR, { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

import { LobbyCreationResponse, LobbyJoinResponse, GameEndedResponse, LeaveLobbyResponse } from './models/GameHub/responses';
import { Player } from './models/GameHub/player';
import { Question } from './models/GameHub/question';
import { Lobby } from './models/GameHub/lobby';
import { Observable, Subject } from 'rxjs';
import { AnswerData } from './models/GameHub/answerdata';


@Injectable({
  providedIn: 'root'
})
export class GameService {
  private hubConnection!: signalR.HubConnection;
  private questionStartedSubject: Subject<Question> = new Subject<Question>();
  private questionEndedSubject: Subject<AnswerData[]> = new Subject<AnswerData[]>();

  public questionStarted$: Observable<Question> = this.questionStartedSubject.asObservable();
  public questionEnded$: Observable<AnswerData[]> = this.questionEndedSubject.asObservable();

  constructor() {
    this.initializeSignalRConnection();
  }

  private initializeSignalRConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl("https://your-api-url/gameHub") // Change the URL to your SignalR hub endpoint
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
        AnswerId: data[1],
        score: data[2]
      }));
      this.questionEndedSubject.next(parsedAnswerData);
    });
  }

  //returns the id of the lobby created
  async createLobby(SteamId: string): Promise<number> {
    try {
      return (await this.hubConnection?.invoke<LobbyCreationResponse>('CreateLobby', SteamId))?.lobbyId;
    } catch (error) {
      console.error('Error while creating lobby: ', error);
      throw error;
    }
  }

  async joinLobby(lobbyId: number, steamId: string): Promise<Player> {
    try {
      return (await this.hubConnection?.invoke<LobbyJoinResponse>('JoinLobby', lobbyId, steamId))?.player;
    } catch (error) {
      console.error('Error while joining lobby: ', error);
      throw error;
    }
  }

  async answerQuestion(lobbyId: number, playerId: string, answerId: number){
    try {
      return (await this.hubConnection?.invoke('AnswerQuestion', lobbyId, playerId, answerId));
    } catch (error) {
      console.error('Error while answering question: ', error);
      throw error;
    }
  }

  async endGame(lobbyId: number): Promise<Lobby> {
    try {
      return (await this.hubConnection?.invoke<GameEndedResponse>('EndGame', lobbyId))?.lobby;
    } catch (error) {
      console.error('Error while ending game: ', error);
      throw error;
    }
  }

  async leaveLobby(lobbyId: number): Promise<LeaveLobbyResponse> {
    try {
      return await this.hubConnection?.invoke<LeaveLobbyResponse>('LeaveLobby', lobbyId);
    } catch (error) {
      console.error('Error while leaving lobby: ', error);
      throw error;
    }
  }
}
