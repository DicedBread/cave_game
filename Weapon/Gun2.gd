extends "gun.gd"

@onready var timer = Timer.new()
@onready var yep = get_parent()

# Called when the node enters the scene tree for the first time.
func _ready():
	super()
	add_child(timer)
	timer.connect("timeout", Callable(shoot))
	timer.start(randf_range(0.1, 0.5))
	timer.start()

	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func shoot():
	singleShot(yep.velocity.normalized())
	timer.start(randf_range(0.1, 0.5))
	timer.start()
