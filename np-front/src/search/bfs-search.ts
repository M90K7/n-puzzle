// Program to print path from root node to destination node
// for N*N - 1 puzzle algorithm using Branch and Bound
// The solution assumes that the instance of the puzzle is solvable

import { NTiles } from "./util";


// Comparison object to be used to order the heap
class comp {
  static compare(lhs: Node, rhs: Node) {
    return Number((lhs.cost + lhs.level) > (rhs.cost + rhs.level));
  }
}

// State space tree nodes
export class Node {

  // Stores the number of misplaced tiles
  cost: number = Infinity;

  constructor(
    readonly mat: NTiles,
    public x: number,
    public y: number,
    readonly level: number,
    readonly parent?: Node
  ) {

    // Stores matrix
    this.mat = mat.map(row => [...row]);
  }
}

export class BfsAlgo {

  N = 3;

  newNode(mat: NTiles, x: number, y: number, newX: number, newY: number, level: number, parent?: Node) {
    const node = new Node(mat, x, y, level, parent);

    // Move tile by 1 position
    [node.mat[x][y], node.mat[newX][newY]] = [node.mat[newX][newY], node.mat[x][y]];

    // Update new blank tile coordinates
    node.x = newX;
    node.y = newY;

    return node;
  }

  calculateMisplaceCost(initial: NTiles, final: NTiles) {
    let count = 0;
    for (let i = 0; i < this.N; i++)
      for (let j = 0; j < this.N; j++)
        if (initial[i][j] && initial[i][j] !== final[i][j])
          count++;
    return count;
  }

  isSafe(x: number, y: number) {
    return x >= 0 && x < this.N && y >= 0 && y < this.N;
  }

  printMatrix(mat: NTiles) {
    for (let i = 0; i < this.N; i++) {
      console.log(mat[i].join(' '));
    }
    console.log('\n');
  }
  printPath(root: Node) {
    if (!root) return;
    if (root.parent)
      this.printPath(root.parent);
    this.printMatrix(root.mat);
  }

  solve(initial: NTiles, x: number, y: number, final: NTiles) {
    const pq: Node[] = [];

    const root = this.newNode(initial, x, y, x, y, 0, undefined);
    root.cost = this.calculateMisplaceCost(initial, final);

    pq.push(root);

    // Bottom, left, top, right
    const row = [1, 0, -1, 0];
    const col = [0, -1, 0, 1];

    while (pq.length > 0) {
      pq.sort(comp.compare);
      const min = pq.shift();

      if (!min) {
        return;
      }

      if (min.cost === 0) {
        this.printPath(min);
        return;
      }

      // Do for each child of min
      // Max 4 children for a node
      for (let i = 0; i < 4; i++) {
        if (this.isSafe(min.x + row[i], min.y + col[i])) {
          // Create a child node and calculate its cost
          const child = this.newNode(min.mat, min.x,
            min.y, min.x + row[i],
            min.y + col[i],
            min.level + 1, min);
          child.cost = this.calculateMisplaceCost(child.mat, final);

          // Add child to the array of live nodes
          pq.push(child);
        }
      }
      console.log(pq);
    }
  }

}