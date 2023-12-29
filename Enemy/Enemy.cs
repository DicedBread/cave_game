using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : CharacterBody2D
{
	public const float Speed = 100.0f;
	public float health = 100;

	private static PackedScene scene = GD.Load<PackedScene>("res://Enemy/Enemy.tscn");

	private Dictionary<Vector2, float> avoidWall = new Dictionary<Vector2, float>();

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
	RayCast2D targetRay;
	// RayCast2D rayRight = new RayCast2D();
	// RayCast2D rayLeft= new RayCast2D();


	enum State {WALKING, IDLE, TARGET, AVOID_WALL, FOLMOUSE}
	State state = State.TARGET;

	Vector2 targetVel = new Vector2(1,0);

	Line2D target;
	Line2D vel;

	public Vector2 TargetObjectiveLoc {get ; set;}

    public override async void _Ready(){
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		targetRay = GetNode<RayCast2D>("TargetRay");
		target = GetNode<Line2D>("Target");
		vel = GetNode<Line2D>("Vel");

		world = GetParent<World>();

		sprite.SpriteFrames = (SpriteFrames) GD.Load(sprites[new Random().Next(0, sprites.Length)]);
		Velocity = new Vector2(0, Speed);
		
		AVReady();

		new Callable(this, "SetUpNav").CallDeferred();
    }

    public override void _PhysicsProcess(double delta)
	{	
		targetRay.TargetPosition = GlobalPosition.DirectionTo(TargetObjectiveLoc) * GlobalPosition.DistanceTo(TargetObjectiveLoc); 
		

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
		if(targetRay.GetCollisionPoint().DistanceTo(GlobalPosition) < 100){
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

	RayCast2D scanner;
	RayCast2D farTar;
	float dist = 100;

	private void AVReady(){
		scanner = GetNode<RayCast2D>("Scanner");
		farTar = GetNode<RayCast2D>("FarTar");
		setUpAvWall();
	}

	public void setUpAvWall(){

	}

	public void AvoidWall(double delta){
		if(targetRay.GetCollisionPoint().DistanceTo(GlobalPosition) >= 100){
			state = State.TARGET;
		}

		// Vector2 baseDir = GlobalPosition.DirectionTo(targetRay.TargetPosition).Normalized();
		// float dist = targetRay.GetCollisionPoint().DistanceTo(GlobalPosition);
		// Vector2 nDir = GlobalPosition.DirectionTo(targetRay.GetCollisionPoint());
		// for(int i = 0; i < 90; i+=2){
		// 	scanner.TargetPosition = baseDir.Rotated(Mathf.DegToRad(-(90/2) + i)) * 200;
		// 	scanner.ForceRaycastUpdate();
		// 	Vector2 v = scanner.GetCollisionPoint();
		// 	float d = v.DistanceSquaredTo(GlobalPosition);
		// 	if(d > dist){
		// 		dist = d;
		// 		nDir = GlobalPosition.DirectionTo(scanner.GetCollisionPoint());
		// 	}
		// }
		// farTar.TargetPosition = nDir.Normalized() * 100;
		// targetVel = nDir;

		List<Vector2> vecLeft = new List<Vector2>();
		List<Vector2> vecRight = new List<Vector2>();
		
		// scanner.TargetPosition = targetRay.TargetPosition.Rotated(Mathf.DegToRad(-45));
		for(int i = 0; i < 5; i++){
			float cuRot = 22.5f * i;
			Vector2 vLeft = targetRay.TargetPosition.Normalized().Rotated(Mathf.DegToRad(cuRot));
			Vector2 vRight = targetRay.TargetPosition.Normalized().Rotated(Mathf.DegToRad(-cuRot));

			scanner.TargetPosition = vLeft;
			scanner.ForceRaycastUpdate();
			scanner.GetCollisionPoint();

			
		
		
		}



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

	public void damage(int dmg, Vector2 v){
		health -= dmg;
	}
}
