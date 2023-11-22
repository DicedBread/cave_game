extends Node2D


@onready var projectileSpawn 

@export var damage:int
@export_range(0.1, 1) var speed: float = 0.5
@export var reloadTime:float
@export_range(0.1, 100) var fireRate:float
@export var projectile:PackedScene

var leftMouseDown = false
var fireRateLock = false
var reloadLock = false


func _ready():
	projectileSpawn = $ProjectileSpawn
	pass # Replace with function body.


func _process(delta):
	if(leftMouseDown and !fireRateLock and !reloadLock):
		singleShot((get_global_mouse_position() - projectileSpawn.global_position).normalized())
		fired()

	pass



func _input(event):
	# if(fireMode != fireModes.MANUAL_AIM): pass;
	if event is InputEventMouseButton:
		if event.button_index == 1 and event.is_pressed():
			leftMouseDown = true
		elif event.button_index == 1 and not event.is_pressed():
			pass
			leftMouseDown = false
			# triggerReleased()


##create single projectile moving in direction 
func singleShot(direction:Vector2):
	var shot = projectile.instantiate()
	get_tree().root.add_child(shot)
	shot.position = projectileSpawn.global_position
	shot.setVelocity(direction * speed * 1000)
	shot.setDamage(damage)



##Shoots in direction based on spread and shot count
# func shoot(direction:Vector2):
# 	for i in range(shotCount):
# 		var spreadAngle = randf_range(deg_to_rad(-spread/2), deg_to_rad(spread/2))
# 		var spreadDirection = direction.rotated(spreadAngle)
# 		singleShot(spreadDirection);
# 	magCount -= 1 
# 	if magCount <= 0:
# 		reload()
# 	else:
# 		fireRateTimer()
# 	pass


func fired():
	fireRateLocker()


func reloadLocker():
	reloadLock = true
	await get_tree().create_timer(reloadTime).timeout
	reloadLock = false

func fireRateLocker():
	fireRateLock = true
	await get_tree().create_timer(fireRate).timeout
	fireRateLock = false
