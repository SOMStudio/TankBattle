using UnityEngine;
using CharacterStates;

[AddComponentMenu("Base/Character/Third Person")]
public class BaseTopDown : ExtendedCustomMonoBehaviour
{
	[Header("Main value")] [SerializeField]
	private float walkSpeed = 2.0f;

	[SerializeField] private float runSpeed = 4.0f;
	[SerializeField] private float speedSmoothing = 10.0f;
	[SerializeField] private float rotateSpeed = 500.0f;
	[SerializeField] private float runAfterSeconds = 3.0f;
	[SerializeField] private bool moveDirectionally;

	[Header("Animation")] [SerializeField] protected AnimationClip idleAnimation;
	[SerializeField] protected AnimationClip walkAnimation;
	[SerializeField] protected float walkMaxAnimationSpeed = 0.75f;
	[SerializeField] protected float runMaxAnimationSpeed = 1.0f;
	[SerializeField] protected float walkTimeStart = 0.0f;
	[SerializeField] protected Animation _animation;

	private CharacterState _characterState;
	private Vector3 moveDirection = Vector3.zero;
	private float moveSpeed;

	[Header("Player Manager")] [SerializeField]
	protected BasePlayerManager myPlayerController;

	protected Keyboard_Input default_input;

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

	private void Awake()
	{
		moveDirection = transform.TransformDirection(Vector3.forward);

		if (_animation == null)
			_animation = GetComponent<Animation>();

		if (!_animation)
			Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");

		if (!idleAnimation)
		{
			_animation = null;
			Debug.Log("No idle animation found. Turning off animations.");
		}

		if (!walkAnimation)
		{
			_animation = null;
			Debug.Log("No walk animation found. Turning off animations.");
		}

		controller = GetComponent<CharacterController>();
	}

	private void Update()
	{
		UpdateState();
	}

	private void LateUpdate()
	{
		if (canControl)
			GetInput();
	}

	public override void Init()
	{
		base.Init();

		if (!default_input)
		{
			default_input = myGO.AddComponent<Keyboard_Input>();
		}

		if (!myPlayerController)
		{
			myPlayerController = myGO.GetComponent<BasePlayerManager>();
		}

		if (myPlayerController != null)
			myPlayerController.Init();
	}

	public void SetUserInput(bool setInput)
	{
		canControl = setInput;
	}

	protected virtual void GetInput()
	{
		horz = Mathf.Clamp(default_input.GetHorizontal(), -1, 1);
		vert = Mathf.Clamp(default_input.GetVertical(), -1, 1);
	}

	private void UpdateSmoothedMovementDirection()
	{
		if (moveDirectionally)
		{
			UpdateDirectionalMovement();
		}
		else
		{
			UpdateRotationMovement();
		}
	}

	private void UpdateDirectionalMovement()
	{
		targetDirection = horz * Vector3.right;
		targetDirection += vert * Vector3.forward;

		if (targetDirection != Vector3.zero)
		{
			moveDirection = Vector3.RotateTowards(moveDirection, targetDirection,
				rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
			moveDirection = moveDirection.normalized;
		}

		curSmooth = speedSmoothing * Time.deltaTime;

		targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);

		_characterState = CharacterState.Idle;

		if (Time.time - runAfterSeconds > walkTimeStart)
		{
			targetSpeed *= runSpeed;
			_characterState = CharacterState.Running;
		}
		else
		{
			targetSpeed *= walkSpeed;
			_characterState = CharacterState.Walking;
		}

		moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);

		if (moveSpeed < walkSpeed * 0.3f)
			walkTimeStart = Time.time;

		Vector3 movement = moveDirection * moveSpeed;
		movement *= Time.deltaTime;

		collisionFlags = controller.Move(movement);

		myTransform.rotation = Quaternion.LookRotation(moveDirection);
	}

	private void UpdateRotationMovement()
	{
		myTransform.Rotate(0, horz * rotateSpeed * Time.deltaTime, 0);
		curSpeed = moveSpeed * vert;
		controller.SimpleMove(myTransform.forward * curSpeed);

		targetDirection = vert * myTransform.forward;

		float curSmooth = speedSmoothing * Time.deltaTime;

		targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);

		_characterState = CharacterState.Idle;

		if (Time.time - runAfterSeconds > walkTimeStart)
		{
			targetSpeed *= runSpeed;
			_characterState = CharacterState.Running;
		}
		else
		{
			targetSpeed *= walkSpeed;
			_characterState = CharacterState.Walking;
		}

		moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);

		if (moveSpeed < walkSpeed * 0.3f)
			walkTimeStart = Time.time;
	}

	private void UpdateState()
	{
		if (!canControl)
		{
			Input.ResetInputAxes();
		}

		UpdateSmoothedMovementDirection();

		if (_animation)
		{
			if (controller.velocity.sqrMagnitude < 0.1f)
			{
				_animation.CrossFade(idleAnimation.name);
			}
			else
			{
				if (_characterState == CharacterState.Running)
				{
					_animation[walkAnimation.name].speed =
						Mathf.Clamp(controller.velocity.magnitude, 0.0f, runMaxAnimationSpeed);
					_animation.CrossFade(walkAnimation.name);
				}
				else if (_characterState == CharacterState.Walking)
				{
					_animation[walkAnimation.name].speed =
						Mathf.Clamp(controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
					_animation.CrossFade(walkAnimation.name);
				}
			}
		}
	}

	public float GetSpeed()
	{
		return moveSpeed;
	}

	public Vector3 GetDirection()
	{
		return moveDirection;
	}

	public bool IsMoving()
	{
		return Mathf.Abs(vert) + Mathf.Abs(horz) > 0.5f;
	}
}