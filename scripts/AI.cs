using Godot;

public partial class AI : CharacterBody2D
{
	[Export] public int SPEED { get; set; } = 400;
	[Export] public int MOVEMENT_CLAMP { get; set; } = 40;

	public Vector2 SCREEN_SIZE;
	public Vector2 AI_SIZE;

	private void _on_restart()
	{
		// reset position of ai paddle
		Position = new Vector2(
			x: Position.X,
			y: 0
		);
	}

	public override void _Ready()
	{
		SCREEN_SIZE = GetViewportRect().Size;
		AI_SIZE = GetNode<Sprite2D>("Sprite2D").Texture.GetSize();
	}

	public override void _Process(double delta)
	{
		Vector2 velocity = Vector2.Zero; // velocity vector w/ no magnitude
		Vector2 ball_position = GetNode<Ball>("../ball").Position;

		// only move if ball in visible range
		if (ball_position.X < SCREEN_SIZE.X / 2 + AI_SIZE.X / 2)
		{
			// move based on ball position
			if (Position.Y > ball_position.Y)
			{
				velocity.Y -= 1;
			}
			else if (Position.Y < ball_position.Y)
			{
				velocity.Y += 1;
			}
		}

		// check that velocity vector isn't 0 so .Normalized() method doesn't break
		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * SPEED;
		}

		float sprite_height = AI_SIZE.Y;
		float movement_range = SCREEN_SIZE.Y / 2 - sprite_height / 2 - MOVEMENT_CLAMP;

		Position += velocity * (float)delta; // move paddle based on velocity vector
		Position = new Vector2(
			x: Position.X,
			y: Mathf.Clamp(Position.Y, -movement_range, movement_range) // make sure paddle doesn't go off-screen
		);
	}
}
