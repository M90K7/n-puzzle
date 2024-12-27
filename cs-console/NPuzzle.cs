
using System.Diagnostics;
using System.Linq;

public class NPuzzle
{
  int size;
  short[][] initialState;
  short[][] goalState;
  int beamWidth;
  Dictionary<short, short[]> goalPositions = new();


  public NPuzzle(short[][] initialState, short[][] goalState, int beamWidth)
  {
    this.size = goalState.Length;
    this.initialState = initialState;
    this.goalState = goalState;
    this.beamWidth = beamWidth;
    this.goalPositions = this.getGoalPositions(goalState);
  }

  // Create a map for goal positions for efficient lookup
  private Dictionary<short, short[]> getGoalPositions(short[][] goalState)
  {
    Dictionary<short, short[]> positions = new();
    for (short i = 0; i < goalState.Length; i++)
    {
      for (short j = 0; j < goalState[i].Length; j++)
      {
        positions.Add(goalState[i][j], [i, j]);
      }
    }
    return positions;
  }

  // Manhattan Distance heuristic
  private int manhattanHeuristic(short[][] state)
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

  private double euclideanDistanceHeuristic(short[][] state)
  {
    var size = state.Length;
    double euc = 0;

    for (short i = 0; i < size; i++)
    {
      for (short j = 0; j < size; j++)
      {
        short value = state[i][j];
        if (value != 0)
        {
          var pos = this.goalPositions[value];
          euc += Math.Sqrt(Math.Pow(pos[0] - i, 2) + Math.Pow(pos[1] - j, 2));
        }
      }
    }

    return euc;
  }

  public int CalcHeuristic(short[][] state)
  {
    return
      // this.euclideanDistanceHeuristic(state)
      this.manhattanHeuristic(state)
      // this.misplacedTilesHeuristic(state)
      ;
  }

  public List<Node> getNeighbors(Node node)
  {
    short[][] directions = [[-1, 0], [1, 0], [0, -1], [0, 1]];

    List<Node> neighbors = new();

    foreach (var dir in directions)
    {
      int newRow = node.blankPos[0] + dir[0];
      int newCol = node.blankPos[1] + dir[1];

      if (newRow >= 0 && newRow < this.size && newCol >= 0 && newCol < this.size)
      {
        var newState = node.copyState();
        newState[node.blankPos[0]][node.blankPos[1]] = newState[newRow][newCol];
        newState[newRow][newCol] = 0;

        neighbors.Add(
          new Node(
            newState,
            node,
            this.CalcHeuristic(newState),
            [Convert.ToInt16(newRow), Convert.ToInt16(newCol)]
          )
        );
      }
    }

    return neighbors;
  }

  // Find the blank tile (0) position
  private short[] findBlankTile(short[][] state)
  {
    for (short i = 0; i < state.Length; i++)
    {
      for (short j = 0; j < state[i].Length; j++)
      {
        if (state[i][j] == 0) return [i, j];
      }
    }
    throw new Exception("No blank tile found");
  }


  public void kBeamSearch()
  {
    Stopwatch watcher = new();
    watcher.Start();

    var blankPos = this.findBlankTile(this.initialState);

    List<Node> openList = [
      new Node(this.initialState, null, this.CalcHeuristic(this.initialState), blankPos)];

    HashSet<string> hashKeys = new();

    long loop = 0;

    while (openList.Count > 0)
    {

      List<Node> nextOpenList = [];

      // var currentNodes = openList.OrderBy(x => x.heuristic).Take(this.beamWidth).ToArray();
      // openList.RemoveRange(0, Math.Min(this.beamWidth, openList.Count));

      // var print = string.Join(" , ", currentNodes.Select(c => c.totalCost.ToString()));
      // Console.WriteLine($"total cost: {print} & Tree {openList.Count} - unique: {hashKeys.Count}");

      for (int i = 0; i < openList.Count; i++)
      {

        loop++;

        var _hashKey = openList[i].HashKey();
        if (hashKeys.Contains(_hashKey))
        {
          continue;
        };

        if (openList[i].heuristic == 0)
        {
          watcher.Stop();
          Console.WriteLine($"solution found. loop {loop} - unique: {hashKeys.Count} - time: {watcher.ElapsedMilliseconds}");
          // this.printSolution(openList[i]);
          return;
        }

        hashKeys.Add(_hashKey);

        foreach (var neighbor in this.getNeighbors(openList[i]))
        {
          _hashKey = neighbor.HashKey();
          if (!hashKeys.Contains(_hashKey))
          {
            nextOpenList.Add(neighbor);
          }
        }
      }

      openList = nextOpenList.OrderBy(l => l.heuristic).Take(beamWidth).ToList();
      // nextOpenList.Clear();

      if (loop % 1000 == 0)
      {
        GC.Collect();
        GC.WaitForPendingFinalizers();
      }
    }

    Console.WriteLine("No solution found.");
  }

  public void StochasticBeamSearch()
  {
    Stopwatch watcher = new();
    watcher.Start();

    var blankPos = this.findBlankTile(this.initialState);

    List<Node> openList = [new Node(this.initialState, null, this.CalcHeuristic(this.initialState), blankPos)];

    HashSet<string> hashKeys = new();

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

    long loop = 0;
    while (openList.Count > 0)
    {
      List<Node> nextOpenList = [];

      // var currentNodes = openList.OrderBy(x => x.heuristic).Take(this.beamWidth).ToArray();
      // openList.RemoveRange(0, Math.Min(this.beamWidth, openList.Count));

      // var print = string.Join(" , ", currentNodes.Select(c => c.totalCost.ToString()));
      // Console.WriteLine($"total cost: {print} & Tree {openList.Count} - unique: {hashKeys.Count}");

      for (int i = 0; i < openList.Count; i++)
      {
        loop++;
        var currentNode = openList[i];
        var _hashKey = currentNode.HashKey();

        if (hashKeys.Contains(_hashKey))
        {
          continue;
        };

        if (currentNode.heuristic == 0)
        {
          watcher.Stop();
          Console.WriteLine($"solution found. loop {loop} - unique: {hashKeys.Count} - time: {watcher.ElapsedMilliseconds}");
          // this.printSolution(currentNode);
          return;
        }

        hashKeys.Add(_hashKey);

        foreach (var neighbor in this.getNeighbors(currentNode))
        {
          _hashKey = neighbor.HashKey();
          if (!hashKeys.Contains(_hashKey))
          {
            nextOpenList.Add(neighbor);
          }
        }

      }
      // Method 1
      openList = SelectNextBeam(nextOpenList);

      // Method 2
      // if (nextOpenList.Count > beamWidth)
      // {
      //   nextOpenList = nextOpenList.OrderBy(b => b.heuristic).ToList();
      //   var _nextOpenList = nextOpenList.Take(beamWidth / 2).ToList();
      //   _nextOpenList.AddRange(nextOpenList.TakeLast(beamWidth / 2));
      //   nextOpenList = _nextOpenList.ToList();
      // }
      // openList = nextOpenList;

      if (openList.Count == 0)
        break;

      if (hashKeys.Count % 1000 == 0)
      {
        GC.Collect();
        GC.WaitForPendingFinalizers();
      }


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

  public static void printState(short[][] state)
  {
    foreach (var row in state)
    {
      Console.WriteLine(string.Join("\t", row.Select(r => string.Format("{0,3:}", r))));
    }
  }

}
