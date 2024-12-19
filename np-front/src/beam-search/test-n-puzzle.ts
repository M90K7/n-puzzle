import { NPuzzle } from "./beam.types";

// https://www.geeksforgeeks.org/check-instance-8-puzzle-solvable/
// https://www.geeksforgeeks.org/check-instance-15-puzzle-solvable/

export class TestNPuzzle {

  private nSize = 4;
  private _getInversionsCount(arr: NPuzzle) {
    let inv_count = 0;
    for (let i = 0; i < this.nSize * this.nSize - 1; i++) {
      for (let j = i + 1; j < this.nSize * this.nSize; j++) {
        if (arr[j] && arr[i] && arr[i] > arr[j])
          inv_count++;
      }
    }
    return inv_count;
  }
  // find Position of blank from bottom
  private _findXPosition(puzzle: NPuzzle) {
    for (let i = this.nSize - 1; i >= 0; i--)
      for (let j = this.nSize - 1; j >= 0; j--)
        if (puzzle[i][j] === 0)
          return this.nSize - i;
  }

  isSolvable(puzzle: NPuzzle) {
    this.nSize = puzzle.length;
    let invCount = this._getInversionsCount(puzzle);

    // If grid is odd, return true if inversion
    // count is even.
    if (this.nSize & 1)
      return !(invCount & 1);

    else {     // grid is even
      let pos = this._findXPosition(puzzle);
      if (pos & 1)
        return !(invCount & 1);
      else
        return invCount & 1;
    }
  }
}