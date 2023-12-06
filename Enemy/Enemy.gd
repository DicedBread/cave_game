extends CharacterBody2D

var health = 100
const SPEED = 100.0

@onready var part = $GPUParticles2D
@onready var sprite = $AnimatedSprite2D
@onready var sprites = ["res://Enemy/Sprites/BlackSprite.tres", "res://Enemy/Sprites/OrangeSprite.tres", "res://Enemy/Sprites/GreySprite.tres"]
@onready var navigation_agent: NavigationAgent2D = $NavigationAgent2D
@onready var ray = $RayCast2D
@onready var rayLeft = RayCast2D.new() 
@onready var rayRight = RayCast2D.new()
@onready var tarRay = $Target
@onready var vel = $Vel

@onready var sword = $Sword


@onready var velRotNode 
var nav = []
@onready var map = get_tree().get_root().get_node("Node2D/ProceduralMap")

signal targetAngleReached
signal changeToTarget
var targetAngle = 0.0
var targetPos = Vector2(0, 0)
var targetVel = Vector2(0, 0)
var initVel = Vector2(0, 0)
var elapsed_time = 1.0

var currentTargetObjective

enum State {WALKING, TURN, IDLE, TARGET, AVOID_WALL, FOLMOUSE}
var state = State.TARGET

@onready var randTimer = Timer.new()

func rand():
	rando()
	add_child(randTimer)
	randTimer.connect("timeout", Callable(rando))
	randTimer.start(randf_range(1, 10))

func rando():
	targetVel = Vector2(1,0).rotated(deg_to_rad(randi_range(0, 360)))
	pass


func _ready():
	rand()
	sword.body_entered.connect(Callable(swordAttack))
	add_child(rayLeft)
	add_child(rayRight)
	connect("changeToTarget", func(): state = State.TARGET)
	rayLeft.position = Vector2(0, 0)
	rayRight.position = Vector2(0, 0)
	setUpNavPoints()
	sprite.sprite_frames = load(sprites[randi_range(0, sprites.size() - 1)])
	velocity = Vector2(SPEED, 0)
	navigation_agent.connect("navigation_finished", Callable(PathFound))
	connect("targetAngleReached", Callable(getAngle))
	currentTargetObjective = World.closestObjPos(global_position)
	call_deferred("actor_setup")

func _physics_process(delta):
	# print(state)
	tarRay.points[1] = targetVel
	vel.points[1] = velocity
	var dir = global_position.direction_to(currentTargetObjective) 
	ray.target_position = dir * global_position.distance_to(currentTargetObjective)
	rayLeft.target_position = dir.rotated(deg_to_rad(-45)) * 100
	rayRight.target_position = dir.rotated(deg_to_rad(45)) * 100
	match state:
		State.WALKING:
			walk(delta)
		State.TURN:
			turn(delta)
		State.IDLE:
			idle(delta)	
		State.TARGET:
			target(delta)
		State.AVOID_WALL:
			avoidWall(delta)
		State.FOLMOUSE:
			followMouse(delta)
	animation()	
	var rot = velocity.angle()
	velRotNode.rotation = rot
	# sprite.rotation = -rot


	sword.rotation = sword.rotation + 5 * delta
	move_and_slide()

func swordAttack(body):
	if body == self:
		return
	if body.is_in_group("Enemy"):
		body.damage(10, Vector2(0,1).rotated(deg_to_rad(randi_range(0, 360))) * 100)

func walk(delta):

	# getAngle()
	# var rot = 0.0
	
	# var currentAngle = rad_to_deg(velocity.angle())
	# if currentAngle > (currentAngle + targetAngle):
	# 	rot += 60 * delta
	# elif currentAngle < (currentAngle + targetAngle):
	# 	rot -= 60 * delta

	var targetVelAngle = targetVel.angle()

	var currVelAngle = velocity.angle()
	var add	= lerp_angle(currVelAngle, targetVelAngle, 0.1)
	var dir = velocity.rotated(add - velocity.angle()) 
	velocity = dir
	# velocity = velocity.rotated(deg_to_rad(rot))
	
func turn(delta):
	pass

func idle(delta):
	pass

func setUpNavPoints():
	velRotNode = Node2D.new()
	# velRotNode.global_position = global_position
	add_child(velRotNode)

	for i in range(0,3):
		# nav.append(ColorRect.new())
		# nav[i].size = Vector2(2, 2)
		nav.append(Marker2D.new())
		velRotNode.add_child(nav[i])
	var offset = map.tile_set.tile_size.x
	nav[0].global_position = global_position + Vector2(offset/2, offset)
	nav[1].global_position = global_position + Vector2(offset, 0)
	nav[2].global_position = global_position + Vector2(offset/2, -offset)


