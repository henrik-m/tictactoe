using Godot;

namespace tictactoe;

public partial class MainMenu : Node2D
{
  private OptionButton difficultyOptionButton;
  private OptionButton playerOptionButton;
  private Button startGameButton;

  public override void _Ready()
  {
    difficultyOptionButton = GetNode<OptionButton>("CenterContainer/VFlowContainer/GridContainer/DifficultyOption");
    playerOptionButton = GetNode<OptionButton>("CenterContainer/VFlowContainer/GridContainer/StartingPlayerOption");
    startGameButton = GetNode<Button>("CenterContainer/VFlowContainer/StartGameButton");

    startGameButton.Pressed += OnStartGameButtonPressed;

    difficultyOptionButton.Clear();
    difficultyOptionButton.AddItem($"{Difficulty.Easy}", (int)Difficulty.Easy);
    difficultyOptionButton.AddItem($"{Difficulty.Hard}", (int)Difficulty.Hard);

    playerOptionButton.Clear();
    playerOptionButton.AddItem($"{StartingPlayer.Player}", (int)StartingPlayer.Player);
    playerOptionButton.AddItem($"{StartingPlayer.Computer}", (int)StartingPlayer.Computer);
  }

  private void OnStartGameButtonPressed()
  {
    var difficulty = (Difficulty)difficultyOptionButton.GetSelectedId();
    var startingPlayer = (StartingPlayer)playerOptionButton.GetSelectedId();

    var gameScene = ResourceLoader.Load<PackedScene>("res://TicTacToeGame.tscn");
    var game = (TicTacToeGame)gameScene.Instantiate();

    game.ConfigureGame(difficulty, startingPlayer);

    GetTree().Root.AddChild(game);
    QueueFree();
  }
}
