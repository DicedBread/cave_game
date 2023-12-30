using Godot;
using System;

public partial class Bullet : Area2D
{
	private static PackedScene scene = GD.Load<PackedScene>("res://Weapon/Bullet.tscn");
	
	public static Bullet Initialize(Vector2 location, Vector2 vel){
		Bullet b = scene.Instantiate<Bullet>();
		b.GlobalPosition = location;
		b.velocity = vel;
		b.Rotate(vel.Angle());
		return b;
	}

	Vector2 velocity;
    private Vector2 initPos;

    public float MaxTravelDist { get; private set; } = 1000;


    // Called when the node enters the scene tree for the first time.

    public override void _Ready()
	{
		initPos = GlobalPosition;
	}

    public override void _PhysicsProcess(double delta){
		Position = Position + (velocity * (float) delta);
		if(Position.DistanceTo(initPos) > MaxTravelDist){
			QueueFree();
		}

    }
}
