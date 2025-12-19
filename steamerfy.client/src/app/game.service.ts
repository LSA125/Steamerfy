import { EventEmitter, Injectable, isDevMode } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';
import { Router } from '@angular/router';
import { Player } from './models/GameHub/player';
import { Question } from './models/GameHub/question';
import { GameState } from './models/GameHub/GameState';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private hubConnection!: HubConnection;

  private newGameStateSubject: Subject<GameState> = new Subject<GameState>();

  private questionStartedSubject: Subject<Question> = new Subject<Question>();
  private questionEndedSubject: Subject<void> = new Subject<void>();

  private playerJoinedSubject: Subject<Player> = new Subject<Player>();
  private playerLeftSubject: Subject<Player> = new Subject<Player>();
  private ErrorSubject: Subject<string> = new Subject<string>();
  private GameEndedSubject: Subject<void> = new Subject<void>()

  public connected: boolean = false;
  public connected$: EventEmitter<void> = new EventEmitter();
  public newGameState$: Observable<GameState> = this.newGameStateSubject.asObservable();
  public questionStarted$: Observable<Question> = this.questionStartedSubject.asObservable();
  public questionEnded$: Observable<void> = this.questionEndedSubject.asObservable();
  public playerJoined$: Observable<Player> = this.playerJoinedSubject.asObservable();
  public playerLeft$: Observable<Player> = this.playerLeftSubject.asObservable();
  public GameEnded$: Observable<void> = this.GameEndedSubject.asObservable();
  public error$: Observable<string> = this.ErrorSubject.asObservable();

  private _players: Player[] = [];
  public userSteamId: string = "";

  public hostSteamId: string = "";
  public lobbyId: number = 0;

  constructor(private router: Router) {
    this.initializeSignalRConnection();
  }

  private log(message: string, ...args: unknown[]): void {
    if (isDevMode()) {
      console.log(message, ...args);
    }
  }

  private logError(message: string, ...args: unknown[]): void {
    if (isDevMode()) {
      console.error(message, ...args);
    }
  }

  private initializeSignalRConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(environment.signalRHubUrl)
      .withAutomaticReconnect()
      .configureLogging(isDevMode() ? LogLevel.Information : LogLevel.Warning)
      .build();

    this.hubConnection
      .start()
      .then(() => { this.connected = true; this.connected$.emit()})
      .catch(err => this.logError('Error while starting connection: ' + err));

    this.hubConnection.on("InvalidLobby", () => {
      this.log('Invalid lobby');
      this.router.navigate(['/']);
    });

    this.hubConnection.on("HostChanged", (hostSteamId: string) => {
      this.hostSteamId = hostSteamId;
      this._players.forEach(player => {
        player.isHost = player.SteamId === hostSteamId;
      });
    });

    this.hubConnection.on("QuestionStarted", (questionData: unknown) => {
      const data = questionData as { QuestionText: string; QuestionURL: string; ImageURLAndOption: [string, string][]; Answer: number; ExpireTime: string };
      const question: Question = new Question(data.QuestionText, data.QuestionURL,
        data.ImageURLAndOption, data.Answer, new Date(data.ExpireTime));
      this.questionStartedSubject.next(question);
    });

    this.hubConnection.on("QuestionEnded", (answerData: string[][]) => {
      this._players.forEach(player => {
        const playerAnswer = answerData.find(data => data[0] === player.SteamId);
        if (playerAnswer) {
          player.Score = parseInt(playerAnswer[2], 10);
          player.SelectedAnswer = parseInt(playerAnswer[1], 10);
        }
      });

      this.questionEndedSubject.next();
    });

    this.hubConnection.on("PlayerJoined", (player: Player) => {
      player.isUser = player.SteamId == this.userSteamId;
      player.isHost = player.SteamId == this.hostSteamId;
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
      this.router.navigate(['/']);
      this.ErrorSubject.next(error);
    });

    this.hubConnection.on("LobbyJoined", (gameState: GameState) => {
      this.lobbyId = gameState.LobbyId;
      this._players = gameState.Players;
      this.hostSteamId = gameState.HostSteamId;
      this.router.navigate(['/game', this.lobbyId]);
      const currentUser = this._players.find(p => p.SteamId === this.userSteamId);
      if (currentUser) {
        currentUser.isUser = true;
      }
      const host = this._players.find(p => p.SteamId === this.hostSteamId);
      if (host) {
        host.isHost = true;
      }
      this.newGameStateSubject.next(gameState);
    });

    this.hubConnection.on("LobbyCreated", (lobbyId: number) => {
      this.lobbyId = lobbyId;
    });
    this.hubConnection.on("GameEnd", () => {
      this.GameEndedSubject.next();
    })
  }

  async StartQuestion(): Promise<void> {
    await this.hubConnection.invoke('StartQuestion', this.lobbyId, this.userSteamId);
  }

  async EndQuestion(): Promise<void> {
    try {
      await this.hubConnection.invoke('EndQuestion', this.lobbyId, this.userSteamId);
    } catch (error) {
      this.logError('Error while ending question: ', error);
      throw error;
    }
  }

  async createLobby(SteamId: string, MaxRounds: number): Promise<number> {
    try {
      await this.hubConnection.invoke('CreateLobby', SteamId, MaxRounds);
      this.userSteamId = SteamId;
      return this.lobbyId;
    } catch (error) {
      this.logError('Error while creating lobby: ', error);
      throw error;
    }
  }

  async joinLobby(lobbyId: number, steamId: string): Promise<void> {
    try {
      await this.hubConnection.invoke('JoinLobby', lobbyId, steamId);
      this.userSteamId = steamId;
    } catch (error) {
      this.logError('Error while joining lobby: ', error);
      throw error;
    }
  }

  async answerQuestion(answerId: number): Promise<void> {
    try {
      await this.hubConnection.invoke('AnswerQuestion', this.lobbyId, this.userSteamId, answerId);
    } catch (error) {
      this.logError('Error while answering question: ', error);
      throw error;
    }
  }

  async leaveLobby(lobbyId: number): Promise<void> {
    try {
      await this.hubConnection.invoke('LeaveLobby', lobbyId);
    } catch (error) {
      this.logError('Error while leaving lobby: ', error);
      throw error;
    }
  }

  public isUserHost(): boolean {
    return this.hostSteamId === this.userSteamId;
  }

  public get Players(): Player[] {
    return this._players;
  }
}
