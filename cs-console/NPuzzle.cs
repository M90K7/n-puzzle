using System.Linq;

public class NPuzzle
{
  int size;
  int[][] initialState;
  int[][] goalState;
  int beamWidth;
  Dictionary<int, int[]> goalPositions = new();


  public NPuzzle(int[][] initialState, int[][] goalState, int beamWidth)
  {
    this.size = goalState.Length;
    this.initialState = initialState;
    this.goalState = goalState;
    this.beamWidth = beamWidth;
    this.goalPositions = this.getGoalPositions(goalState);
  }

  // Create a map for goal positions for efficient lookup
  private Dictionary<int, int[]> getGoalPositions(int[][] goalState)
  {
    Dictionary<int, int[]> positions = new();
    for (var i = 0; i < goalState.Length; i++)
    {
      for (var j = 0; j < goalState[i].Length; j++)
      {
        positions.Add(goalState[i][j], [i, j]);
      }
    }
    return positions;
  }

  // Manhattan Distance heuristic
  private int manhattanHeuristic(int[][] state)
  {
    var size = state.Length;
    var heuristic = 0;

    for (var i = 0; i < size; i++)
    {
      for (var j = 0; j < size; j++)
      {
        var value = state[i][j];
        if (value != 0)
        {
          var pos = this.goalPositions[value];
          heuristic += Math.Abs(i - pos[0]) + Math.Abs(j - pos[1]);
        }
      }
    }

    return heuristic;
  }

  // Misplaced tiles heuristic
  private int misplacedTilesHeuristic(int[][] state)
  {
    var misplaced = 0;
    for (var i = 0; i < state.Length; i++)
    {
      for (var j = 0; j < state[i].Length; j++)
      {
        var value = state[i][j];
        if (value != 0 && value != this.goalState[i][j])
        {
          misplaced++;
        }
      }
    }
    return misplaced;
  }

  private double euclideanDistanceHeuristic(int[][] state)
  {
    var size = state.Length;
    double euc = 0;

    for (var i = 0; i < size; i++)
    {
      for (var j = 0; j < size; j++)
      {
        var value = state[i][j];
        if (value != 0)
        {
          var pos = this.goalPositions[value];
          euc += Math.Sqrt(Math.Pow(pos[0] - i, 2) + Math.Pow(pos[1] - j, 2));
        }
      }
    }

    return euc;
  }

  public double CalcHeuristic(int[][] state){
    return 
    // this.euclideanDistanceHeuristic(state) + 
      this.manhattanHeuristic(state)
      // +this.misplacedTilesHeuristic(state)
      ;
  }

  public List<Node> getNeighbors(Node node)
  {
    int[][] directions = [[-1, 0], [1, 0], [0, -1], [0, 1]];

    List<Node> neighbors = new();

    foreach (var dir in directions)
    {
      var newRow = node.blankPos[0] + dir[0];
      var newCol = node.blankPos[1] + dir[1];

      if (newRow >= 0 && newRow < this.size && newCol >= 0 && newCol < this.size)
      {
        var newState = node.copyState();
        newState[node.blankPos[0]][node.blankPos[1]] = newState[newRow][newCol];
        newState[newRow][newCol] = 0;

        neighbors.Add(
          new Node(
            newState,
            node,
            node.cost + 1,
            this.CalcHeuristic(newState),
            [newRow, newCol]
          )
        );
      }
    }

    return neighbors;
  }

  // Find the blank tile (0) position
  private int[] findBlankTile(int[][] state)
  {
    for (var i = 0; i < state.Length; i++)
    {
      for (var j = 0; j < state[i].Length; j++)
      {
        if (state[i][j] == 0) return [i, j];
      }
    }
    throw new Exception("No blank tile found");
  }


  public void kBeamSearch()
  {
    var blankPos = this.findBlankTile(this.initialState);
    List<Node> openList = [new Node(this.initialState, null, 0,
      this.CalcHeuristic(this.initialState),
      blankPos)];
    Dictionary<string, bool> hashKeys = new();


    while (openList.Count > 0)
    {
      var currentNodes = openList.OrderBy(x => x.totalCost).Take(this.beamWidth).ToArray();
      openList.RemoveRange(0, Math.Min(this.beamWidth, openList.Count));

      var print = string.Join(" , ", currentNodes.Select(c => c.totalCost.ToString()));
      Console.WriteLine($"total cost: {print} & Tree {openList.Count} - unique: {hashKeys.Count}");

      foreach (var currentNode in currentNodes)
      {
        var currentStateKey = currentNode.HashKey();
        if (hashKeys.ContainsKey(currentStateKey))
        {
          continue;
        };

        if (currentNode.heuristic == 0)
        {
          Console.WriteLine($"solution found. Tree {openList.Count} - unique: {hashKeys.Count}");
          this.printSolution(currentNode);
          return;
        }

        hashKeys.Add(currentStateKey, true);

        foreach (var neighbor in this.getNeighbors(currentNode))
        {
          var neighborStateKey = neighbor.HashKey();
          if (!hashKeys.ContainsKey(neighborStateKey))
          {
            openList.Add(neighbor);
          }
        }
      }
    }

    Console.WriteLine("No solution found.");
  }
  public void printSolution(Node node){
    Stack<Node> path = [];
    while (node != null) {
      path.Push(node);
      node = node.parent;
    }

    Console.WriteLine("Solution path:");
    int c = path.Count;
    while (path.Count > 0) {
      this.printState(path.Pop()!.state);
      Console.WriteLine("----");
    }
    Console.WriteLine($"Solution with: {c} paths");
  }

  public void printState(int[][] state){
    foreach (var row in state) {
      Console.WriteLine(string.Join("\t", row.Select(r => string.Format("{0,3:}", r))));
    }
  }

}
