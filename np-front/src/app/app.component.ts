import { Component } from '@angular/core';
import { JsonPipe } from "@angular/common";

import { BfsAlgo } from "../search/bfs-search";
import { NTiles } from "../search/util";
import { NPuzzle } from "../search/k-beam-search";
import { TestNPuzzle } from "../search/test-n-puzzle";

@Component({
  selector: 'app-root',
  imports: [
    JsonPipe
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'k-beam-search';
  initial!: NTiles;

  goal!: NTiles;

  constructor() {
  }

  runBfsAlgo() {
    this.initial = [
      [1, 2, 3],
      [5, 6, 0],
      [7, 8, 4]
    ];

    this.goal = [
      [1, 2, 3],
      [5, 8, 6],
      [0, 7, 4]
    ];

    const algo = new BfsAlgo();

    const startX = 1, startY = 2;

    algo.solve(this.initial, startX, startY, this.goal);
  }

  runBeamAlgo() {
    // Example usage
    this.initial = [
      [1, 2, 4, 3, 0, 6],
      [7, 8, 9, 10, 11, 12],
      [13, 14, 15, 16, 17, 18],
      [19, 20, 21, 22, 23, 24],
      [5, 25, 27, 28, 29, 30],
      [31, 26, 32, 33, 34, 35],
    ];
    this.goal = [
      [1, 2, 3, 4, 5, 6],
      [7, 8, 9, 10, 11, 12],
      [13, 14, 15, 16, 17, 18],
      [19, 20, 21, 22, 23, 24],
      [25, 26, 27, 28, 29, 30],
      [31, 32, 33, 34, 35, 0],
    ];

    // this.initial = [
    //   [2, 1, 3],
    //   [5, 6, 8],
    //   [7, 0, 4]
    // ];

    // this.goal = [
    //   [0, 2, 3],
    //   [5, 8, 6],
    //   [1, 4, 7]
    // ];

    const solveRes = new TestNPuzzle(this.initial, this.goal).calcSolvable();

    console.log("canSolve: ", solveRes);

    if (!solveRes.canSolve) {
      return;
    }

    const beamWidth = 2;

    const puzzle = new NPuzzle(this.initial, this.goal, beamWidth);
    try {
      const solution = puzzle.kBeamSearch();
      console.log("Solution found:", solution);
    } catch (error) {
      console.error(error);
    }
  }
}