func getAngle():
	var lef = map.local_to_map(nav[0].global_position)
	var fro = map.local_to_map(nav[1].global_position)
	var rig = map.local_to_map(nav[2].global_position)
	var f = World.currentNoise.get_noise_2d(fro.x, fro.y)
	var l = World.currentNoise.get_noise_2d(lef.x, lef.y)
	var r = World.currentNoise.get_noise_2d(rig.x, rig.y)

	# if f > l and f > r:
		# targetAngle = 0.0
		# targetVel = ve
	if l > r:
		targetVel = global_position.direction_to(nav[2].global_position) * SPEED
		# targetAngle = 45.0
	elif r > l:
		targetVel = global_position.direction_to(nav[0].global_position) * SPEED
		# targetAngle = -45.0

func getLowestAngle()->float:
	var lef = map.local_to_map(nav[0].global_position)
	var fro = map.local_to_map(nav[1].global_position)
	var rig = map.local_to_map(nav[2].global_position)
	var f = World.currentNoise.get_noise_2d(fro.x, fro.y)
	var l = World.currentNoise.get_noise_2d(lef.x, lef.y)
	var r = World.currentNoise.get_noise_2d(rig.x, rig.y)

	if f > -0.1:
		if l > r:
			return nav[0].position.angle()
		elif r > l:
			return nav[2].position.angle()
		else:
			return 0.0
	return 0.0


func damage(dmg, vel:Vector2):
	health -= dmg
	part.process_material.gravity = Vector3(vel.x, vel.y, 0)

	emmitFor(0.2)
	if health <= 0:
		queue_free()

func animation():

	var rot = rad_to_deg(velocity.angle())
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

func target(delta: float):
	if navigation_agent.is_target_reached():
		state = State.WALKING
		return

	if ray.get_collision_point().distance_to(global_position) < 100:
		state = State.AVOID_WALL
		return
	
	var targetPos = navigation_agent.get_next_path_position()
	targetVel = global_position.direction_to(targetPos) * SPEED

	var currVelAngle = velocity.angle()
	var targetVelAngle = targetVel.angle()
	var angle = lerp_angle(currVelAngle, targetVelAngle, 0.1)
	velocity = velocity.rotated(angle - velocity.angle()) 
	move_and_slide()
	
	pass

func avoidWall(delta: float):
	var off = ray.get_collision_point().distance_to(global_position)
	if off > 150:
		changeToTarget.emit()
		return
	velocity = global_position.direction_to(currentTargetObjective) * SPEED

	var distRight = rayRight.get_collision_point().distance_to(global_position)
	var distLeft = rayLeft.get_collision_point().distance_to(global_position)
	if velocity.angle() == ray.target_position.angle():

		if distRight > distLeft:
			targetPos = velocity.rotated(deg_to_rad(90))
		else:
			targetPos = velocity.rotated(deg_to_rad(-90))
	else:
		var isGoingLeft = velocity.angle() - ray.target_position.angle() > 0

		if isGoingLeft: 
			if distRight > distLeft:
				targetPos = velocity.rotated(deg_to_rad(90 * delta))
			else:
				targetPos = velocity.rotated(deg_to_rad(-90 * delta))
		
	var dir = global_position.direction_to(targetPos) * SPEED
	velocity = dir
	pass

func actor_setup():	
	await get_tree().physics_frame
	set_movement_target(currentTargetObjective)
	

func set_movement_target(movement_target: Vector2):
	navigation_agent.target_position = movement_target

func PathFound():
	state = State.TARGET


func followMouse(delta):
	# if elapsed_time >= 1.0:

	var currVelAngle = velocity.angle()
	var targetVelAngle = targetVel.angle()
	var add	= lerp_angle(currVelAngle, targetVelAngle, 0.1)
	var dir = velocity.rotated(add - velocity.angle()) 
	velocity = dir

func _unhandled_input(event):
	if event is InputEventMouseMotion:
		# print("Mouse Motion at: ", event.position)
		if state == State.FOLMOUSE:
			targetPos = get_global_mouse_position()
			targetVel = global_position.direction_to(targetPos) * SPEED


func emmitFor(t:float):
	var timer = Timer.new()
	add_child(timer)
	timer.connect("timeout", func(): part.emitting = false; timer.queue_free())
	part.emitting = true
	timer.start(t)
