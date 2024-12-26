// File: n_puzzle_beam_search.ts

import { TestNPuzzle } from "./test-n-puzzle";
import { NTiles, PosRC } from "./util";

export class Node {

  public readonly totalCost: number;

  constructor(
    public readonly state: NTiles,
    public readonly parent: Node | null,
    public readonly cost: number,
    public readonly heuristic: number,
    public readonly blankPos: PosRC
  ) {
    this.totalCost = this.cost + this.heuristic;
  }

  copyState(): NTiles {
    const copy: NTiles = [];
    for (let i = 0; i < this.state.length; i++) {
      copy.push([...this.state[i]]);
    }
    return copy;
  }

  // equals(other: Node): boolean {
  //   return this.state.flat().join(',') === other.state.flat().join(',');
  // }
}

export class NPuzzle {
  readonly size: number;
  initialState: NTiles;
  readonly goalState: NTiles;
  beamWidth: number;
  readonly goalPositions: Map<number, PosRC>;


  constructor(initialState: NTiles, goalState: NTiles, beamWidth: number) {
    this.size = goalState.length;
    this.initialState = initialState;
    this.goalState = goalState;
    this.beamWidth = beamWidth;
    this.goalPositions = this.getGoalPositions(goalState);
  }

  // Create a map for goal positions for efficient lookup
  private getGoalPositions(goalState: NTiles): Map<number, PosRC> {
    const positions = new Map<number, PosRC>();
    for (let i = 0; i < goalState.length; i++) {
      for (let j = 0; j < goalState[i].length; j++) {
        positions.set(goalState[i][j], [i, j]);
      }
    }
    return positions;
  }

  // Manhattan Distance heuristic
  private manhattanHeuristic(state: NTiles): number {
    const size = state.length;
    let heuristic = 0;

    for (let i = 0; i < size; i++) {
      for (let j = 0; j < size; j++) {
        const value = state[i][j];
        if (value !== 0) {
          const [goalRow, goalCol] = this.goalPositions.get(value)!;
          heuristic += (Math.abs(i - goalRow) + Math.abs(j - goalCol));
        }
      }
    }

    return heuristic;
  }

  // Misplaced tiles heuristic
  private misplacedTilesHeuristic(state: NTiles): number {
    let misplaced = 0;
    for (let i = 0; i < state.length; i++) {
      for (let j = 0; j < state[i].length; j++) {
        const value = state[i][j];
        if (value !== 0 && value !== this.goalState[i][j]) {
          misplaced++;
        }
      }
    }
    return misplaced;
  }

  private euclideanDistance(state: NTiles): number {
    const size = state.length;
    let euc = 0;

    for (let i = 0; i < size; i++) {
      for (let j = 0; j < size; j++) {
        const value = state[i][j];
        if (value !== 0) {
          const [goalRow, goalCol] = this.goalPositions.get(value)!;
          euc += Math.sqrt(Math.pow(goalRow - i, 2) + Math.pow(goalCol - j, 2));
        }
      }
    }

    return euc;
  }

  getNeighbors(node: Node): Node[] {
    const directions: PosRC[] = [[-1, 0], [1, 0], [0, -1], [0, 1]];

    const neighbors: Node[] = [];

    for (const [dRow, dCol] of directions) {
      const newRow = node.blankPos[0] + dRow;
      const newCol = node.blankPos[1] + dCol;

      if (newRow >= 0 && newRow < this.size && newCol >= 0 && newCol < this.size) {
        const newState = node.copyState();
        newState[node.blankPos[0]][node.blankPos[1]] = newState[newRow][newCol];
        newState[newRow][newCol] = 0;

        neighbors.push(
          new Node(
            newState,
            node,
            node.cost + 1,
            this.manhattanHeuristic(newState),
            [newRow, newCol]
          )
        );
      }
    }

    return neighbors;
  }

  // Find the blank tile (0) position
  private findBlankTile(state: NTiles): PosRC {
    for (let i = 0; i < state.length; i++) {
      for (let j = 0; j < state[i].length; j++) {
        if (state[i][j] === 0) return [i, j];
      }
    }
    throw new Error("No blank tile found");
  }

