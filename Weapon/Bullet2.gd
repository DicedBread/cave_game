extends "Bullet.gd"


# Called when the node enters the scene tree for the first time.
func _ready():
	super()
	pass # Replace with function body.



func body_entered(body):
	if body.is_in_group("Player"):
		body.damage(damage)
		queue_free()

	if(body.is_in_group("Map")):
		queue_free()
