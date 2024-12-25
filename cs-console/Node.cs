public class Node
{
    public int[][] state;
    public Node? parent;
    public int cost;
    public double heuristic;
    public int[] blankPos;
    public double totalCost;

    public Node(int[][] state, Node parent, int cost, double heuristic, int[] blankPos)
    {
        this.state = state;
        this.parent = parent;
        this.cost = cost;
        this.heuristic = heuristic;
        // this.h2 = h2;
        this.blankPos = blankPos;
        this.totalCost = this.cost + this.heuristic;
    }
    // equals(other: Node): boolean {
    //   return this.state.flat().join(',') === other.state.flat().join(',');
    // }

    public int[][] copyState()
    {
        int[][] copy = new int[state.Length][];
        for (int i = 0; i < state.Length; i++)
        {
            copy[i] = new int[state.Length];
            for (int j = 0; j < state.Length; j++)
            {
                copy[i][j] = state[i][j];
            }
        }
        return copy;
    }

    public string HashKey()
    {
        string _key = "";

        for (int i = 0; i < state.Length; i++)
        {
            _key += string.Join(",", state[i]);
        }

        return _key;
    }
}
