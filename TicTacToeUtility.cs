using Godot;
using System;
using System.Collections.Generic;
using System.Text;

namespace tictactoe;

public static class TicTacToeUtility
{
  public static bool CheckForWinner(CellState[,] board, out CellState winner)
  {
    winner = CheckForWinnerInRows(board);
    if (winner == CellState.Empty)
    {
      winner = CheckForWinnerInColumns(board);
    }
    if (winner == CellState.Empty)
    {
      winner = CheckForWinnerInDiagonals(board);
    }
    if (winner != CellState.Empty || IsBoardFull(board))
    {
      return true;
    }
    return false;
  }

  public static bool IsBoardFull(CellState[,] board)
  {
    foreach (var cell in board)
    {
      if (cell == CellState.Empty)
      {
        return false;
      }
    }
    return true;
  }

  public static CellState CheckForWinnerInRows(CellState[,] board)
  {
    for (int y = 0; y < 3; y++)
    {
      if (board[0, y] != CellState.Empty && board[0, y] == board[1, y] && board[1, y] == board[2, y])
      {
        return board[0, y];
      }
    }
    return CellState.Empty;
  }

  public static CellState CheckForWinnerInColumns(CellState[,] board)
  {
    for (int x = 0; x < 3; x++)
    {
      if (board[x, 0] != CellState.Empty && board[x, 0] == board[x, 1] && board[x, 1] == board[x, 2])
      {
        return board[x, 0];
      }
    }
    return CellState.Empty;
  }

  public static CellState CheckForWinnerInDiagonals(CellState[,] board)
  {
    if (board[0, 0] != CellState.Empty && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
    {
      return board[0, 0];
    }
    if (board[2, 0] != CellState.Empty && board[2, 0] == board[1, 1] && board[1, 1] == board[0, 2])
    {
      return board[2, 0];
    }
    return CellState.Empty;
  }

  public static int Minimax(CellState[,] board, int depth, bool isMaximizing, CellState player, CellState computer)
  {
    if (CheckForWinner(board, out var winner))
    {
      if (winner == player) return depth - 10;
      if (winner == computer) return 10 - depth;
      return 0;
    }

    if (isMaximizing)
    {
      int maxEval = int.MinValue;
      for (int x = 0; x < 3; x++)
      {
        for (int y = 0; y < 3; y++)
        {
          if (board[x, y] == CellState.Empty)
          {
            board[x, y] = computer; // Maximizing for computer
            int eval = Minimax(board, depth + 1, false, player, computer);
            board[x, y] = CellState.Empty;
            maxEval = Math.Max(maxEval, eval);
          }
        }
      }
      return maxEval;
    }
    else
    {
      int minEval = int.MaxValue;
      for (int x = 0; x < 3; x++)
      {
        for (int y = 0; y < 3; y++)
        {
          if (board[x, y] == CellState.Empty)
          {
            board[x, y] = player; // Minimizing for player
            int eval = Minimax(board, depth + 1, true, player, computer);
            board[x, y] = CellState.Empty;
            minEval = Math.Min(minEval, eval);
          }
        }
      }
      return minEval;
    }
  }

  public static Vector2 GetOptimalMove(CellState[,] board, CellState humanPlayer, CellState computerPlayer)
  {
    Vector2 bestMove = new Vector2(-1, -1);
    int bestScore = int.MinValue;

    var tempBoard = CopyBoard(board);

    for (int x = 0; x < 3; x++)
    {
      for (int y = 0; y < 3; y++)
      {
        if (tempBoard[x, y] == CellState.Empty)
        {
          tempBoard[x, y] = computerPlayer;
          int moveScore = Minimax(tempBoard, 0, false, humanPlayer, computerPlayer);

          GD.Print($"Move: {x}, {y}, Score: {moveScore}");
          PrintBoard(tempBoard);

          tempBoard[x, y] = CellState.Empty;

          if (moveScore > bestScore)
          {
            bestScore = moveScore;
            bestMove = new Vector2(x, y);
          }
        }
      }
    }

    return bestMove;
  }

  public static Vector2 GetRandomMove(CellState[,] board)
  {
    // Get a list of available moves
    var availableMoves = new List<Vector2>();
    for (int x = 0; x < 3; x++)
    {
      for (int y = 0; y < 3; y++)
      {
        if (board[x, y] == CellState.Empty)
        {
          availableMoves.Add(new Vector2(x, y));
        }
      }
    }

    return availableMoves[GD.RandRange(0, availableMoves.Count - 1)];
  }
  public static CellState[,] CopyBoard(CellState[,] board)
  {
    var tempBoard = new CellState[3, 3];
    Array.Copy(board, tempBoard, board.Length);
    return tempBoard;
  }

  public static void PrintBoard(CellState[,] board)
  {
    var boardString = new StringBuilder();

    for (int x = 0; x < 3; x++)
    {
      for (int y = 0; y < 3; y++)
      {
        var symbol = board[x, y] switch
        {
          CellState.X => "X",
          CellState.O => "O",
          _ => "-",
        };
        boardString.Append(symbol);

        if (y == 2)
        {
          boardString.AppendLine();
        }
      }
    }

    GD.Print(boardString.ToString());
  }

}
