using Godot;
using System;



public partial class Objective : StaticBody2D
{

	private SplitContainer toolTip;

	private const float BUTTON_TIME = 1.5f; 
	private Timer startTimer = new Timer();


	private int health = 100;
	private Timer timer = new Timer();

	enum State {IDLE, DEFEND}
	State state = State.IDLE;

	public override void _Ready()
	{
		toolTip = GetNode<SplitContainer>("ToolTip");
		startTimer.Timeout += startDefence;
		toolTip.Hide();
	}

	public override void _Process(double delta)
	{
		switch (state)
		{
			case State.IDLE:
				break;
			
			case State.DEFEND:
				break;


			default:
				break;
		}
	}

	public void idle(){

	}

	private void TrackTime(){
		if(state == State.DEFEND) return;
		if(Input.IsActionJustPressed("e")){
			startTimer.Start();
		}
		if(Input.IsActionJustReleased("e")){
			if(startTimer.IsStopped()){
				InteractFail();
			}
		}
	}

	private void startDefence(){
		state = State.DEFEND;
		toolTip.Hide();
		startTimer.Stop();
	}

	private void InteractFail(){
		startTimer.Stop();
	}
}
