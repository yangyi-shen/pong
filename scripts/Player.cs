using System.Diagnostics;
using Godot;

public partial class Player : CharacterBody2D
{

	// stuff you can set in inspector
	[Export] public StringName INPUT_UP { get; set; } = null;
	[Export] public StringName INPUT_DOWN { get; set; } = null;
	[Export] public int SPEED { get; set; } = 400;
	[Export] public int MOVEMENT_CLAMP { get; set; } = 40;

	// data initialized in _Ready() and used in _Process()
	public Vector2 SCREEN_SIZE;
	public Vector2 PLAYER_SIZE;

	private void _on_restart()
	{
		// reset position of player
		Position = new Vector2(
			x: Position.X,
			y: 0
		);
	}

	public override void _Ready()
	{
		SCREEN_SIZE = GetViewportRect().Size;
		PLAYER_SIZE = GetNode<Sprite2D>("Sprite2D").Texture.GetSize();
	}

	public override void _Process(double delta)
	{
		Vector2 velocity = Vector2.Zero; // velocity vector w/ no magnitude

		// movement controls along y-axis
		if (Input.IsActionPressed(INPUT_UP))
		{
			velocity.Y -= 1;
		}
		else if (Input.IsActionPressed(INPUT_DOWN))
		{
			velocity.Y += 1;
		}

		// check that velocity vector isn't 0 so .Normalized() method doesn't break
		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * SPEED;
		}

		float sprite_height = PLAYER_SIZE.Y;
		float movement_range = SCREEN_SIZE.Y / 2 - sprite_height / 2 - MOVEMENT_CLAMP;

		Position += velocity * (float)delta; // move paddle based on velocity vector
		Position = new Vector2(
			x: Position.X,
			y: Mathf.Clamp(Position.Y, -movement_range, movement_range) // make sure paddle doesn't go off-screen
		);
	}
}