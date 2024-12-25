public class TestResult
{
    private readonly int start;
    private readonly int startRem;
    private readonly int goal;
    private readonly int goalRem;
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
