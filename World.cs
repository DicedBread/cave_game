using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public partial class World : Node2D
{
	private const int OBJECTIVE_DIST = 100;
    private const int OBJECTIVE_COUNT = 3;
    private ProceduralMap map;
	private FastNoiseLite worldNoise;
	public FastNoiseLite WorldNoise {get {return worldNoise;} set{return;}}
	public int heheh = 1;
	private CharacterBody2D player;
	private PackedScene obj = GD.Load<PackedScene>("res://MapGen/Objective.tscn");
	private Dictionary<Vector2, StaticBody2D> objectives = new Dictionary<Vector2, StaticBody2D>();


	[Signal]
	public delegate void noiseUpdateEventHandler();

	[Signal]
	public delegate void setTargetEventHandler(Vector2 v); 


	public override void _Ready()
	{
		map = GetNode<ProceduralMap>("ProceduralMap");
		worldNoise = new FastNoiseLite();
		noiseParam(worldNoise);

		player = GetNode<CharacterBody2D>("Player");
		createObjectives();
		// Enemy e = GetNode<Enemy>("Enemy");
		// new Callable(e, "SetTarget").CallDeferred(GetClosestTargetLocation(e.GlobalPosition));
		// e.SetTarget(GetClosestTargetLocation(e.GlobalPosition));
		// Connect(SignalName.noiseUpdate, new Callable(e, "SetTarget"));
		// EmitSignal(SignalName.noiseUpdate, GetClosestTargetLocation(e.GlobalPosition));
	}
	

    public override void _Process(double delta){
		map.generateChunck(player.GlobalPosition, worldNoise);
    }

    public void createObjectives(){
		// TODO stop duplicate pos
		for(int i =0; i< OBJECTIVE_COUNT; i++){
			StaticBody2D inst = obj.Instantiate<StaticBody2D>();
			Random r = new Random();
			Vector2I pos = getCoordInValidZone(r.Next(-OBJECTIVE_DIST, OBJECTIVE_DIST), r.Next(-OBJECTIVE_DIST, OBJECTIVE_DIST));
			Vector2 worldPos = map.MapToLocal(pos);
			inst.GlobalPosition = worldPos;
			objectives.Add(worldPos, inst);
			AddChild(inst);
			Vector2I v = new Vector2I(1000, 1000);
			map.genNavArea(inst.GlobalPosition, v, worldNoise);
		}
	}

	// searches for lower noise values till current has no lower noise values as neighbors 
	public Vector2I getCoordInValidZone(int x, int y){
		Vector2I oldCord = new Vector2I(x, y);
		Vector2I newCord = getLowestNoiseNeighbor(oldCord);
		while(oldCord != newCord){
			oldCord = newCord;
			newCord = getLowestNoiseNeighbor(oldCord);
		}
		return oldCord;		
	}

	// checks if 2dNoise value has a neighbor with a lower noise value and return vector to lowest
	public Vector2I getLowestNoiseNeighbor(Vector2I v){
		Vector2I currVal = v;
		float currN = worldNoise.GetNoise2Dv(v);
		Vector2I[] values = {
			new Vector2I(v.X + 1, v.Y), //N
			new Vector2I(v.X - 1, v.Y), //S
			new Vector2I(v.X , v.Y + 1), //E
			new Vector2I(v.X , v.Y - 1), //W
		};
		foreach(Vector2I vec in values){
			float n = worldNoise.GetNoise2Dv(vec);
			if(n < currN){
				currVal = vec;
				currN = n;
			}
		}
		return currVal;
	}

	// returns location of closest objective to vec
	public Vector2 GetClosestTargetLocation(Vector2 vec){
		Vector2 curOut = objectives.Keys.ToList<Vector2>()[0];
		double curDist = vec.DistanceTo(curOut); 
		foreach(Vector2 v in objectives.Keys){
			double dist = v.DistanceTo(vec);
			if(dist < curDist){
				curOut = v;
				curDist = dist;	
			}
		}
		return curOut;
	}

	// set params for noise
	public void noiseParam(FastNoiseLite noise){
		noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
		noise.CellularJitter = 0.45f;
		noise.DomainWarpFrequency = 0.5f;
		noise.FractalGain = 0.5f;
		noise.FractalLacunarity = 0.5f;
		noise.FractalOctaves = 5;
		noise.FractalPingPongStrength = 0.5f;
		noise.FractalWeightedStrength = 0.5f;
		noise.Frequency = 0.03f;
	}


	
	public override void _UnhandledInput(InputEvent @event){
		if(@event is InputEventMouseButton e)
			if(e.IsPressed() && e.ButtonIndex == MouseButton.Right){
				Enemy em = Enemy.Instantiate();
				em.GlobalPosition = GetGlobalMousePosition();
				// em.SetTarget(GetClosestTargetLocation(em.GlobalPosition));
				AddChild(em);
			}
	}


	public float getWorldNoiseAt(Vector2 v){
		return worldNoise.GetNoise2Dv(map.LocalToMap(v));
	}

}
