import { Component } from '@angular/core';
import { Chess } from 'chess.js'; // Импортируем Chess как функцию
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-chess-board',
  templateUrl: './chess-board.component.html',
  styleUrls: ['./chess-board.component.css'],
  standalone: true,
  imports: [CommonModule]
})
export class ChessBoardComponent {
  private chess: any;
  public board: any[][];

  constructor() {
    this.chess = new Chess(); // Вызов Chess как функции
    this.board = this.chess.board();
  }

  movePiece(from: string, to: string): void {
    const move = this.chess.move({ from, to });
    if (move) {
      this.updateBoard();
    } else {
      alert('Invalid move!');
    }
  }

  updateBoard(): void {
    this.board = this.chess.board();
  }

  ngOnInit(): void {
    this.updateBoard();
  }
  
}
