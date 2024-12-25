internal class Program
{
    private static void Main(string[] args)
    {
        int[][] initial = [
      [1, 2, 4, 3, 0, 6],
      [7, 8, 9, 10, 11, 12],
      [13, 14, 15, 16, 17, 18],
      [19, 20, 21, 22, 23, 24],
      [5, 25, 27, 28, 29, 30],
      [31, 26, 32, 33, 34, 35],
    ];
    int[][] goal = [
      [1, 2, 3, 4, 5, 6],
      [7, 8, 9, 10, 11, 12],
      [13, 14, 15, 16, 17, 18],
      [19, 20, 21, 22, 23, 24],
      [25, 26, 27, 28, 29, 30],
      [31, 32, 33, 34, 35, 0],
    ];

      var solveRes = new TestNPuzzle(initial, goal).calcSolvable();

    Console.WriteLine("canSolve: {0}", solveRes.canSolve);

    if (!solveRes.canSolve) {
      return;
    }

    int beamWidth = 3;

    NPuzzle puzzle = new (initial, goal, beamWidth);
    try {
      puzzle.kBeamSearch();
    } catch (Exception ex) {
      Console.WriteLine(ex.Message);
    }

    }
}
//   printSolution(node: Node): void {
//     var path: Node[] = [];
//     while (node) {
//       path.push(node);
//       node = node.parent!;
//     }

//     console.log("Solution path:");
//     while (path.Length > 0) {
//       this.printState(path.pop()!.state);
//       console.log("----");
//     }
//   }

//   printState(state: NTiles): void {
//     for (var row of state) {
//       console.log(row.join('\t'));
//     }
//   }

//   private getStateKey(state: NTiles) {

//     var sum = 0;
//     for (var row = 0; row < state.Length; row++) {
//       sum += state[row].reduce((sum, v, col) => sum + (v * (col + row)), 0);
//     }

//     return sum;
//   }
// }

