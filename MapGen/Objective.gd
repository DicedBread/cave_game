extends StaticBody2D

@onready var player = get_parent().get_node("Player")
@onready var toolTip = $ToolTip

@onready var startTimer = Timer.new() 

var buttonTime = 1.5
var health = 100
var time = Timer.new()

enum State {IDLE, DEFEND}
var state = State.IDLE

# Called when the node enters the scene tree for the first time.
func _ready():
	add_child(startTimer)
	add_child(time)
	toolTip.hide()
	startTimer.connect("timeout", Callable(startDefence))


func _process(delta):
	match state:
		State.IDLE:
			idle()
		State.DEFEND:
			pass
	pass

func idle():
	var dist = global_position.distance_to(player.global_position)
	if dist < 100:
		toolTip.show()
		track_time_button()
	else:
		toolTip.hide()




func track_time_button():
	if state == State.DEFEND: return
	if Input.is_action_just_pressed("e"):
		startTimer.start(buttonTime)
	if Input.is_action_just_released("e"):
		if not startTimer.is_stopped():
			interactFail()
			
func startDefence():
	state = State.DEFEND
	# print('success')
	toolTip.hide()
	startTimer.stop()
			
func interactFail():
	startTimer.stop()
	# print('fail')
