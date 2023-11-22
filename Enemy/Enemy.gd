extends CharacterBody2D


const SPEED = 300.0

# var velocity = Vector2()

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")

@onready var nav = $NavigationAgent2D
@onready var obj = get_node("/root/Node2D/Objective")



func _ready():
	nav.velocity_computed.connect(Callable(self, "velocity_computed"))

	print(global_position)
	print(obj.global_position)
	# get_world_2d().get_navigation_map().
	call_deferred("navSetup")
	# navSetup()
	pass


func _physics_process(delta):
	if nav.is_navigation_finished():
		return

	var nextPos = nav.get_next_path_position()
	var currPos = global_position
	var newVel = (nextPos - currPos).normalized() * SPEED
	if nav.avoidance_enabled:
		newVel = nav.get_avoidance_velocity(newVel)
	else:
		velocity_computed(newVel) 

		
func velocity_computed(safe_vel: Vector2):
	velocity = safe_vel
	move_and_slide()
	
func navSetup():
	nav.set_target_position(obj.global_position)


