using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Enemy : CharacterBody2D
{
	public const float Speed = 100.0f;
	public const float LookDist = 100.0f;
	public float health = 100;

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
	RayCast2D targetRay;

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
		
		setUpWeights();
		AVReady();
		setUp(ls);
		setUp(ls2);

		new Callable(this, "SetUpNav").CallDeferred();
    }

    public override void _PhysicsProcess(double delta)
	{	
		targetRay.TargetPosition = GlobalPosition.DirectionTo(TargetObjectiveLoc) * GlobalPosition.DistanceTo(TargetObjectiveLoc); 
		target.SetPointPosition(1, targetVel);
		vel.SetPointPosition(1, Velocity);	
		doThing();

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

	public void AvoidWallOld(double delta){
		if(!targetRay.IsColliding() || targetRay.GetCollisionPoint().DistanceTo(GlobalPosition) >= 100){
			state = State.TARGET;
		}

		List<Vector2> vecLeft = new List<Vector2>();
		List<Vector2> vecRight = new List<Vector2>();
		
		// scanner.TargetPosition = targetRay.TargetPosition.Rotated(Mathf.DegToRad(-45));
		for(int i = 0; i < 7; i++){
			float cuRot = 22.5f * i;
			Vector2 vLeft = targetRay.TargetPosition.Normalized().Rotated(Mathf.DegToRad(cuRot)) * 100;
			Vector2 vRight = targetRay.TargetPosition.Normalized().Rotated(Mathf.DegToRad(-cuRot)) * 100;
  
			scanner.TargetPosition = vLeft;
			ls[i].TargetPosition = vLeft;

			Vector2 vecToCol = vLeft;
			scanner.ForceRaycastUpdate();
			if(scanner.IsColliding()){
			 	vecToCol = GlobalPosition.DirectionTo(scanner.GetCollisionPoint()) * GlobalPosition.DistanceTo(scanner.GetCollisionPoint());
			}
			// GD.Print(vLeft + " "  + vecToCol + " "+ (int) (vecToCol.Length() * 100) / vLeft.Length());
			ls2[i].TargetPosition = vecToCol;
			vecLeft.Add(vecToCol);

			vecToCol = vRight;
			scanner.TargetPosition = vRight;
			scanner.ForceRaycastUpdate();
			if(scanner.IsColliding()){
				vecToCol = GlobalPosition.DirectionTo(scanner.GetCollisionPoint()) * GlobalPosition.DistanceTo(scanner.GetCollisionPoint());
			}
			vecRight.Add(vecToCol);
		}
		// GD.Print("");

		Vector2 dirLeft = vecLeft.Aggregate<Vector2>((e1, e2) => (e2.Length() > 50) ? e1 + e2 : e1);
		Vector2 dirRight = vecRight.Aggregate<Vector2>((e1, e2) => (e2.Length() > 50) ? e1 + e2 : e1);

		// Vector2 dirLeft = vecLeft.Aggregate<Vector2>((e1, e2) =>  e1 + e2);
		// Vector2 dirRight = vecRight.Aggregate<Vector2>((e1, e2) => e1 + e2);
		// bool leftIsClosestToTargetAngle = (dirLeft.AngleTo(targetVel) > dirRight.AngleTo(targetVel));
		// Vector2 final = (leftIsClosestToTargetAngle) ? dirLeft : dirRight; 

		// if(Velocity.Angle() == GlobalPosition.DirectionTo(targetRay.TargetPosition).Angle()){
		// 	GD.Print("dude");
		Vector2	final = (dirLeft.Length() > dirRight.Length()) ? dirLeft : dirRight;
		// }


		targetVel = final;

		float currentAngle = Velocity.Angle();
		float targetAngle = targetVel.Angle();
		float angle = Mathf.LerpAngle(currentAngle, targetAngle, 0.1f);
		Velocity = Velocity.Rotated(angle - currentAngle);
	}


	Dictionary<Vector2, float> weights = new Dictionary<Vector2, float>();

	public void setUpWeights(){
		float incrementAngle = 360/8;
		Vector2 baseVec = new Vector2(1, 0);
		for(int i = 0; i < 8; i++){
			Vector2 v = baseVec.Rotated(Mathf.RadToDeg(incrementAngle * i)) * LookDist;
			weights.Add(v, 0);
		}
	}

	public void AvoidWall(double delta){
		if(!targetRay.IsColliding() || targetRay.GetCollisionPoint().DistanceTo(GlobalPosition) >= 100){
			state = State.TARGET;
		}

		Vector2 v = setWeights();

		targetVel = v.Normalized() * Speed;

		float currentAngle = Velocity.Angle();
		float targetAngle = targetVel.Angle();
		float angle = Mathf.LerpAngle(currentAngle, targetAngle, 0.1f);
		Velocity = Velocity.Rotated(angle - currentAngle);
	}

	public Vector2 setWeights(){
		Vector2 o = weights.Keys.ToList()[0];
		float highestWeight = weights[o];
		foreach (Vector2 v in weights.Keys)
		{
			float w = weights[v];
			float angleToTarget = Mathf.RadToDeg(v.Angle() - targetRay.TargetPosition.Angle());

			scanner.TargetPosition = v;
			scanner.ForceRaycastUpdate();
			
			float howCloseIsCol = 1;
			if(scanner.IsColliding()){
				howCloseIsCol = scanner.GetCollisionPoint().DistanceTo(GlobalPosition) / 100;
			}

			float sumWeight = Mathf.Max(howCloseIsCol + angleToTarget - 1, 0);
			weights[v] = sumWeight;
			if(sumWeight > highestWeight){
				o = v;
			}
		}
		return o;
	}

	List<RayCast2D> ls = new List<RayCast2D>();
	List<RayCast2D> ls2 = new List<RayCast2D>();
	

	public void setUp(List<RayCast2D> l){
		for(int i = 0; i < 7; i++){
			RayCast2D r = new RayCast2D();
			AddChild(r);
			l.Add(r);
		}
	}

	public void doThing(){
		// for(int i = 0; i < 7; i++){
		// 	float cuRot = 22.5f * i;
		// 	Vector2 vLeft = targetRay.TargetPosition.Normalized().Rotated(Mathf.DegToRad(cuRot)) * 90;
		// 	ls[i].TargetPosition = vLeft;
		// }
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
