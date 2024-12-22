// File: n_puzzle_beam_search.ts

import { NTiles } from "./util";

export interface Node {
  state: NTiles;
  g: number; // Cost so far
  h: number; // Heuristic cost
  f: number; // Total cost
  path: NTiles[]; // Path from the start state
  parent?: Node;
}

export class NPuzzle {
  size: number;
  initialState: NTiles;
  goalState: NTiles;
  beamWidth: number;
  goalPositions: Map<number, [number, number]>;

  constructor(initialState: NTiles, goalState: NTiles, beamWidth: number) {
    this.size = goalState.length;
    this.initialState = initialState;
    this.goalState = goalState;
    this.beamWidth = beamWidth;
    this.goalPositions = this.getGoalPositions(goalState);
  }

  // Create a map for goal positions for efficient lookup
  private getGoalPositions(goalState: NTiles): Map<number, [number, number]> {
    const positions = new Map<number, [number, number]>();
    for (let i = 0; i < goalState.length; i++) {
      for (let j = 0; j < goalState[i].length; j++) {
        positions.set(goalState[i][j], [i, j]);
      }
    }
    return positions;
  }

  // Manhattan Distance heuristic
  private manhattanHeuristic(state: NTiles): number {
    let distance = 0;
    for (let i = 0; i < state.length; i++) {
      for (let j = 0; j < state[i].length; j++) {
        const value = state[i][j];
        if (value !== 0) { // Skip empty tile
          const [goalX, goalY] = this.goalPositions.get(value)!;
          distance += Math.abs(goalX - i) + Math.abs(goalY - j);
        }
      }
    }
    return distance;
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

  // Find the blank tile (0) position
  private findBlankTile(state: NTiles): [number, number] {
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
  public solve(): NTiles[] {
    const beam: Node[] = [
      { state: this.initialState, g: 0, h: this.manhattanHeuristic(this.initialState), f: 0, path: [] },
    ];

    while (beam.length > 0) {
      const newBeam: Node[] = [];

      for (const node of beam) {
        if (JSON.stringify(node.state) === JSON.stringify(this.goalState)) {
          return node.path; // Solution found
        }

        const successors = this.generateSuccessors(node.state);
        for (const successor of successors) {
          const g = node.g + 1;
          const h = this.manhattanHeuristic(successor); // Change to misplacedTilesHeuristic if needed
          newBeam.push({
            state: successor,
            g,
            h,
            f: g + h,
            path: [...node.path, successor],
            parent: node
          });
        }
      }

      // Sort by f and limit the beam width
      newBeam.sort((a, b) => a.f - b.f);
      beam.length = Math.min(this.beamWidth, newBeam.length);
      for (let i = 0; i < beam.length; i++) {
        beam[i] = newBeam[i];
      }

      console.log(beam);
    }

    throw new Error("No solution found");
  }

  public solveMe(): NTiles[] {
    const beam: Node[] = [
      { state: this.initialState, g: 0, h: this.manhattanHeuristic(this.initialState), f: 0, path: [] },
    ];

    while (beam.length > 0) {
      const newBeam: Node[] = [];

      for (const node of beam) {
        if (JSON.stringify(node.state) === JSON.stringify(this.goalState)) {
          return node.path; // Solution found
        }

        const successors = this.generateSuccessors(node.state);
        for (const successor of successors) {
          const g = node.g + 1;
          const h = this.manhattanHeuristic(successor); // Change to misplacedTilesHeuristic if needed
          newBeam.push({
            state: successor,
            g,
            h,
            f: g + h,
            path: [...node.path, successor],
            parent: node
          });
        }
      }

      // Sort by f and limit the beam width
      newBeam.sort((a, b) => a.f - b.f);
      beam.length = Math.min(this.beamWidth, newBeam.length);
      for (let i = 0; i < beam.length; i++) {
        beam[i] = newBeam[i];
      }

      console.log(beam);
    }

    throw new Error("No solution found");
  }
}


