extends CharacterBody2D


const SPEED = 300.0

@onready var sprite = $Sprite
var lastDir = 1
# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")


func _physics_process(delta):

	# Get the input dirX and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	var dirX = Input.get_axis("a", "d")
	var dirY = Input.get_axis("w", "s")
	
	if dirX or dirY:
		velocity.y = dirY * SPEED
		velocity.x = dirX * SPEED
	else:
		velocity.y = move_toward(velocity.y, 0, SPEED)
		velocity.x = move_toward(velocity.x, 0, SPEED)
	animation()
	move_and_slide()


func animation():
	sprite.flip_h = lastDir < 0
	if(velocity.x > 0):
		lastDir = 1
	elif(velocity.x < 0):
		lastDir = -1


	if velocity.x != 0 or velocity.y != 0:
		sprite.play("run")
	else:
		sprite.play("idle")