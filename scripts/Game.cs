using Godot;

public partial class Game : Node2D
{

	private void _on_start_game()
	{
		ProcessMode = ProcessModeEnum.Always;
	}

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Disabled;
	}

	public override void _Process(double delta)
	{
	}
}
