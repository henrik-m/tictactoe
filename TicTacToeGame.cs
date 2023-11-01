using Godot;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

public partial class TicTacToeGame : Node2D
{
	enum CellState
	{
		Empty,
		X,
		O
	}

	enum Difficulty
	{
		Easy,
		Hard
	}

	private CellState[,] gameBoard = new CellState[3, 3];
	private CellState currentPlayer = CellState.X;
	private CellState humanPlayer = CellState.X;
	private CellState computerPlayer = CellState.O;
	private Difficulty currentDifficulty = Difficulty.Hard;

	public override void _Ready()
	{
		ResetGame();
		ConnectButtonsToOnCellPressed();
		ConnectGameOverDialogToRestartGame();
	}

	private void ConnectGameOverDialogToRestartGame()
	{
		var gameOverDialog = GetGameOverDialog();
		gameOverDialog.Confirmed += () => ResetGame();
	}

	private void ConnectButtonsToOnCellPressed()
	{
		for (int x = 0; x < 3; x++)
		{
			for (int y = 0; y < 3; y++)
			{
				var row = x;
				var col = y;

				var button = GetButton(row, col);

				button.Pressed += () => OnCellPressed(row, col);
			}
		}
	}

	private void ResetGame()
	{
		for (int x = 0; x < 3; x++)
		{
			for (int y = 0; y < 3; y++)
			{
				gameBoard[x, y] = CellState.Empty;
				UpdateCell(x, y);
			}
		}
		currentPlayer = CellState.X;

		if (currentPlayer == computerPlayer)
		{
			MakeComputerMove();
		}
	}

	private void OnCellPressed(int x, int y)
	{
		if (gameBoard[x, y] == CellState.Empty && currentPlayer == humanPlayer)
		{
			gameBoard[x, y] = currentPlayer;
			UpdateCell(x, y);
			if (CheckForWinner(gameBoard, out var winner))
			{
				ShowGameOverDialog(winner);
			}
			else
			{
				SwitchPlayer();

				if (currentPlayer == computerPlayer)
				{
					MakeComputerMove();
				}
			}
		}
	}

	private AcceptDialog GetGameOverDialog()
	{
		return GetNode<AcceptDialog>("GameOverDialog");
	}

	private Button GetButton(int x, int y)
	{
		return GetNode<Button>($"CenterContainer/GridContainer/Button_{x}{y}");
	}

	private void UpdateCell(int x, int y)
	{
		var button = GetButton(x, y);
		button.Text = gameBoard[x, y] switch
		{
			CellState.X => "X",
			CellState.O => "O",
			_ => "",
		};
	}

	private static bool CheckForWinner(CellState[,] board, out CellState winner)
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

	private static bool IsBoardFull(CellState[,] board)
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

	private static CellState CheckForWinnerInRows(CellState[,] board)
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

	private static CellState CheckForWinnerInColumns(CellState[,] board)
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

	private static CellState CheckForWinnerInDiagonals(CellState[,] board)
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

	private void ShowGameOverDialog(CellState winner)
	{
		var winnerDialog = GetGameOverDialog();

		if (winner == CellState.Empty)
		{
			winnerDialog.DialogText = "It's a tie!";
		}
		else
		{
			winnerDialog.DialogText = $"Player {winner} wins!";
		}

		winnerDialog.PopupCentered();
	}

	private void SwitchPlayer()
	{
		currentPlayer = (currentPlayer == CellState.X) ? CellState.O : CellState.X;
	}

	private void MakeComputerMove()
	{
		// Choose a random move from the available moves
		if (!IsBoardFull(gameBoard))
		{
			var move = currentDifficulty switch
			{
				Difficulty.Easy => GetRandomMove(gameBoard),
				Difficulty.Hard => GetOptimalMove(gameBoard, humanPlayer, computerPlayer),
				_ => throw new NotSupportedException($"Difficulty {currentDifficulty} is not supported."),
			};

			int x = (int)move.X;
			int y = (int)move.Y;

			// Make the move
			gameBoard[x, y] = currentPlayer;
			UpdateCell(x, y);
			if (CheckForWinner(gameBoard, out var winner))
			{
				ShowGameOverDialog(winner);
			}
			else
			{
				SwitchPlayer();
			}
		}
	}

	private static CellState[,] CopyBoard(CellState[,] board)
	{
		var tempBoard = new CellState[3, 3];
		Array.Copy(board, tempBoard, board.Length);
		return tempBoard;
	}

	private static void PrintBoard(CellState[,] board)
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

	private static Vector2 GetOptimalMove(CellState[,] board, CellState humanPlayer, CellState computerPlayer)
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

	private static Vector2 GetRandomMove(CellState[,] board)
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

	private static int Minimax(CellState[,] board, int depth, bool isMaximizing, CellState player, CellState computer)
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
}
