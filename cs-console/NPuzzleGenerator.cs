using System;
using System.Collections.Generic;
using System.Linq;

public class NPuzzleGenerator
{
  private short[][] puzzle;
  private short size;
  private (short row, short col) blankPos;

  public NPuzzleGenerator(short size)
  {
    this.size = size;
    this.puzzle = this.CreatePuzzle();
    this.blankPos = this.FindBlank();
  }

  public short[][] CreatePuzzle()
  {
    // ایجاد یک لیست از اعداد 0 تا size*size - 1
    List<short> tiles = new();

    for (short i = 0; i < size * size; i++)
    {
      tiles.Add(i);
    }

    // به هم ریختن لیست با استفاده از الگوریتم Fisher-Yates
    Random rng = new Random();
    int n = tiles.Count;
    while (n > 1)
    {
      n--;
      int k = rng.Next(n + 1);
      short value = tiles[k];
      tiles[k] = tiles[n];
      tiles[n] = value;
    }

    // تبدیل لیست به آرایه دو بعدی
    short[][] puzzle = new short[size][];
    for (short i = 0; i < size; i++)
    {
      puzzle[i] = new short[size];
      for (short j = 0; j < size; j++)
      {
        puzzle[i][j] = tiles[i * size + j];
      }
    }
    return puzzle;
  }

  public short[][] CreateGoalState()
  {
    short[][] goal = new short[size][];
    short counter = 0;
    for (short i = 0; i < size; i++)
    {
      goal[i] = new short[size];
      for (short j = 0; j < size; j++)
      {
        goal[i][j] = counter++;
      }
    }
    return goal;
  }

  private (short row, short col) FindBlank()
  {
    for (short i = 0; i < size; i++)
    {
      for (short j = 0; j < size; j++)
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
    (short row, short col) newPos = blankPos;

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
      short[][] newPuzzle = puzzle.Select(row => row.ToArray()).ToArray();

      // جابجایی خانه ها در کپی
      (newPuzzle[blankPos.row][blankPos.col], newPuzzle[newPos.row][newPos.col]) = (newPuzzle[newPos.row][newPos.col], newPuzzle[blankPos.row][blankPos.col]);

      // بازگرداندن یک نمونه جدید از کلاس با پازل جدید
      return new NPuzzleGenerator(size) { puzzle = newPuzzle, blankPos = newPos };
    }
    return null;
  }

  public void PrintPuzzle(short[][] puzzle)
  {
    for (short i = 0; i < size; i++)
    {
      for (short j = 0; j < size; j++)
      {
        Console.Write(puzzle[i][j].ToString().PadLeft(2, '0') + " ");
      }
      Console.WriteLine();
    }
  }

  private (short row, short col) FindValueInGoal(short[][] goal, short value)
  {
    for (short i = 0; i < size; i++)
    {
      for (short j = 0; j < size; j++)
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