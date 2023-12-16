using Godot;
using System;
using System.Threading.Tasks;

public partial class Enemy : CharacterBody2D
{
	public const float Speed = 100.0f;

	private static PackedScene scene = GD.Load<PackedScene>("res://Enemy/Enemy.tscn");

	public static Enemy Instantiate(){
		return scene.Instantiate<Enemy>();
	}

	public static Enemy Instantiate(Vector2 pos){
		Enemy o = scene.Instantiate<Enemy>();
		o.GlobalPosition = pos;
		return o;
	}

	World world;

	AnimatedSprite2D sprite;
	String[] sprites = {"res://Enemy/Sprites/BlackSprite.tres", "res://Enemy/Sprites/OrangeSprite.tres", "res://Enemy/Sprites/GreySprite.tres"};

	NavigationAgent2D navAgent;
	RayCast2D ray;
	RayCast2D rayRight = new RayCast2D();
	RayCast2D rayLeft= new RayCast2D();


	enum State {WALKING, IDLE, TARGET, AVOID_WALL, FOLMOUSE}
	State state = State.TARGET;

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
		
		// AddChild(rayLeft);
		// AddChild(rayRight);

		// TargetObjectiveLoc = new Vector2(1, 1);

		new Callable(this, "SetUpNav").CallDeferred();
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
			case State.AVOID_WALL:
				AvoidWall(delta);
				break;
			default:
				break;
		}
		Animation();
		MoveAndSlide();
	}

	public void Target(double delta){

		if(navAgent.IsTargetReached()){
			state = State.WALKING;
			return;
		}
		if(ray.GetCollisionPoint().DistanceTo(GlobalPosition) < 100){
			state = State.AVOID_WALL;
			return;
		}

		Vector2 targetPos = navAgent.GetNextPathPosition();
		targetVel = GlobalPosition.DirectionTo(targetPos) * Speed;

		float currentAngle = Velocity.Angle();
		float targetAngle = targetVel.Angle();
		float angle = Mathf.LerpAngle(currentAngle, targetAngle, 0.1f);
		Velocity = Velocity.Rotated(angle - currentAngle);
	}

	public void AvoidWall(double delta){
		Velocity = new Vector2(0, 0);
	}

	public void FollowMouse(double delta){
		Vector2 targetPos = GetGlobalMousePosition();
		targetVel = GlobalPosition.DirectionTo(targetPos) * Speed;

		float currentAngle = Velocity.Angle();
		float targetAngle = targetVel.Angle();
		float angle = Mathf.LerpAngle(currentAngle, targetAngle, 0.1f);
		Velocity = Velocity.Rotated(angle - currentAngle);
	}


	private async void SetUpNav(){
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		Vector2 obj = world.GetClosestTargetLocation(GlobalPosition);
		// Vector2 obj = GlobalPosition;
		navAgent.TargetPosition = obj;
		TargetObjectiveLoc = obj;
	}

	public void SetTarget(Vector2 v){
		// await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		TargetObjectiveLoc = v;
		// GD.Print(TargetObjectiveLoc);
		// GD.Print("setTar "+ navAgent.Name);
	}


	public void Animation(){
		bool isNorth = Velocity.Y < 0;

		float rot = Mathf.Abs(Mathf.RadToDeg(Velocity.Angle()));
		if(rot < 45.0f){
			sprite.Play("walkEast");
			return;
		}
		if(rot > 135.0f){
			sprite.Play("walkWest");
		}

		if(rot > 22.5f && rot < 67.5f){
			String val = isNorth ? "walkNorthEast" : "walkSouthEast"; 
			sprite.Play(val);
		}

		if(rot > 112.5f && rot < 157.5f){
			String val = isNorth ? "walkNorthWest" : "walkSouthWest";
			sprite.Play(val);
		}

		if(rot > 67.5 && rot < 112.5){
			String val = isNorth ? "walkNorth" : "walkSouth";
			sprite.Play(val);
		} 

	}

	private float GetNoise2Dv(Vector2 v){
		return world.getWorldNoiseAt(v);
	}
}
