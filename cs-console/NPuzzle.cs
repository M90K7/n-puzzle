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

  public double CalcHeuristic(int[][] state)
  {
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

    List<Node> openList = [new Node(this.initialState, null, 0, this.CalcHeuristic(this.initialState), blankPos)];

    Dictionary<string, bool> hashKeys = new();

    List<Node> closedNodes = [];

    while (openList.Count > 0)
    {
      List<Node> nextOpenList = [];
      List<Node> discardedNodes = [];

      // var currentNodes = openList.OrderBy(x => x.heuristic).Take(this.beamWidth).ToArray();
      // openList.RemoveRange(0, Math.Min(this.beamWidth, openList.Count));

      // var print = string.Join(" , ", currentNodes.Select(c => c.totalCost.ToString()));
      // Console.WriteLine($"total cost: {print} & Tree {openList.Count} - unique: {hashKeys.Count}");

      foreach (var currentNode in openList)
      {
        // var currentStateKey = currentNode.HashKey();
        // if (hashKeys.ContainsKey(currentStateKey))
        // {
        //   continue;
        // };

        if (currentNode.heuristic == 0)
        {
          Console.WriteLine($"solution found. Tree {openList.Count} - unique: {hashKeys.Count}");
          this.printSolution(currentNode);
          return;
        }

        // hashKeys.Add(currentStateKey, true);

        foreach (var neighbor in this.getNeighbors(currentNode))
        {
          // var neighborStateKey = neighbor.HashKey();
          // if (!hashKeys.ContainsKey(neighborStateKey))
          {
            nextOpenList.Add(neighbor);
          }
        }
      }

      nextOpenList = nextOpenList.OrderBy(l => l.heuristic).ToList();
      // closedNodes.AddRange(nextOpenList.Skip(beamWidth + 1));

      openList = nextOpenList.Take(beamWidth).ToList();
      nextOpenList.Clear();

      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    Console.WriteLine("No solution found.");
  }

  public void StochasticBeamSearch()
  {
    var blankPos = this.findBlankTile(this.initialState);

    List<Node> openList = [new Node(this.initialState, null, 0, this.CalcHeuristic(this.initialState), blankPos)];

    Dictionary<string, bool> hashKeys = new();

    List<Node> closedNodes = [];

    Random random = new Random();

    var SelectNextBeam = (List<Node> nodes) =>
    {
      var nextBeam = new List<Node>();
      double totalFitness = nodes.Sum(c => c.heuristic);
      if (totalFitness == 0) totalFitness = 1;

      nodes.Sort((a, b) => b.heuristic.CompareTo(a.heuristic));

      for (int i = 0; i < Math.Min(beamWidth, nodes.Count); i++)
      {
        double randomValue = random.NextDouble() * totalFitness;
        double cumulativeFitness = 0;

        foreach (var candidate in nodes)
        {
          cumulativeFitness += candidate.heuristic;
          if (randomValue <= cumulativeFitness)
          {
            nextBeam.Add(candidate);
            break;
          }
        }
      }

      return nextBeam.Distinct().ToList();
    };

    while (openList.Count > 0)
    {
      List<Node> nextOpenList = [];

      // var currentNodes = openList.OrderBy(x => x.heuristic).Take(this.beamWidth).ToArray();
      // openList.RemoveRange(0, Math.Min(this.beamWidth, openList.Count));

      // var print = string.Join(" , ", currentNodes.Select(c => c.totalCost.ToString()));
      // Console.WriteLine($"total cost: {print} & Tree {openList.Count} - unique: {hashKeys.Count}");

      foreach (var currentNode in openList)
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
            nextOpenList.Add(neighbor);
          }
        }

      }
      // Method 1
      // openList = SelectNextBeam(nextOpenList);

      // Method 2
      if (nextOpenList.Count > beamWidth)
      {
        nextOpenList = nextOpenList.OrderBy(_ => random.Next()).Take(beamWidth).ToList();
      }
      openList = nextOpenList;


      if (openList.Count == 0)
        break;

      // GC.Collect();
      // GC.WaitForPendingFinalizers();
    }

    Console.WriteLine("No solution found.");
  }


  public void printSolution(Node node)
  {
    Stack<Node> path = [];
    while (node != null)
    {
      path.Push(node);
      node = node.parent;
    }

    Console.WriteLine("Solution path:");
    int c = path.Count;
    while (path.Count > 0)
    {
      printState(path.Pop()!.state);
      Console.WriteLine("----");
    }
    Console.WriteLine($"Solution with: {c} paths");
  }

  public static void printState(int[][] state)
  {
    foreach (var row in state)
    {
      Console.WriteLine(string.Join("\t", row.Select(r => string.Format("{0,3:}", r))));
    }
  }

}
