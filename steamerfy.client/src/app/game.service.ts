import { EventEmitter, Injectable, Output } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Observable, Subject, connect } from 'rxjs';
import { Router } from '@angular/router';
import { Player } from './models/GameHub/player';
import { Question } from './models/GameHub/question';
import { AnswerData } from './models/GameHub/answerdata';
import { GameState } from './models/GameHub/GameState';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private hubConnection!: HubConnection;

  private newGameStateSubject: Subject<GameState> = new Subject<GameState>();

  private questionStartedSubject: Subject<Question> = new Subject<Question>();
  private questionEndedSubject: Subject<AnswerData[]> = new Subject<AnswerData[]>();

  private playerJoinedSubject: Subject<Player> = new Subject<Player>();
  private playerLeftSubject: Subject<Player> = new Subject<Player>();
  private ErrorSubject: Subject<string> = new Subject<string>();

  public connected: boolean = false;
  public connected$: EventEmitter<void> = new EventEmitter();
  public newGameState$: Observable<GameState> = this.newGameStateSubject.asObservable();
  public questionStarted$: Observable<Question> = this.questionStartedSubject.asObservable();
  public questionEnded$: Observable<AnswerData[]> = this.questionEndedSubject.asObservable();
  public playerJoined$: Observable<Player> = this.playerJoinedSubject.asObservable();
  public playerLeft$: Observable<Player> = this.playerLeftSubject.asObservable();
  public error$: Observable<string> = this.ErrorSubject.asObservable();

  private _players: Player[] = [];
  public _userSteamId: string = "";
  public get userSteamId(): string {
    if (this._userSteamId == "") {
      this._userSteamId = localStorage.getItem("steamId") || "";
    }
    return this._userSteamId;
  }
  public set userSteamId(value: string) {
    this._userSteamId = value;
  }
  public hostSteamId: string = "";
  public lobbyId: number = 0;

  constructor(private router: Router) {
    this.initializeSignalRConnection();
  }

  private initializeSignalRConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl("http://localhost:5063/gameHub") // Change the URL to your SignalR hub endpoint
      .configureLogging(LogLevel.Information)
      .build();

    this.hubConnection
      .start()
      .then(() => { this.connected = true; this.connected$.emit()})
      .catch(err => console.log('Error while starting connection: ' + err));

    this.hubConnection.on("InvalidLobby", () => {
      console.log('Invalid lobby');
      this.router.navigate(['/']);
    });

    this.hubConnection.on("QuestionStarted", (questionData: any) => {
      var question: Question = new Question(questionData.QuestionText, questionData.QuestionURL,
        questionData.ImageURLAndOption, questionData.Answer, new Date(questionData.ExpireTime));
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
      player.isUser = player.SteamId === this.userSteamId;
      player.Score = 0;
      this._players.push(player);
      this.playerJoinedSubject.next(player);
    });

    this.hubConnection.on("PlayerLeft", (player: Player) => {
      const playerIndex = this.Players.findIndex(p => p.SteamId === player.SteamId);
      if (playerIndex !== -1) {
        this._players.splice(playerIndex, 1);
      }
      this.playerLeftSubject.next(player);
    });

    this.hubConnection.on("error", (error: string) => {
      this.ErrorSubject.next(error);
    });

    this.hubConnection.on("LobbyJoined", (gameState: GameState) => {
      this.userSteamId = gameState.steamId;
      this.lobbyId = gameState.LobbyId;
      this._players = gameState.Players;
      this.hostSteamId = gameState.HostSteamId;
      this.newGameStateSubject.next(gameState);
    });

    this.hubConnection.on("LobbyCreated", (lobbyId: number) => {
      this.lobbyId = lobbyId;
    });
  }

  async StartQuestion(): Promise<void> {
    console.log('Starting question')
     this.hubConnection.invoke('StartQuestion', this.lobbyId, this.userSteamId);
  }

  async EndQuestion(): Promise<void> {
    try {
      await this.hubConnection.invoke('EndQuestion', this.lobbyId, this.userSteamId);
    } catch (error) {
      console.log('Error while ending question: ', error);
      throw error;
    }
  }

  // returns the id of the lobby created
  async createLobby(SteamId: string): Promise<number> {
    try {
      console.log('Creating lobby with SteamId: ', SteamId);
      await this.hubConnection.invoke('CreateLobby', SteamId);
      this.userSteamId = SteamId;
      return this.lobbyId;
    } catch (error) {
      console.error('Error while creating lobby: ', error);
      throw error;
    }
  }

  async joinLobby(lobbyId: number, steamId: string) {
    try {
      await this.hubConnection.invoke('JoinLobby', lobbyId, steamId);
      this.userSteamId = steamId;
    } catch (error) {
      console.error('Error while joining lobby: ', error);
      throw error;
    }
  }

  async answerQuestion(answerId: number): Promise<void> {
    try {
      await this.hubConnection.invoke('AnswerQuestion', this.lobbyId, this.userSteamId, answerId);
    } catch (error) {
      console.error('Error while answering question: ', error);
      throw error;
    }
  }

  async leaveLobby(lobbyId: number): Promise<void> {
    try {
      await this.hubConnection.invoke('LeaveLobby', lobbyId);
    } catch (error) {
      console.error('Error while leaving lobby: ', error);
      throw error;
    }
  }

  async LoadGameState(lobbyId:number) {
    try {
      await this.hubConnection.invoke('GetGameState', lobbyId);
    } catch (error) {
      console.error('Error while loading game state: ', error);
      throw error;
    }
  }

  public get Players(): Player[] {
    return this._players;
  }

  public isUserHost(): boolean {
    return this._userSteamId === this.hostSteamId;
  }
}
