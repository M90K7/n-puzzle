import { NTiles, isEven } from "./util";

// https://www.geeksforgeeks.org/check-instance-8-puzzle-solvable/
// https://www.geeksforgeeks.org/check-instance-15-puzzle-solvable/

export class TestNPuzzle {

  private nSize = 0;
  private _getInversionsCount(arr: NTiles) {
    let inv_count = 0;
    for (let i = 0; i < this.nSize * this.nSize - 1; i++) {
      for (let j = i + 1; j < this.nSize * this.nSize; j++) {
        if (arr[j] && arr[i] && arr[i] > arr[j])
          inv_count++;
      }
    }
    return inv_count;
  }

  private _findBlankPosition(puzzle: NTiles) {
    for (let i = 0; i < this.nSize; i++)
      for (let j = 0; j < this.nSize; j++)
        if (puzzle[i][j] === 0)
          return i + 1;
    return 0;
  }

  private _calcReminder(puzzle: NTiles) {
    let invCount = this._getInversionsCount(puzzle);
    if (isEven(this.nSize)) {
      invCount += this._findBlankPosition(puzzle);
      return invCount % 2;
    }
    return invCount % 2;
  }

  isSolvable(startPuzzle: NTiles, goalPuzzle: NTiles) {
    this.nSize = goalPuzzle.length;
    return this._calcReminder(startPuzzle) === this._calcReminder(goalPuzzle);
  }
}