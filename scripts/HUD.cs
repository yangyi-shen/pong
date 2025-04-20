using Godot;

public partial class HUD : CanvasLayer
{
	[Signal] public delegate void restartEventHandler();

	private void _on_score(int player_number)
	{
		if (player_number == 1)
		{
			Label label = GetNode<Label>("player_score");
			label.Text = (int.Parse(label.Text) + 1).ToString(); // increment score
		}
		else if (player_number == 2)
		{
			Label label = GetNode<Label>("ai_score");
			label.Text = (int.Parse(label.Text) + 1).ToString();
		}
	}

	private void _on_pressed()
	{
		// propagate signal to main node
		EmitSignal(SignalName.restart);

		// reset score values
		Label player_label = GetNode<Label>("player_score");
		Label ai_label = GetNode<Label>("ai_score");
		player_label.Text = "0";
		ai_label.Text = "0";
	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
}
