using Godot;
using System;
using System.Security.Principal;

public partial class ProceduralMap : TileMap
{
	private readonly Vector2I wall = new Vector2I(1, 1); 
	private readonly Vector2I floor = new Vector2I(1, 1); 
	private readonly Vector2I navFloor = new Vector2I(1, 0);


	Vector2 winSize;
	private float width;
	private float height;
	int tilePxWidth;


	CharacterBody2D player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tilePxWidth = TileSet.TileSize.X;
		windowSizeUpdate();
		GetTree().Root.SizeChanged += windowSizeUpdate;

		player = GetParent().GetNode<CharacterBody2D>("Player"); // could change to signal possibly
	}

	private void windowSizeUpdate(){
		if(tilePxWidth <= 0) throw new ArgumentOutOfRangeException();
		winSize = GetViewportRect().Size;
		width = (winSize.X / tilePxWidth) + tilePxWidth * 2;
		height = (winSize.Y / tilePxWidth) + tilePxWidth * 2;
	}


	public void generateChunck(Vector2 center, FastNoiseLite noise){
		if (noise == null) return;
		Vector2 tilePos = LocalToMap(center);
		for(int i = 0; i < width; i++){
			for(int j = 0; j < height; j++){
				Vector2I pos = new Vector2I((int) (tilePos.X + i - width/2), (int) (tilePos.Y + j - height/2));
				if(GetCellTileData(0, pos) != null) continue;
				float noiseVal = noise.GetNoise2Dv(pos);
				if(noiseVal > 0){
					SetCell(0, pos, 1, wall);
				}else{
					SetCell(0, pos, 0, floor);
				}
			}
		}
	}

	
	public void genNavArea(Vector2 center, Vector2 size, FastNoiseLite noise){
		Vector2I tileP1 = LocalToMap(new Vector2I((int)(center.X - size.X/2), (int)(center.Y - size.Y/2)));
		Vector2I tileP2 = LocalToMap(new Vector2I((int)(center.X + size.X/2), (int)(center.Y + size.Y/2)));
		Vector2I pos;
		for(int i = tileP1.X; i < tileP2.X; i++){
			for(int j = tileP1.Y; j < tileP2.Y; j++){
				pos = new Vector2I(i, j);
				float noiseVal = noise.GetNoise2Dv(pos);
				if(noiseVal > 0){
					SetCell(0, pos, 1, wall);
				}else{
					SetCell(0, pos, 0, navFloor);
				}
			}
		}
	}

}
