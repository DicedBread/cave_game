using Godot;
using System;

public partial class Bullet : Area2D
{
	private static PackedScene scene = GD.Load<PackedScene>("res://Enemy/Bullet.tscn");
	
	public static void Initialize(){
		
	}



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

    public override void _PhysicsProcess(double delta){

    }
}
