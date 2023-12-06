extends TileMap


@onready var win = get_viewport_rect().size

var tilePixelWidth = tile_set.get_tile_size().x
var width 
var height
var sc = 1

var nav = tile_set.get_navigation_layer_layers(0)


const OBJECTIVE_DIST = 100
var obj = preload("res://MapGen/Objective.tscn")
var objectives = []



@onready var player = get_parent().get_node("Player")

# Called when the node enters the scene tree for the first time.
func _ready():
	get_tree().get_root().size_changed.connect(windowSizeUpdate)
	windowSizeUpdate()
	# createObjective()
	# caveNoise.set_seed(randi())


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
			
			var new = World.currentNoise.get_noise_2d(pos.x, pos.y)
			var isWall = false
			var tile = Vector2(1, 1)
			if(new > 0):
				isWall = true
				# if(roundf(new * 10) == 0.0):
				# 	set_cell(0, pos, 1, getWallOrientation(pos))
					# set_cell(0, pos, 1, Vector2(0,0))

					# continue
			set_cell(0, pos, isWall, tile)


func genNavArea(p1:Vector2, size:Vector2):
	var tileP1 = local_to_map(p1)
	var tileP2 = local_to_map(p1 + size)

	for i in range(tileP1.x, tileP2.x):
		for j in range(tileP1.y, tileP2.y):
			var pos = Vector2(i, j)
			# if(get_cell_tile_data(0, pos) != null): continue 
			var new = World.currentNoise.get_noise_2d(pos.x, pos.y)
			var isWall = false
			var tile = Vector2(1, 0)
			if(new > 0):
				isWall = true
				tile = Vector2(1, 1)
			set_cell(0, pos, isWall, tile)

func getWallOrientation(loc:Vector2) -> Vector2:
	var wallOrNot = [
		World.currentNoise.get_noise_2d(loc.x, loc.y - 1) < 0, #up
		World.currentNoise.get_noise_2d(loc.x + 1, loc.y) < 0, #right
		World.currentNoise.get_noise_2d(loc.x, loc.y + 1) < 0, #down
		World.currentNoise.get_noise_2d(loc.x - 1, loc.y) < 0, #left
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




# #creates 3 objectives in walkable area of the map
# func createObjective():
# 	for i in range(3):	
# 		var inst = obj.instantiate()
# 		var pos = getCoordInValidZone(randi_range(-OBJECTIVE_DIST, OBJECTIVE_DIST), randi_range(-OBJECTIVE_DIST, OBJECTIVE_DIST))
# 		inst.position = map_to_local(pos)
# 		add_child(inst)

# func getCoordInValidZone(x:int, y:int) -> Vector2:
# 	var old = Vector2(x, y)
# 	var new = getSurroundingLowestNoiseCoord(old)
# 	while(old != new):
# 		old = new
# 		new = getSurroundingLowestNoiseCoord(old)
# 	return old

# func getSurroundingLowestNoiseCoord(base:Vector2)->Vector2:
# 	var currVal = base
# 	var values = [
# 		Vector2(currVal.x + 1, currVal.y),
# 		Vector2(currVal.x - 1, currVal.y),
# 		Vector2(currVal.x, currVal.y + 1),
# 		Vector2(currVal.x, currVal.y - 1)		
# 	]
# 	var currN = World.currentNoise.get_noise_2d(currVal.x, currVal.y)
# 	for i in range(4):
# 		if(World.currentNoise.get_noise_2d(values[i].x, values[i].y) < currN):
# 			currVal = values[i]
# 			currN = World.currentNoise.get_noise_2d(values[i].x, values[i].y)
# 	return currVal	





# func get_noise_along(v1:Vector2, v2:Vector2)->float:
# 	var vec1 = local_to_map(v1)
# 	var vec2 = local_to_map(v2)

# 	var vec1 = 

func getNoiseAt(vec:Vector2)->float:
	var v = local_to_map(vec)
	return World.currentNoise.get_noise_2d(v.x, v.y)

