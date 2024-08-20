// game-control.component.ts
import { Component } from '@angular/core';

@Component({
  selector: 'app-game-control',
  templateUrl: './game-control.component.html',
  styleUrls: ['./game-control.component.css'],
  standalone: true
})
export class GameControlComponent {
  // Удалите инъекцию через конструктор
  constructor() {
    // Доступ к компоненту `ChessBoardComponent` напрямую не поддерживается
  }

  newGame(): void {
    // Логика для новой игры
  }

  undoMove(): void {
    // Логика для отмены хода
  }
}
