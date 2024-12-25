import { NTiles, isEven } from "./util";

// https://www.geeksforgeeks.org/check-instance-8-puzzle-solvable/
// https://www.geeksforgeeks.org/check-instance-15-puzzle-solvable/

export class TestNPuzzle {

  private nSize = 0;

  constructor(private readonly startPuzzle: NTiles, private readonly goalPuzzle: NTiles) {
    this.nSize = goalPuzzle.length;
  }
  private _getInversionsCount(arr: NTiles) {
    let inv_count = 0;
    for (let row = 0; row < this.nSize; row++) {
      for (let col = 0; col < this.nSize; col++) {
        if (arr[row][col] === 0) {
          continue;
        }

        // جستجوی کاشی های جلویی
        let j = col + 1;
        for (let i = row; i < this.nSize; i++) {
          for (; j < this.nSize; j++) {
            if (arr[i][j] && arr[i][j] < arr[row][col])
              inv_count++;
          }
          j = 0;
        }
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

  private _calcInversions(puzzle: NTiles) {
    let invCount = this._getInversionsCount(puzzle);
    if (isEven(this.nSize)) {
      invCount += this._findBlankPosition(puzzle);
      return invCount;
    }
    return invCount;
  }

  calcSolvable() {
    const start = this._calcInversions(this.startPuzzle);
    const goal = this._calcInversions(this.goalPuzzle);
    const result = {
      start,
      goal,
      startRem: start % 2,
      goalRem: goal % 2,
      canSolve: false
    };
    result.canSolve = (result.startRem === result.goalRem);
    return result;
  }
}