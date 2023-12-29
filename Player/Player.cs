using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;

	AnimatedSprite2D sprite;
	int lastDir = 1;

	public override void _Ready(){
		sprite = GetNode<AnimatedSprite2D>("Sprite");
	}

	public override void _PhysicsProcess(double delta){
		float dirX = Input.GetAxis("a", "d");
		float dirY = Input.GetAxis("w", "s");


		if(dirX != 0 || dirY != 0){
			Velocity = new Vector2(dirX, dirY) * Speed;
		}else{
			Velocity = Velocity.MoveToward(new Vector2(0f,0f), Speed);
		}
		Animation();
		MoveAndSlide();
	}


	public void Animation(){
		sprite.FlipH = lastDir < 0;
		if(Velocity.X > 0){
			lastDir = 1;
		}else{
			lastDir = -1;
		}

		if(Velocity.X != 1 || Velocity.Y != 0){
			sprite.Play("run");
		}else{
			sprite.Play("idle");
		}
	}
}
