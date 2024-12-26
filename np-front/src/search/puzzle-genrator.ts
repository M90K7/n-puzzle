// ابعاد پازل
const SIZE: number = 10;

// تعریف نوع برای مختصات
interface Coordinates {
  row: number;
  col: number;
}

export class NPuzzleGenerator {
  private puzzle: number[][];
  private size: number;
  private blankPos: Coordinates;

  constructor(size: number, initialPuzzle?: number[][]) {
    this.size = size;
    if (initialPuzzle) {
      this.puzzle = initialPuzzle;
      this.blankPos = this.findBlank();
    } else {
      this.puzzle = this.createPuzzle();
      this.blankPos = this.findBlank();
    }
  }

  public createPuzzle(): number[][] {
    let tiles: number[] = Array.from(Array(this.size * this.size).keys());
    tiles = this.shuffleArray(tiles);

    const puzzle: number[][] = [];
    for (let i = 0; i < this.size; i++) {
      puzzle[i] = [];
      for (let j = 0; j < this.size; j++) {
        puzzle[i][j] = tiles[i * this.size + j];
      }
    }
    return puzzle;
  }

  private shuffleArray(array: number[]): number[] {
    for (let i = array.length - 1; i > 0; i--) {
      const j: number = Math.floor(Math.random() * (i + 1));
      [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
  }

  public createGoalState(): number[][] {
    const goal: number[][] = [];
    let counter = 0;
    for (let i = 0; i < this.size; i++) {
      goal[i] = [];
      for (let j = 0; j < this.size; j++) {
        goal[i][j] = counter++;
      }
    }
    return goal;
  }

  private findBlank(): Coordinates {
    for (let i = 0; i < this.size; i++) {
      for (let j = 0; j < this.size; j++) {
        if (this.puzzle[i][j] === 0) {
          return { row: i, col: j };
        }
      }
    }
    throw new Error("Blank tile not found!");
  }

  public moveTile(direction: string): NPuzzleGenerator | null {
    let newPos: Coordinates | undefined;

    switch (direction) {
      case "up":
        newPos = { row: this.blankPos.row - 1, col: this.blankPos.col };
        break;
      case "down":
        newPos = { row: this.blankPos.row + 1, col: this.blankPos.col };
        break;
      case "left":
        newPos = { row: this.blankPos.row, col: this.blankPos.col - 1 };
        break;
      case "right":
        newPos = { row: this.blankPos.row, col: this.blankPos.col + 1 };
        break;
    }

    if (newPos && newPos.row >= 0 && newPos.row < this.size && newPos.col >= 0 && newPos.col < this.size) {
      // ایجاد کپی عمیق به روش صحیح
      const newPuzzle: number[][] = this.puzzle.map(row => [...row]);

      // جابجایی خانه ها در کپی
      [newPuzzle[this.blankPos.row][this.blankPos.col], newPuzzle[newPos.row][newPos.col]] = [newPuzzle[newPos.row][newPos.col], newPuzzle[this.blankPos.row][this.blankPos.col]];

      // بازگرداندن یک نمونه جدید از کلاس با پازل جدید
      return new NPuzzleGenerator(this.size, newPuzzle);
    }
    return null;
  }

  public printPuzzle(): void {
    for (let i = 0; i < this.size; i++) {
      let row: string = "";
      for (let j = 0; j < this.size; j++) {
        row += this.puzzle[i][j].toString().padStart(2, '0') + " ";
      }
      console.log(row);
    }
  }

  public getPuzzle(): number[][] {
    return this.puzzle;
  }

  public manhattanDistance(goal: number[][]): number {
    let distance: number = 0;
    for (let i = 0; i < this.size; i++) {
      for (let j = 0; j < this.size; j++) {
        const value = this.puzzle[i][j];
        if (value !== 0) {
          let goalRow = -1;
          let goalCol = -1;
          for (let gr = 0; gr < this.size; gr++) {
            for (let gc = 0; gc < this.size; gc++) {
              if (goal[gr][gc] === value) {
                goalRow = gr;
                goalCol = gc;
                break;
              }
            }
            if (goalRow !== -1) break;
          }
          distance += Math.abs(i - goalRow) + Math.abs(j - goalCol);
        }
      }
    }
    return distance;
  }
}