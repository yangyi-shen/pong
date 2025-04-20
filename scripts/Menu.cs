using Godot;

public partial class Menu : CanvasLayer
{
	[Signal] public delegate void start_gameEventHandler();

	private void _on_pressed() {
		Hide();
		EmitSignal(SignalName.start_game);
	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
}
