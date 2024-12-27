public class Node
{
    public short[][] state;
    public Node parent;
    public int heuristic;
    public short[] blankPos;

    public Node(short[][] state, Node parent, int heuristic, short[] blankPos)
    {
        this.state = state;
        this.parent = parent;

        this.heuristic = heuristic;
        // this.h2 = h2;
        this.blankPos = blankPos;
    }
    // equals(other: Node): boolean {
    //   return this.state.flat().join(',') === other.state.flat().join(',');
    // }

    public short[][] copyState()
    {
        short[][] copy = new short[state.Length][];
        for (int i = 0; i < state.Length; i++)
        {
            copy[i] = new short[state.Length];
            for (int j = 0; j < state.Length; j++)
            {
                copy[i][j] = state[i][j];
            }
        }

        return (copy);
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
