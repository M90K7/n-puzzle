#nullable disable
internal class Program
{
  private static void Main(string[] args)
  {
    // >>>>>>   6 * 6
    //   short[][] initial = [
    // [1, 2, 4, 3, 16, 6],
    //     [7, 8, 9, 10, 11, 12],
    //     [13, 14, 15, 0, 17, 18],
    //     [19, 20, 21, 22, 23, 24],
    //     [5, 25, 27, 28, 29, 30],
    //     [31, 26, 32, 33, 34, 35],
    //   ];
    // short[][] goal = [
    //   [1, 2, 3, 4, 5, 6],
    //   [7, 8, 9, 10, 11, 12],
    //   [13, 14, 15, 16, 17, 18],
    //   [19, 20, 21, 22, 23, 24],
    //   [25, 26, 27, 28, 29, 30],
    //   [31, 32, 33, 34, 35, 0],
    // ];

    // >>>>>>   8 * 8
    short[][] initial = [
  [  9  ,10  ,24 , 45 , 42 , 48 , 13  ,59],
  [ 26  ,17  ,54 , 41 , 50 , 61 ,  0  ,43],
  [ 57  ,14  , 7 , 58 , 12 ,  3 , 27  ,62],
  [ 18  , 2  , 8 , 20 , 31 , 19 , 30  , 4],
  [ 38  ,28  ,56 , 34 , 60 , 29 ,  1  ,63],
  [ 36  ,11  ,52 , 25 , 32 , 15 , 16  ,40],
  [ 23  ,51  ,55 ,  5 , 22 , 53 , 47  ,21],
  [ 46  ,49  , 6 , 33 , 37 , 35 , 39  ,44],
];

    short[][] goal = [
     [27 ,  6,  44,  32,  19,  36,  24,  15],
 [50 , 54,   5,  48,  43,  33,  53,  45],
 [ 4 , 38,  37,  41,  17,  35,  21,  63],
 [18 , 46,  22,   9,  58,  11,  14,  51],
 [31 , 52,  55,  12,  61,  62,  20,   8],
 [59 , 49,  23,   1,  25,  10,  28,  60],
 [29 , 13,  47,   2,  56,  34,   3,   7],
 [42 , 16,   0,  26,  30,  57,  40,  39],
];

    const int N = 8;
    const int K = 32;

    // var puzzleGen = new NPuzzleGenerator(N);
    // initial = puzzleGen.CreatePuzzle();
    // goal = puzzleGen.CreatePuzzle();

    Console.WriteLine("Goal:");
    NPuzzle.printState(goal);

    Console.WriteLine("Start:");
    NPuzzle.printState(initial);

    var solveRes = new TestNPuzzle(initial, goal).calcSolvable();

    Console.WriteLine("canSolve: {0}", solveRes.canSolve);

    if (!solveRes.canSolve)
    {
      return;
    }

    NPuzzle puzzle = new(initial, goal, K);
    try
    {
      puzzle.kBeamSearch();
      // puzzle.StochasticBeamSearch();
    }
    catch (Exception ex)
    {
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

