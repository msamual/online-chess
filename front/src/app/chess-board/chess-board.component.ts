import { Component } from '@angular/core';
import { Chess } from 'chess.js'; // Импортируем Chess как функцию
import { CommonModule } from '@angular/common';
import { GameService } from '../GameService';
import { WebSocketService } from '../WebSocket';

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
  private draggedFrom: { row: number, col: number } | null = null;

  constructor(private gameService: GameService, private webSocketService: WebSocketService) {
    this.chess = new Chess(); // Вызов Chess как функции
    this.board = this.chess.board();
  }

  movePiece(from: string, to: string): void {
    const move = this.chess.move({ from, to });
    if (move) {
      this.webSocketService.sendMove(move); // Отправка хода на сервер
      this.updateBoard();
    } else {
      alert('Invalid move!');
    }
  }

  updateBoard(): void {
    this.board = this.chess.board();
  }

  ngOnInit(): void {
    this.webSocketService.onMoveReceived().subscribe((move: any) => {
      this.chess.move(move);
      this.updateBoard();
    });
  }

  onDragStart(event: DragEvent, row: number, col: number): void {
    this.draggedFrom = { row, col };
    
    // Прозрачное изображение, чтобы скрыть стандартное перетаскивание
    const img = new Image();
    img.src = 'images/transparent.png';
    event.dataTransfer!.setDragImage(img, 0, 0);

    event.dataTransfer!.effectAllowed = 'move';
}

  onDragOver(event: DragEvent): void {
    event.preventDefault(); // Нужно, чтобы разрешить событие drop
    event.dataTransfer!.dropEffect = 'move';
  }

  onDrop(event: DragEvent, row: number, col: number): void {
    if (this.draggedFrom) {
     const from = this.convertToChessNotation(this.draggedFrom.row, this.draggedFrom.col);
     const to = this.convertToChessNotation(row, col);
     this.movePiece(from, to);
     this.updateBoard();
     this.draggedFrom = null;
    }
  }

  convertToChessNotation(row: number, col: number): string {
    const letters = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];
    return letters[col] + (8 - row);
  }

  getImageForPiece(square: any): string {
    if (!square) {
      return '';
    }
  
    return `images/${square.color}${square.type}.png`;
  }
  
  newGame(){
    this.chess.reset();
    this.updateBoard();
  }

  undoMove(){
    this.chess.undo();
    this.updateBoard();
  }
}
