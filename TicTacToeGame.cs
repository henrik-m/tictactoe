using Godot;
using System;

namespace tictactoe;

public partial class TicTacToeGame : Node2D
{ 
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

				button.Pressed += () => MakePlayerMove(row, col);
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

	private void MakePlayerMove(int x, int y)
	{
		if (gameBoard[x, y] == CellState.Empty && currentPlayer == humanPlayer)
		{
			gameBoard[x, y] = currentPlayer;
			UpdateCell(x, y);
			if (TicTacToeUtility.CheckForWinner(gameBoard, out var winner))
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
		if (!TicTacToeUtility.IsBoardFull(gameBoard))
		{
			var move = currentDifficulty switch
			{
				Difficulty.Easy => TicTacToeUtility.GetRandomMove(gameBoard),
				Difficulty.Hard => TicTacToeUtility.GetOptimalMove(gameBoard, humanPlayer, computerPlayer),
				_ => throw new NotSupportedException($"Difficulty {currentDifficulty} is not supported."),
			};

			int x = (int)move.X;
			int y = (int)move.Y;

			// Make the move
			gameBoard[x, y] = currentPlayer;
			UpdateCell(x, y);
			if (TicTacToeUtility.CheckForWinner(gameBoard, out var winner))
			{
				ShowGameOverDialog(winner);
			}
			else
			{
				SwitchPlayer();
			}
		}
	}





}
