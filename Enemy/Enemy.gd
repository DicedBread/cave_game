extends CharacterBody2D


const SPEED = 100.0

@onready var left
@onready var right 
@onready var front 

@onready var area = $area

@onready var sprite = $AnimatedSprite2D
@onready var sprites = ["res://Enemy/Sprites/BlackSprite.tres", "res://Enemy/Sprites/OrangeSprite.tres"]

@onready var walkTimer = Timer.new()
var walkForward = true

# @onready var obj = get_node("/root/Node2D/Objective")

@onready var map = get_tree().get_root().get_node("Node2D/ProceduralMap")

var health = 100

var targetVel = velocity
var targetAngle = 0.0
var currentAngle = 0.0

enum State {WALKING, TURN, IDLE}

var state = State.WALKING

func _ready():
	area.connect("body_entered", Callable(turn))
	area.connect("area_entered", Callable(stopTurn))
	setUpNavMarkers()
	sprite.play("walkEast")
	# velocity = Vector2(0, SPEED)
	setUpWalkTimer()
	var testAngle = 90 + 45 + 44
	velocity = Vector2(SPEED, 0).rotated(deg_to_rad(testAngle))
	sprite.sprite_frames = load(sprites[randi_range(0, sprites.size() - 1)])
	pass


func _physics_process(delta):
	getDirection()
	if currentAngle > targetAngle:
		currentAngle -= 60 * delta
	elif currentAngle < targetAngle:
		currentAngle += 60 * delta
	velocity = Vector2(SPEED, 0).rotated(deg_to_rad(currentAngle))
	rotation = velocity.angle()
	sprite.rotation = -velocity.angle()
	doTheThing(rad_to_deg(velocity.angle()))
	move_and_slide()
	
	pass


func setUpNavMarkers():
	front =Marker2D.new()
	left = Marker2D.new()
	right =Marker2D.new()
	add_child(front)
	add_child(left)
	add_child(right)
	var offset = map.get_quadrant_size()
	front.global_position = self.global_position + Vector2(offset, 0)
	left.global_position = self.global_position + Vector2(offset/2, offset)
	right.global_position  = self.global_position + Vector2(offset/2, -offset)

func getDirection():
	var fro = map.local_to_map(front.global_position)
	var rig = map.local_to_map(right.global_position)
	var lef = map.local_to_map(left.global_position)
	var f = map.caveNoise.get_noise_2d(fro.x, fro.y)
	var l = map.caveNoise.get_noise_2d(lef.x, lef.y)
	var r = map.caveNoise.get_noise_2d(rig.x, rig.y)


	if state == State.WALKING:
		pass

	if f > -0.1:
		walkForward = false

	if walkForward:
		targetAngle = currentAngle
		return

	if l > r:
		targetAngle = currentAngle - 45
	elif r > l:
		targetAngle = currentAngle + 45

func turn(body):
	if body.is_in_group("Enemy"):
		walkForward = false
		walkTimer.stop()
		targetAngle += 180
	pass

func stopTurn(body):
	if area.get_overlapping_bodies().filter(func(x): return x.is_in_group("Enemy")).size() <= 0:
		walkTimer.start()

func damage(dmg):
	health -= dmg
	if health <= 0:
		queue_free()
	pass

func doTheThing(rot:float):
	var off = 0.0
	if rot < (off + 45.0/2) and rot > (off -45.0/2):
		sprite.play("walkEast")
		pass

	off += 45.0
	if rot < (off + 45.0/2) and rot > (off -45.0/2):
		sprite.play("walkSouthEast")
		pass
	
	off += 45.0
	if rot < (off + 45.0/2) and rot > (off -45.0/2):
		sprite.play("walkSouth")
		pass

	off += 45.0
	if rot < (off + 45.0/2) and rot > (off -45.0/2):
		sprite.play("walkSouthWest")
		pass
	
	off = 0.0
	off -= 45.0
	if rot < (off + 45.0/2) and rot > (off - 45.0/2):
		sprite.play("walkNorthEast")
		pass

	off -= 45.0
	if rot < (off + 45.0/2) and rot > (off -45.0/2):
		sprite.play("walkNorth")
		pass

	off -= 45.0
	if rot < (off + 45.0/2) and rot > (off -45.0/2):
		sprite.play("walkNorthWest")
		pass

	if rot < (-180 + 45.0/2) or rot > (180 - 45.0/2):
		sprite.play("walkWest")
		pass
	
	pass
	
func setUpWalkTimer():
	add_child(walkTimer)
	walkTimer.set_wait_time(1.0)
	walkTimer.connect("timeout", Callable(toggleWalk))
	walkTimer.start()
	pass

func toggleWalk():
	walkForward = !walkForward
	walkTimer.set_wait_time(randf_range(2.0, 10.0))
