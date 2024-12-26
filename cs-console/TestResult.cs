public class TestResult
{
    public readonly int start;
    public readonly int startRem;
    public readonly int goal;
    public readonly int goalRem;
    public bool canSolve;

    public TestResult(int start, int goal)
    {
        this.start = start;
        this.goal = goal;
        this.startRem = start % 2;
        this.goalRem = goal % 2;
        this.canSolve = this.startRem == goalRem;
    }
}
