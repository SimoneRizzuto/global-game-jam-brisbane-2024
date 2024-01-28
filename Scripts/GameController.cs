using globalgamejam2024.Shared;
using Godot;
using System;

public partial class GameController : Node
{
	[Export] public PackedScene MainMenuScene;
	[Export] public PackedScene GameScene;
	[Export] public GameUIController UIController;
	[Export] public AudioStreamPlayer GameplayMusic;
	[Export] public AudioStreamPlayer MenuMusic;
	[Export] public Label GameOverLabel;
	
	
	//[Export] public Godot.Collections.Array<PackedScene> Levels = new Godot.Collections.Array<PackedScene>();

	protected Node currentLevel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GGJ.gameController = this;
		GGJ.gameUIController = UIController;
		GotoMainMenu();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//
	}

	public void GotoMainMenu()
	{
		if (currentLevel != null)
		{
			currentLevel.QueueFree();
		}

		MenuMusic.Play();
		GameplayMusic.Stop();
		
		currentLevel = MainMenuScene.Instantiate();
		AddChild(currentLevel);

		UIController.Visible = false;
	}

	public void GameOver()
	{
		GameOverLabel.Visible = true;
		GGJ.player.Visible = false;
		GGJ.mobController.StopSpawning();
	}

	public void NewGame()
	{
		if (currentLevel != null)
		{
			currentLevel.QueueFree();
		}

		MenuMusic.Stop();
		GameplayMusic.Play();
		
		currentLevel = GameScene.Instantiate();
		AddChild(currentLevel);
		GGJ.mobController.StartSpawning();
		
		UIController.ResetHealth();
		UIController.Visible = true;
	}

	public void QuitGame()
	{
		if (currentLevel != null)
		{
			currentLevel.QueueFree();
		}
		GetTree().Quit();
	}
}
