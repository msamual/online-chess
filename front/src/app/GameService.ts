import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private newGameSubject = new Subject<void>();
  private undoMoveSubject = new Subject<void>();

  newGame$ = this.newGameSubject.asObservable();
  undoMove$ = this.undoMoveSubject.asObservable();

  startNewGame() {
    this.newGameSubject.next();
  }

  undoLastMove() {
    this.undoMoveSubject.next();
  }
}
