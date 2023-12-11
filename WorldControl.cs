using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class WorldControl : Node
{
    private const int OBJECTIVE_DIST = 100;
    private const int OBJECTIVE_COUNT = 3; 
	private FastNoiseLite worldNoise;
    private PackedScene obj = GD.Load<PackedScene>("res://MapGen/Objective.tscn");
	private Dictionary<Vector2, StaticBody2D> objectives = new Dictionary<Vector2, StaticBody2D>();


    public override void _Ready()
    {
        worldNoise = new FastNoiseLite();
		// noiseParam(worldNoise);
    }

	// returns location of closest objective to vec
	public Vector2 GetClosestTargetLocation(Vector2 vec){
		GD.Print(objectives.Keys.ToList<Vector2>().Count + " bruh");
		Vector2 curOut = objectives.Keys.ToList<Vector2>()[0];
		double curDist = vec.DistanceTo(curOut); 
		foreach(Vector2 v in objectives.Keys){
			double dist = v.DistanceTo(vec);
			if(dist < curDist){
				curOut = v;
				curDist = dist;	
			}
		}
		GD.Print(curOut + " ferojkn");
		

		return curOut;
	}
    



}