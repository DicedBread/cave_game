using Godot;
using System;

public partial class Main : Node
{

	Node2D world;



	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		world = GetNode<Node2D>("World");
		
	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
