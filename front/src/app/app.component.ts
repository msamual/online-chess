import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ChessBoardComponent } from './chess-board/chess-board.component';
import { GameControlComponent } from './game-control/game-control.component';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ChessBoardComponent, GameControlComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'online-chess';
}
