extends Node2D


var navMap:RID


@onready var e = $Enemy
@onready var o = $Objective


# Called when the node enters the scene tree for the first time.
func _ready():
	# call_deferred("setUpNavMap")

	# print( e.position)
	# print( o.position)
	pass # Replace with function body.


func setUpNavMap():
	navMap = NavigationServer2D.map_create()
	var region:RID = NavigationServer2D.region_create()
	NavigationServer2D.region_set_transform(region, Transform2D())
	NavigationServer2D.region_set_map(region, navMap)
	

	await get_tree().physics_frame
	var path = NavigationServer2D.map_get_path(navMap, e.position, o.position, true)
	print(path)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
