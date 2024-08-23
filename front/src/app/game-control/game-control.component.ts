// game-control.component.ts
import { Component, EventEmitter, Output } from '@angular/core';
import { GameService } from '../GameService';

@Component({
  selector: 'app-game-control',
  templateUrl: './game-control.component.html',
  styleUrls: ['./game-control.component.css'],
  standalone: true
})

export class GameControlComponent {
  
  @Output() newGameEvent = new EventEmitter<void>();
  @Output() undoMoveEvent = new EventEmitter<void>();

  constructor(private gameService: GameService) {}

  newGame(): void {
    this.gameService.startNewGame();
  }

  undoMove(): void {
    this.gameService.undoLastMove();
  }
}
