using System;
using System.Collections.Generic;
using System.Linq;

public class NPuzzleGenerator
{
  private int[][] puzzle;
  private int size;
  private (int row, int col) blankPos;

  public NPuzzleGenerator(int size)
  {
    this.size = size;
    this.puzzle = this.CreatePuzzle();
    this.blankPos = this.FindBlank();
  }

  public int[][] CreatePuzzle()
  {
    // ایجاد یک لیست از اعداد 0 تا size*size - 1
    List<int> tiles = Enumerable.Range(0, size * size).ToList();

    // به هم ریختن لیست با استفاده از الگوریتم Fisher-Yates
    Random rng = new Random();
    int n = tiles.Count;
    while (n > 1)
    {
      n--;
      int k = rng.Next(n + 1);
      int value = tiles[k];
      tiles[k] = tiles[n];
      tiles[n] = value;
    }

    // تبدیل لیست به آرایه دو بعدی
    int[][] puzzle = new int[size][];
    for (int i = 0; i < size; i++)
    {
      puzzle[i] = new int[size];
      for (int j = 0; j < size; j++)
      {
        puzzle[i][j] = tiles[i * size + j];
      }
    }
    return puzzle;
  }

  public int[][] CreateGoalState()
  {
    int[][] goal = new int[size][];
    int counter = 0;
    for (int i = 0; i < size; i++)
    {
      goal[i] = new int[size];
      for (int j = 0; j < size; j++)
      {
        goal[i][j] = counter++;
      }
    }
    return goal;
  }

  private (int row, int col) FindBlank()
  {
    for (int i = 0; i < size; i++)
    {
      for (int j = 0; j < size; j++)
      {
        if (puzzle[i][j] == 0)
        {
          return (i, j);
        }
      }
    }
    throw new Exception("Blank tile not found!");
  }

  public NPuzzleGenerator MoveTile(string direction)
  {
    (int row, int col) newPos = blankPos;

    switch (direction)
    {
      case "up":
        newPos.row--;
        break;
      case "down":
        newPos.row++;
        break;
      case "left":
        newPos.col--;
        break;
      case "right":
        newPos.col++;
        break;
    }

    if (newPos.row >= 0 && newPos.row < size && newPos.col >= 0 && newPos.col < size)
    {
      // ایجاد کپی عمیق به روش صحیح
      int[][] newPuzzle = puzzle.Select(row => row.ToArray()).ToArray();

      // جابجایی خانه ها در کپی
      (newPuzzle[blankPos.row][blankPos.col], newPuzzle[newPos.row][newPos.col]) = (newPuzzle[newPos.row][newPos.col], newPuzzle[blankPos.row][blankPos.col]);

      // بازگرداندن یک نمونه جدید از کلاس با پازل جدید
      return new NPuzzleGenerator(size) { puzzle = newPuzzle, blankPos = newPos };
    }
    return null;
  }

  public void PrintPuzzle(int[][] puzzle)
  {
    for (int i = 0; i < size; i++)
    {
      for (int j = 0; j < size; j++)
      {
        Console.Write(puzzle[i][j].ToString().PadLeft(2, '0') + " ");
      }
      Console.WriteLine();
    }
  }

  public int ManhattanDistance(int[][] goal)
  {
    int distance = 0;
    for (int i = 0; i < size; i++)
    {
      for (int j = 0; j < size; j++)
      {
        int value = puzzle[i][j];
        if (value != 0)
        {
          (int goalRow, int goalCol) = FindValueInGoal(goal, value);
          distance += Math.Abs(i - goalRow) + Math.Abs(j - goalCol);
        }
      }
    }
    return distance;
  }

  private (int row, int col) FindValueInGoal(int[][] goal, int value)
  {
    for (int i = 0; i < size; i++)
    {
      for (int j = 0; j < size; j++)
      {
        if (goal[i][j] == value)
        {
          return (i, j);
        }
      }
    }
    throw new Exception("Value not found in goal state!");
  }
}