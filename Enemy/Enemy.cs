using Godot;
using System;
using System.Threading.Tasks;

public partial class Enemy : CharacterBody2D
{
	public const float Speed = 100.0f;

	World world;

	AnimatedSprite2D sprite;
	String[] sprites = {"res://Enemy/Sprites/BlackSprite.tres", "res://Enemy/Sprites/OrangeSprite.tres", "res://Enemy/Sprites/GreySprite.tres"};

	NavigationAgent2D navAgent;
	RayCast2D ray;
	RayCast2D rayRight = new RayCast2D();
	RayCast2D rayLeft= new RayCast2D();


	enum State {WALKING, IDLE, TARGET, AVOID_WALL, FOLMOUSE}
	State state = State.TARGET;

	// Vector2 targetPos = new Vector2(1000,0);
	Vector2 targetVel = new Vector2(1,0);

	Line2D target;
	Line2D vel;

	public Vector2 TargetObjectiveLoc {get ; set;}

    public override async void _Ready(){
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		ray = GetNode<RayCast2D>("RayCast2D");
		target = GetNode<Line2D>("Target");
		vel = GetNode<Line2D>("Vel");

		world = GetParent<World>();

		sprite.SpriteFrames = (SpriteFrames) GD.Load(sprites[new Random().Next(0, sprites.Length)]);
		Velocity = new Vector2(0, Speed);
		
		AddChild(rayLeft);
		AddChild(rayRight);

		TargetObjectiveLoc = new Vector2(1, 1);
    }

    public override void _PhysicsProcess(double delta)
	{
		ray.TargetPosition = GlobalPosition.DirectionTo(TargetObjectiveLoc) * GlobalPosition.DistanceTo(TargetObjectiveLoc); 
		target.SetPointPosition(1, targetVel);
		vel.SetPointPosition(1, Velocity);	

		switch (state)
		{
			case State.IDLE:
				break;
			case State.WALKING:
				break;
			case State.TARGET:
				Target(delta);
				break;
			case State.FOLMOUSE:
				FollowMouse(delta);
				break;
			default:
				break;
		}
		MoveAndSlide();
	}

	public void Target(double delta){

		if(navAgent.IsTargetReached()){
			state = State.WALKING;
			return;
		}
		// if(ray.GetCollisionPoint().DistanceTo(GlobalPosition) < 100){
		// 	state = State.AVOID_WALL;
		// 	return;
		// }

		Vector2 targetPos = navAgent.GetNextPathPosition();
		targetVel = GlobalPosition.DirectionTo(targetPos) * Speed;

		float currentAngle = Velocity.Angle();
		float targetAngle = targetVel.Angle();
		float angle = Mathf.LerpAngle(currentAngle, targetAngle, 0.1f);
		Velocity = Velocity.Rotated(angle - currentAngle);
	}

	public void FollowMouse(double delta){
		Vector2 targetPos = GetGlobalMousePosition();
		targetVel = GlobalPosition.DirectionTo(targetPos) * Speed;

		float currentAngle = Velocity.Angle();
		float targetAngle = targetVel.Angle();
		float angle = Mathf.LerpAngle(currentAngle, targetAngle, 0.1f);
		Velocity = Velocity.Rotated(angle - currentAngle);
	}


	private async Task SetUpNav(Vector2 v){
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		navAgent.TargetPosition = v;
	}

	public async void SetTarget(Vector2 v){
		TargetObjectiveLoc = v;
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		navAgent.TargetPosition = v;

	}

	private float GetNoise2Dv(Vector2 v){
		return world.getWorldNoiseAt(v);
	}
}
