import { Injectable } from '@angular/core';
import { WebSocketSubject } from 'rxjs/webSocket';

@Injectable({
  providedIn: 'root'
})
export class WebSocketService {
  private socket$: WebSocketSubject<any>;

  constructor() {
    this.socket$ = new WebSocketSubject('ws://localhost:5058'); // Адрес вашего ASP.NET Core сервера
  }

  sendMove(move: any) {
    this.socket$.next(move);
  }

  onMoveReceived() {
    return this.socket$.asObservable();
  }
}
