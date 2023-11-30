extends Node2D

@onready var e = preload("res://Enemy/Enemy.tscn")

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _input(event):
	# if(fireMode != fireModes.MANUAL_AIM): pass;
	if event is InputEventMouseButton:
		if event.button_index == 1 and event.is_pressed():
			var em = e.instantiate()
			add_child(em)
			em.global_position = get_global_mouse_position()

		elif event.button_index == 1 and not event.is_pressed():
			pass
			# triggerReleased()
