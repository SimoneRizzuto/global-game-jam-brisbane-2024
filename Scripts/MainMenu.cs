using globalgamejam2024.Shared;
using Godot;
using System;

public partial class MainMenu : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
    }

    public void NewGame()
    {
        GGJ.gameController.NewGame();
    }

    public void QuitGame()
    {
        GGJ.gameController.QuitGame();
    }
}