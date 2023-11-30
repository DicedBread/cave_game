extends Area2D

var velocity = Vector2()
var damage = 0
var pierce = 1

@onready var unitPos = position

# Called when the node enters the scene tree for the first time.
func _ready():
	connect("body_entered", Callable(body_entered))
	connect("area_entered", Callable(area_entered))
	# body_entered.connect(on_body_entered)
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(delta):
	position = position + velocity * delta
	if(position.distance_to(unitPos) > 1000):
		queue_free()
	# isHit()
	pass

func setVelocity(newVelocity):
	unitPos = position
	velocity = newVelocity
	rotate(velocity.angle())

func setDamage(newDamage):
	damage = newDamage

# func setPierce(newPierce):
# 	pierce = newPierce

# func setKnockback(newKnockback):
# 	#knockback = newKnockback
# 	pass

# ##TODO: should scale by size
# func setSize(newSize):
# 	#size = newSize
# 	pass

# func onHit():
# 	pierce -= 1
# 	if pierce <= 0:
# 		queue_free()

func body_entered(body):
	if body.is_in_group("Enemy"):
		body.damage(damage)
		queue_free()
		

	if(body.is_in_group("Map")):
		queue_free()
	

func area_entered(area):
	pass

func _on_timer_timeout():
	queue_free()
