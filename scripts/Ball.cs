using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;

public partial class Ball : CharacterBody2D
{
	[Export] public int SPEED { get; set; } = 600;
	[Export] public int STARTING_DISTANCE { get; set; } = 200;

	public Vector2 SCREEN_SIZE;
	public Vector2 BALL_SIZE;
	public Vector2 BALL_VELOCITY;
	public Random RANDOM = new Random();

	[Signal] public delegate void scoreEventHandler(int player_number);

	private async void _on_restart()
	{
		// reset ball position before 2 second pause when restarting
		float random_number = (float)RANDOM.NextDouble();
		int random_side = random_number > 0.5 ? 1 : -1;
		float random_y = (random_number - 0.5f) * (SCREEN_SIZE.Y / 2 - BALL_SIZE.Y / 2);

		Position = new Vector2(
			x: STARTING_DISTANCE * random_side,
			y: random_y
		);

		BALL_VELOCITY = new Vector2(0, 0); // hold ball in place before pause

		GetTree().Paused = true;
		await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
		GetTree().Paused = false;

		BALL_VELOCITY = new Vector2(-random_side, -1).Normalized() * SPEED; // set initial velocity towards other side
	}

	private async void NewRound()
	{
		// pause game for 2 seconds at start of every round
		GetTree().Paused = true;
		await ToSignal(GetTree().CreateTimer(2.0f), "timeout");
		GetTree().Paused = false;

		// on initialization, face ball towards random side of screen & assign random y position
		float random_number = (float)RANDOM.NextDouble();
		int random_side = random_number > 0.5 ? 1 : -1;
		float random_y = (random_number - 0.5f) * (SCREEN_SIZE.Y / 2 - BALL_SIZE.Y / 2);


		Debug.Print($"random_number: {random_number}");
		Debug.Print($"random_side: {random_side}");
		Debug.Print($"random_y: {random_y}");

		BALL_VELOCITY = new Vector2(-random_side, -1).Normalized() * SPEED; // set initial velocity towards other side

		Position = new Vector2(
			x: STARTING_DISTANCE * random_side,
			y: random_y
		);
	}

	public override void _Ready()
	{
		SCREEN_SIZE = GetViewportRect().Size;
		BALL_SIZE = GetNode<Sprite2D>("Sprite2D").Texture.GetSize();

		// always start the ball on right side of screen, moving top-left
		BALL_VELOCITY = new Vector2(-1, -1).Normalized() * SPEED;
	}

	public override void _Process(double delta)
	{
		// if colliding with screen top or bottom
		if (Math.Abs(Position.Y) >= Math.Abs(SCREEN_SIZE.Y / 2 - BALL_SIZE.Y / 2))
		{
			BALL_VELOCITY = new Vector2(BALL_VELOCITY.X, BALL_VELOCITY.Y * -1);
		}

		float screen_width = SCREEN_SIZE.X / 2;
		float horizontal_movement = Math.Abs(BALL_VELOCITY.X * (float)delta);

		// if going out of screen sides (scores) & is first frame outside of screen
		if (-screen_width - horizontal_movement < Position.X && Position.X <= -screen_width)
		{
			GetNode<AudioStreamPlayer2D>("audio_score").Play(); // play sound effect when score
			EmitSignal(SignalName.score, 1);
			NewRound();
		}
		else if (screen_width + horizontal_movement > Position.X && Position.X >= screen_width)
		{
			GetNode<AudioStreamPlayer2D>("audio_score").Play();
			EmitSignal(SignalName.score, 2);
			NewRound();
		}

		KinematicCollision2D collision = MoveAndCollide(BALL_VELOCITY * (float)delta); // move ball
		if (collision != null) // handle potential collision with paddle
		{
			Vector2 collision_normal = collision.GetNormal();

			// if colliding with paddle sides
			if (collision_normal.X == 1 || collision_normal.X == -1)
			{
				BALL_VELOCITY = new Vector2(BALL_VELOCITY.X * -1, BALL_VELOCITY.Y);
			}

			// if colliding with paddle top/bottom
			if (collision_normal.Y == 1 || collision_normal.Y == -1)
			{
				BALL_VELOCITY = new Vector2(BALL_VELOCITY.X, BALL_VELOCITY.Y * -1);
			}

			GetNode<AudioStreamPlayer2D>("audio_collide").Play(); // play sound effect whenever hitting paddle
		}
	}
}
