import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WebsocketService {

  private socket: WebSocket | undefined;
  private readonly SERVER_URL = 'ws://localhost:5000/ws';

  public connect(): void {
    this.socket = new WebSocket(this.SERVER_URL);
  }

  public onMessage(): Observable<any> {
    return new Observable(observer => {
      if (!this.socket) throw new Error('Socket is not initialized')
      this.socket.addEventListener('message', event => {
        observer.next(JSON.parse(event.data));
      });
    });
  }

  public send(data: any): void {
    if (!this.socket) throw new Error('Socket is not initialized')
    this.socket.send(JSON.stringify(data));
  }

  public close(): void {
    if (!this.socket) throw new Error('Socket is not initialized')
    this.socket.close();
  }
}
