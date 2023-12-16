using Godot;
using System;

public partial class Turret : StaticBody2D
{

	Area2D target;


	public override void _Ready()
	{
		target = GetNode<Area2D>("Target");
		target.BodyEntered += HandelTarget;

		
	}

	public override void _Process(double delta)
	{
	}

	public void HandelTarget(Node2D b){
		GD.Print(b);
		if(b.IsInGroup("Enemy")){

		}
	}
}
