extends TileMap


@onready var win = get_viewport_rect().size

var tilePixelWidth = get_quadrant_size()
var width 
var height
var sc = 1

const OBJECTIVE_DIST = 100
var obj = preload("res://MapGen/Objective.tscn")
var objectives = []


var caveNoise:FastNoiseLite = FastNoiseLite.new()



# @onready var mat = get_material()

@onready var player = get_parent().get_node("Player")
# @onready var im = get_parent().get_node("Player/Camera2D/GridContainer/Sprite2D")
# @onready var objectiveNoise = FastNoiseLite.new()

# Called when the node enters the scene tree for the first time.
func _ready():


	get_tree().get_root().size_changed.connect(windowSizeUpdate)
	windowSizeUpdate()
	noiseParam()
	createObjective()
	# material.set_shader_parameter("playerLoc", player.position)
	# im.set_texture(ImageTexture.create_from_image(caveNoise.get_image(500, 500)))

func windowSizeUpdate():
	win = get_viewport_rect().size
	# mat.set_shader_parameter("screenDimen", win)
	width = (win.x / tilePixelWidth) + tilePixelWidth*2 
	height = (win.y / tilePixelWidth) + tilePixelWidth*2	


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	generateChunck(player.position)
	pass


func generateChunck(position:Vector2):
	var tilePos = local_to_map(position)
	for i in range(width):
		for j in range(height):
			var pos = Vector2(tilePos.x + i - width/2, tilePos.y + j - height/2)
			if(get_cell_tile_data(0, pos) != null): continue 
			
			var new = caveNoise.get_noise_2d(pos.x, pos.y)
			var tile
			var isWall = false
			tile = Vector2(1, 1)
			if(new > 0):
				isWall = true
				if(roundf(new * 10) == 0.0):
					set_cell(0, pos, 1, getWallOrientation(pos))
					continue
			set_cell(0, pos, isWall, tile)


func getWallOrientation(loc:Vector2) -> Vector2:
	var wallOrNot = [
	caveNoise.get_noise_2d(loc.x, loc.y - 1) < 0, #up
	caveNoise.get_noise_2d(loc.x + 1, loc.y) < 0, #right
	caveNoise.get_noise_2d(loc.x, loc.y + 1) < 0, #down
	caveNoise.get_noise_2d(loc.x - 1, loc.y) < 0, #left
	]
	var wallOrNotStr = "".join(wallOrNot)
	var dict = { #vile, vile, vile, vile
		"truefalsefalsefalse" = Vector2(4, 6),
		"falsetruefalsefalse" = Vector2(5, 4),
		"falsefalsetruefalse" = Vector2(4, 7),
		"falsefalsefalsetrue" = Vector2(6, 4),
		"truetruefalsefalse" = Vector2(5, 6),
		"falsetruetruefalse" = Vector2(5, 5),
		"falsefalsetruetrue" = Vector2(6, 5),
		"truefalsefalsetrue" = Vector2(6, 6),
	}
	if(dict.has(wallOrNotStr)):
		return dict[wallOrNotStr]
	return Vector2(1, 1)




#creates 3 objectives in walkable area of the map
func createObjective():
	for i in range(3):	
		var inst = obj.instantiate()
		var pos = getCoordInValidZone(randi_range(-OBJECTIVE_DIST, OBJECTIVE_DIST), randi_range(-OBJECTIVE_DIST, OBJECTIVE_DIST))
		inst.position = map_to_local(pos)
		add_child(inst)

func getCoordInValidZone(x:int, y:int) -> Vector2:
	var old = Vector2(x, y)
	var new = getSurroundingLowestNoiseCoord(old)
	while(old != new):
		old = new
		new = getSurroundingLowestNoiseCoord(old)
	return old

func getSurroundingLowestNoiseCoord(base:Vector2)->Vector2:
	var currVal = base
	var values = [
		Vector2(currVal.x + 1, currVal.y),
		Vector2(currVal.x - 1, currVal.y),
		Vector2(currVal.x, currVal.y + 1),
		Vector2(currVal.x, currVal.y - 1)		
	]
	var currN = caveNoise.get_noise_2d(currVal.x, currVal.y)
	for i in range(4):
		if(caveNoise.get_noise_2d(values[i].x, values[i].y) < currN):
			currVal = values[i]
			currN = caveNoise.get_noise_2d(values[i].x, values[i].y)
	return currVal	



func noiseParam():
	caveNoise.set_noise_type(FastNoiseLite.NoiseType.TYPE_PERLIN)
	caveNoise.set_cellular_jitter(0.45)
	caveNoise.set_domain_warp_frequency(0.5)
	caveNoise.set_fractal_gain(0.5)
	caveNoise.set_fractal_lacunarity(0.5)
	caveNoise.set_fractal_octaves(5)
	caveNoise.set_fractal_ping_pong_strength(0.5)
	caveNoise.set_fractal_weighted_strength(0.5)
	caveNoise.set_frequency(0.03) 
