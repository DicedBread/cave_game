extends Node2D


@onready var cam = $Player/Camera2D
@onready var map = $proceduralMap

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass



func _input(event):
	# print("jsng")
	if event is InputEventMouseButton:
		if event.is_pressed():
			# zoom in
			if event.button_index == MOUSE_BUTTON_WHEEL_UP:
				cam.zoom = cam.zoom + Vector2(0.1, 0.1)
				map.sc = 1 - cam.zoom.x
				# call the zoom function
			# zoom out
			if event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
				cam.zoom = cam.zoom - Vector2(0.1, 0.1)
				map.sc = 1 - cam.zoom.x