  // Generate all possible successor states
  private generateSuccessors(state: NTiles): NTiles[] {
    const [blankX, blankY] = this.findBlankTile(state);
    const directions: [number, number][] = [
      [0, 1], // Right
      [1, 0], // Down
      [0, -1], // Left
      [-1, 0], // Up
    ];
    const successors: NTiles[] = [];

    for (const [dx, dy] of directions) {
      const newX = blankX + dx;
      const newY = blankY + dy;

      if (
        newX >= 0 &&
        newX < this.size &&
        newY >= 0 &&
        newY < this.size
      ) {
        const newState = state.map(row => row.slice()); // Deep copy
        [newState[blankX][blankY], newState[newX][newY]] = [newState[newX][newY], newState[blankX][blankY]];
        successors.push(newState);
      }
    }
    return successors;
  }

  // Beam Search algorithm
  // public solve(): NTiles[] {
  //   const beam: Node[] = [
  //     { state: this.initialState, g: 0, h: this.manhattanHeuristic(this.initialState), f: 0, path: [] },
  //   ];

  //   while (beam.length > 0) {
  //     const newBeam: Node[] = [];

  //     for (const node of beam) {
  //       if (JSON.stringify(node.state) === JSON.stringify(this.goalState)) {
  //         return node.path; // Solution found
  //       }

  //       const successors = this.generateSuccessors(node.state);
  //       for (const successor of successors) {
  //         const g = node.g + 1;
  //         const h = this.manhattanHeuristic(successor); // Change to misplacedTilesHeuristic if needed
  //         newBeam.push({
  //           state: successor,
  //           g,
  //           h,
  //           f: g + h,
  //           path: [...node.path, successor],
  //           parent: node
  //         });
  //       }
  //     }

  //     // Sort by f and limit the beam width
  //     newBeam.sort((a, b) => a.f - b.f);
  //     beam.length = Math.min(this.beamWidth, newBeam.length);
  //     for (let i = 0; i < beam.length; i++) {
  //       beam[i] = newBeam[i];
  //     }

  //     console.log(beam);
  //   }

  //   throw new Error("No solution found");
  // }


  kBeamSearch(): void {
    const blankPos = this.findBlankTile(this.initialState);
    let openList: Node[] = [new Node(this.initialState, null, 0, this.manhattanHeuristic(this.initialState), blankPos)];
    const hashKeys = new Set<string>();


    while (openList.length > 0) {

      let nextOpenList = [];

      // const currentNodes = openList.toSorted((a, b) => a.cost - b.cost).splice(0, this.beamWidth).reverse();
      // const currentNodes = openList.sort((a, b) => a.cost - b.cost).splice(0, this.beamWidth);
      // openList.splice(0, this.beamWidth);
      // const currentNodes = openList.splice(0, this.beamWidth);

      // console.log("total cost: " + openList.map(c => `${c.totalCost}-${c.heuristic}`).toString());

      for (const currentNode of openList) {
        const currentStateKey = currentNode.state.flat().join(',');
        if (hashKeys.has(currentStateKey)) {
          continue;
        };

        if (currentNode.heuristic === 0) {
          this.printSolution(currentNode);
          return;
        }

        hashKeys.add(currentStateKey);

        for (const neighbor of this.getNeighbors(currentNode)) {
          const neighborStateKey = neighbor.state.flat().join(',');
          if (!hashKeys.has(neighborStateKey)) {
            nextOpenList.push(neighbor);
          }
        }
      }

      openList = nextOpenList.sort((a, b) => a.heuristic - b.heuristic).splice(0, this.beamWidth);
    }

    console.log("No solution found.");
  }

  printSolution(node: Node): void {
    const path: Node[] = [];
    while (node) {
      path.push(node);
      node = node.parent!;
    }

    console.log("Solution path:");
    while (path.length > 0) {
      this.printState(path.pop()!.state);
      console.log("----");
    }
  }

  printState(state: NTiles): void {
    for (const row of state) {
      console.log(row.join('\t\t'));
    }
  }

  private getStateKey(state: NTiles) {

    let sum = 0;
    for (let row = 0; row < state.length; row++) {
      sum += state[row].reduce((sum, v, col) => sum + (v * (col + row)), 0);
    }

    return sum;
  }
}


