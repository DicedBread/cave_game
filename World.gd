extends Node


@onready var currentNoise:FastNoiseLite = FastNoiseLite.new()
@onready var map = get_tree().get_root().get_node("Node2D/ProceduralMap")
@onready var player = get_tree().get_root().get_node("Node2D/Player")

const OBJECTIVE_DIST = 100
var obj = preload("res://MapGen/Objective.tscn")
var objectives = []

# Called when the node enters the scene tree for the first time.
func _ready():
	noiseParam()
	createObjective()

	# var path = createPath(player.global_position, objectives[0].global_position)
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


#creates 3 objectives in walkable area of the map
func createObjective():
	for i in range(3):	
		var inst = obj.instantiate()
		objectives.append(inst)
		var pos = getCoordInValidZone(randi_range(-OBJECTIVE_DIST, OBJECTIVE_DIST), randi_range(-OBJECTIVE_DIST, OBJECTIVE_DIST))
		inst.global_position = map.map_to_local(pos)
		map.add_child(inst)
		var vec = Vector2(1000, 1000)
		map.genNavArea(inst.global_position - (vec/2), vec)

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
	var currN = World.currentNoise.get_noise_2d(currVal.x, currVal.y)
	for i in range(4):
		if(World.currentNoise.get_noise_2d(values[i].x, values[i].y) < currN):
			currVal = values[i]
			currN = World.currentNoise.get_noise_2d(values[i].x, values[i].y)
	return currVal	


func createPath(a1:Vector2, a2:Vector2)-> Array:
	var n1 = map.local_to_map(a1)
	var n2 = map.local_to_map(a2)
	
	var path = []
	n1.append(0)

	return path



func noiseParam():
	currentNoise.set_noise_type(FastNoiseLite.NoiseType.TYPE_PERLIN)
	currentNoise.set_cellular_jitter(0.45)
	currentNoise.set_domain_warp_frequency(0.5)
	currentNoise.set_fractal_gain(0.5)
	currentNoise.set_fractal_lacunarity(0.5)
	currentNoise.set_fractal_octaves(5)
	currentNoise.set_fractal_ping_pong_strength(0.5)
	currentNoise.set_fractal_weighted_strength(0.5)
	currentNoise.set_frequency(0.03) 


func closestObjPos(pos:Vector2)->Vector2:
	var closestPos = objectives[0].global_position
	var dist = pos.distance_to(objectives[0].global_position)
	for i in range(objectives.size()):
		var currPos = objectives[i].global_position
		var currDist = pos.distance_to(currPos)
		if(currDist < dist):
			dist = currDist
			closestPos = currPos
	return closestPos
