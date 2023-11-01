using Godot;

public partial class TicTacToeGame : Node2D
{
	enum CellState
	{
		Empty,
		X,
		O
	}

	private CellState[,] gameBoard = new CellState[3, 3];
	private CellState currentPlayer = CellState.X;

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
	}

	private void OnCellPressed(int x, int y)
	{
		if (gameBoard[x, y] == CellState.Empty)
		{
			gameBoard[x, y] = currentPlayer;
			UpdateCell(x, y);
			CheckForWinner();
			currentPlayer = (currentPlayer == CellState.X) ? CellState.O : CellState.X;
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

	private void CheckForWinner()
	{
		var winner = CheckForWinnerInRows();
		if (winner == CellState.Empty)
		{
			winner = CheckForWinnerInColumns();
		}
		if (winner == CellState.Empty)
		{
			winner = CheckForWinnerInDiagonals();
		}
		if (winner != CellState.Empty || IsBoardFull())
		{
			ShowGameOverDialog(winner);
		}
	}

	private bool IsBoardFull()
	{
    foreach (var cell in gameBoard)
		{
      if (cell == CellState.Empty)
			{
        return false;
      }
    }
    return true;
  }

	private CellState CheckForWinnerInRows()
	{
		for (int y = 0; y < 3; y++)
		{
			if (gameBoard[0, y] != CellState.Empty && gameBoard[0, y] == gameBoard[1, y] && gameBoard[1, y] == gameBoard[2, y])
			{
				return gameBoard[0, y];
			}
		}
		return CellState.Empty;
	}

	private CellState CheckForWinnerInColumns()
	{
		for (int x = 0; x < 3; x++)
		{
			if (gameBoard[x, 0] != CellState.Empty && gameBoard[x, 0] == gameBoard[x, 1] && gameBoard[x, 1] == gameBoard[x, 2])
			{
				return gameBoard[x, 0];
			}
		}
		return CellState.Empty;
	}

	private CellState CheckForWinnerInDiagonals()
	{
		if (gameBoard[0, 0] != CellState.Empty && gameBoard[0, 0] == gameBoard[1, 1] && gameBoard[1, 1] == gameBoard[2, 2])
		{
			return gameBoard[0, 0];
		}
		if (gameBoard[2, 0] != CellState.Empty && gameBoard[2, 0] == gameBoard[1, 1] && gameBoard[1, 1] == gameBoard[0, 2])
		{
			return gameBoard[2, 0];
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
}
