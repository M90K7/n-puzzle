public class TestNPuzzle
{
    private int nSize = 0;
    private readonly int[][] startPuzzle;
    private readonly int[][] goalPuzzle;

    public TestNPuzzle(int[][] startPuzzle, int[][] goalPuzzle)
    {
        this.nSize = goalPuzzle.Length;
        this.startPuzzle = startPuzzle;
        this.goalPuzzle = goalPuzzle;
    }
    private int _getInversionsCount(int[][] arr)
    {
        var inv_count = 0;
        for (var row = 0; row < this.nSize; row++)
        {
            for (var col = 0; col < this.nSize; col++)
            {
                if (arr[row][col] == 0)
                {
                    continue;
                }

                // جستجوی کاشی های جلویی
                var j = col + 1;
                for (var i = row; i < this.nSize; i++)
                {
                    for (; j < this.nSize; j++)
                    {
                        if (arr[i][j] != 0 && arr[i][j] < arr[row][col])
                            inv_count++;
                    }
                    j = 0;
                }
            }
        }
        return inv_count;
    }

    private int _findBlankPosition(int[][] puzzle)
    {
        for (var i = 0; i < this.nSize; i++)
            for (var j = 0; j < this.nSize; j++)
                if (puzzle[i][j] == 0)
                    return i + 1;
        return 0;
    }

    private int _calcInversions(int[][] puzzle)
    {
        var invCount = this._getInversionsCount(puzzle);
        if (this.nSize % 2 == 0)
        {
            invCount += this._findBlankPosition(puzzle);
            return invCount;
        }
        return invCount;
    }

    public TestResult calcSolvable()
    {
        var start = this._calcInversions(this.startPuzzle);
        var goal = this._calcInversions(this.goalPuzzle);
        TestResult result = new TestResult(start, goal);
        return result;
    }
}