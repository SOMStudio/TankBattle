using UnityEngine;
using System.Collections;
using CharacterStates;

[AddComponentMenu("Base/Character/Third Person")]

public class BaseTopDown : ExtendedCustomMonoBehaviour 
{
	[Header("Main value")]
	[SerializeField]
	private float walkSpeed= 2.0f; // The speed when walking
	[SerializeField]
	private float runSpeed= 4.0f; // after runAfterSeconds of walking we run with runSpeed
	[SerializeField]
	private float speedSmoothing= 10.0f;
	[SerializeField]
	private float rotateSpeed= 500.0f;
	[SerializeField]
	private float runAfterSeconds= 3.0f;
	[SerializeField]
	private bool moveDirectionally;

	[Header("Animation")]
	[SerializeField]
	protected AnimationClip idleAnimation;
	[SerializeField]
	protected AnimationClip walkAnimation;
	[SerializeField]
	protected float walkMaxAnimationSpeed = 0.75f;
	[SerializeField]
	protected float runMaxAnimationSpeed = 1.0f;
	[SerializeField]
	protected float walkTimeStart= 0.0f;
	[SerializeField]
	protected Animation _animation;

	private CharacterState _characterState;
	// The current move direction in x-z
	private Vector3 moveDirection= Vector3.zero;
	// The current x-z move speed
	private float moveSpeed= 0.0f;

	[Header("Player Manager")]
	[SerializeField]
	protected BasePlayerManager myPlayerController; 
	protected Keyboard_Input default_input;
	
	// The last collision flags returned from controller.Move
	private CollisionFlags collisionFlags;
	
	public float horz;
	public float vert;
	
	private CharacterController controller;

	private Vector3 targetDirection;
	private float curSmooth;
	private float targetSpeed;
	private float curSpeed;
	private Vector3 forward;
	private Vector3 right;

	// main event
			
	void Awake ()
	{
		moveDirection = transform.TransformDirection (Vector3.forward);
		
		// if _animation has not been set up in the inspector, we'll try to find it on the current gameobject
		if (_animation == null)
			_animation = GetComponent<Animation> ();
		
		if (!_animation)
			Debug.Log ("The character you would like to control doesn't have animations. Moving her might look weird.");
		
		if (!idleAnimation) {
			_animation = null;
			Debug.Log ("No idle animation found. Turning off animations.");
		}
		if (!walkAnimation) {
			_animation = null;
			Debug.Log ("No walk animation found. Turning off animations.");
		}

		controller = GetComponent<CharacterController> ();
	}

	void Update ()
	{	
		UpdateState ();
	}

	void LateUpdate()
	{
		// we check for input in LateUpdate because Unity recommends this
		if (canControl)
			GetInput ();
	}

	// main logic

	public override void Init ()
	{
		base.Init ();
		
		// add default keyboard input
		if (!default_input) {
			default_input = myGO.AddComponent<Keyboard_Input> ();
		}
		
		// cache a reference to the player controller
		if (!myPlayerController) {
			myPlayerController = myGO.GetComponent<BasePlayerManager> ();
		}
		
		if(myPlayerController!=null)
			myPlayerController.Init();
	}
	
	public void SetUserInput( bool setInput )
	{
		canControl = setInput;	
	}
	
	protected virtual void GetInput()
	{
		horz = Mathf.Clamp (default_input.GetHorizontal (), -1, 1);
		vert = Mathf.Clamp (default_input.GetVertical (), -1, 1);
	}

	void  UpdateSmoothedMovementDirection ()
	{			
		if (moveDirectionally) {
			UpdateDirectionalMovement ();
		} else {
			UpdateRotationMovement ();
		}
	}
	
	void UpdateDirectionalMovement()
	{
		// find target direction
		targetDirection = horz * Vector3.right;
		targetDirection += vert * Vector3.forward;
		
		// We store speed and direction seperately,
		if (targetDirection != Vector3.zero) {
			moveDirection = Vector3.RotateTowards (moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
			moveDirection = moveDirection.normalized;
		}
		
		// Smooth the speed based on the current target direction
		curSmooth = speedSmoothing * Time.deltaTime;
		
		// Choose target speed
		//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
		targetSpeed = Mathf.Min (targetDirection.magnitude, 1.0f);
	
		_characterState = CharacterState.Idle;
		
		// decide on animation state and adjust move speed
		if (Time.time - runAfterSeconds > walkTimeStart) {
			targetSpeed *= runSpeed;
			_characterState = CharacterState.Running;
		} else {
			targetSpeed *= walkSpeed;
			_characterState = CharacterState.Walking;
		}
		
		moveSpeed = Mathf.Lerp (moveSpeed, targetSpeed, curSmooth);
		
		// Reset walk time start when we slow down
		if (moveSpeed < walkSpeed * 0.3f)
			walkTimeStart = Time.time;
			
		// Calculate actual motion
		Vector3 movement = moveDirection * moveSpeed;
		movement *= Time.deltaTime;
		
		// Move the controller
		collisionFlags = controller.Move (movement);
		
		// Set rotation to the move direction
		myTransform.rotation = Quaternion.LookRotation (moveDirection);
	}
	
	void UpdateRotationMovement ()
	{
		myTransform.Rotate (0, horz * rotateSpeed * Time.deltaTime, 0);
		curSpeed = moveSpeed * vert;
		controller.SimpleMove (myTransform.forward * curSpeed);

		// Target direction (the max we want to move, used for calculating target speed)
		targetDirection = vert * myTransform.forward;
				
		// Smooth the speed based on the current target direction
		float curSmooth = speedSmoothing * Time.deltaTime;
		
		// Choose target speed
		//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
		targetSpeed = Mathf.Min (targetDirection.magnitude, 1.0f);
	
		_characterState = CharacterState.Idle;
		
		// decide on animation state and adjust move speed
		if (Time.time - runAfterSeconds > walkTimeStart) {
			targetSpeed *= runSpeed;
			_characterState = CharacterState.Running;
		} else {
			targetSpeed *= walkSpeed;
			_characterState = CharacterState.Walking;
		}
		
		moveSpeed = Mathf.Lerp (moveSpeed, targetSpeed, curSmooth);
		
		// Reset walk time start when we slow down
		if (moveSpeed < walkSpeed * 0.3f)
			walkTimeStart = Time.time;
		
	}
	
	void UpdateState ()
	{	
		if (!canControl) {
			// kill all inputs if not controllable.
			Input.ResetInputAxes ();
		}
		
		UpdateSmoothedMovementDirection ();
		
		// ANIMATION sector
		if (_animation) {
			if (controller.velocity.sqrMagnitude < 0.1f) {
				_animation.CrossFade (idleAnimation.name);
			} else {
				if (_characterState == CharacterState.Running) {
					_animation [walkAnimation.name].speed = Mathf.Clamp (controller.velocity.magnitude, 0.0f, runMaxAnimationSpeed);
					_animation.CrossFade (walkAnimation.name);	
				} else if (_characterState == CharacterState.Walking) {
					_animation [walkAnimation.name].speed = Mathf.Clamp (controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
					_animation.CrossFade (walkAnimation.name);	
				}
			}
		}	
	}
	
	public float GetSpeed ()
	{
		return moveSpeed;
	}
		
	public Vector3 GetDirection ()
	{
		return moveDirection;
	}
	
	public bool IsMoving ()
	{
		 return Mathf.Abs(vert) + Mathf.Abs(horz) > 0.5f;
	}
}