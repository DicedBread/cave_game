using Godot;
using System;
using System.Linq;

public partial class Turret : StaticBody2D
{

	[Export(PropertyHint.Range, "1,200,")]
	private float maxRange = 200;

	[Export(PropertyHint.Range, "0.1, 20")]
	private float fireRate = 0.3f;

	Area2D targetArea;
	CollisionShape2D shape;
	CircleShape2D targetCir;

	RayCast2D ray;
	Enemy target;

	Timer timer = new Timer();


	public override void _Ready(){
		targetArea = GetNode<Area2D>("Target");
		targetArea.BodyEntered += getClosestTar;
		targetArea.BodyExited += getClosestTar;
		targetCir = (CircleShape2D) targetArea.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		targetCir.Radius = maxRange;
		ray = GetNode<RayCast2D>("RayCast2D");
		timer.Timeout += Shoot;
		timer.WaitTime = fireRate;
		AddChild(timer);
		timer.Start();
	}

    public override void _PhysicsProcess(double delta){
		if(target != null){
			ray.TargetPosition = GlobalPosition.DirectionTo(target.GlobalPosition) * GlobalPosition.DistanceTo(target.GlobalPosition);
			targetCir.Radius = MathF.Min(GlobalPosition.DistanceTo(target.GlobalPosition), maxRange);
		}
	}

	public void getClosestTar(Node2D b){
		Enemy e = (Enemy) targetArea.GetOverlappingBodies().Where(e => e.IsInGroup("Enemy")).MaxBy(e => GlobalPosition.DistanceTo(e.GlobalPosition));
		if(e == null){
			target =null;
			return;
		};
		Vector2 v = GlobalPosition.DirectionTo(e.GlobalPosition) * GlobalPosition.DistanceTo(e.GlobalPosition);
		ray.TargetPosition =  v; 
		target = e;
	}

	public void Shoot(){
		if(target == null) return;
		GD.Print("yep");
		Bullet b = Bullet.Initialize(GlobalPosition, GlobalPosition.DirectionTo(target.GlobalPosition) * 1000);
		GetParent().AddChild(b);
	}
}
