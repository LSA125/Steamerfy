import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

import { LobbyCreationResponse, LobbyJoinResponse, QuestionResponse, PlayerAnswerResponse, NextQuestionResponse, GameEndedResponse, LeaveLobbyResponse } from './models/GameHub/responses';
import { Player } from './models/GameHub/player';
import { Question } from './models/GameHub/question';
import { Lobby } from './models/GameHub/lobby';


@Injectable({
  providedIn: 'root'
})
export class GameService {
  private hubConnection: HubConnection | undefined;

  constructor() {
    this.initializeSignalRConnection();
  }

  private initializeSignalRConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://example.com/gamehub') // Replace with your SignalR endpoint
      .build();

    this.hubConnection.start()
      .then(() => console.log('SignalR connection established'))
      .catch(err => console.error('Error while establishing SignalR connection: ', err));
  }

  //returns the id of the lobby created
  async createLobby(): Promise<number | undefined> {
    try {
      return (await this.hubConnection?.invoke<LobbyCreationResponse>('CreateLobby'))?.lobbyId;
    } catch (error) {
      console.error('Error while creating lobby: ', error);
      throw error;
    }
  }

  async joinLobby(lobbyId: number, steamId: string): Promise<Player | undefined> {
    try {
      return (await this.hubConnection?.invoke<LobbyJoinResponse>('JoinLobby', lobbyId, steamId))?.player;
    } catch (error) {
      console.error('Error while joining lobby: ', error);
      throw error;
    }
  }

  async getQuestion(lobbyId: number): Promise<Question | undefined> {
    try {
      return (await this.hubConnection?.invoke<QuestionResponse>('GetQuestion', lobbyId))?.question;
    } catch (error) {
      console.error('Error while getting question: ', error);
      throw error;
    }
  }

  async answerQuestion(lobbyId: number, playerId: string, answerId: number): Promise<number | undefined> {
    try {
      return (await this.hubConnection?.invoke<PlayerAnswerResponse>('AnswerQuestion', lobbyId, playerId, answerId))?.score;
    } catch (error) {
      console.error('Error while answering question: ', error);
      throw error;
    }
  }

  async nextQuestion(lobbyId: number): Promise<Question | undefined> {
    try {
      return (await this.hubConnection?.invoke<NextQuestionResponse>('NextQuestion', lobbyId))?.question;
    } catch (error) {
      console.error('Error while moving to next question: ', error);
      throw error;
    }
  }

  async endGame(lobbyId: number): Promise<Lobby | undefined> {
    try {
      return (await this.hubConnection?.invoke<GameEndedResponse>('EndGame', lobbyId))?.lobby;
    } catch (error) {
      console.error('Error while ending game: ', error);
      throw error;
    }
  }

  async leaveLobby(lobbyId: number): Promise<LeaveLobbyResponse | undefined> {
    try {
      return await this.hubConnection?.invoke<LeaveLobbyResponse>('LeaveLobby', lobbyId);
    } catch (error) {
      console.error('Error while leaving lobby: ', error);
      throw error;
    }
  }
}
