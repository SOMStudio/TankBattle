using SOMStudio.TankBattle.Scripts.Enums;
using SOMStudio.TankBattle.Scripts.Tools.Input;
using UnityEngine;

namespace SOMStudio.TankBattle.Scripts.Base
{
	[AddComponentMenu("SOMStudio/TankBattle/Base/Top-Down Person")]
	public class BaseTopDown : ExtendedCustomMonoBehaviour
	{
		[Header("Main value")]
		[SerializeField] private float walkSpeed = 2.0f;
		[SerializeField] private float runSpeed = 4.0f;
		[SerializeField] private float speedSmoothing = 10.0f;
		[SerializeField] private float rotateSpeed = 500.0f;
		[SerializeField] private float runAfterSeconds = 3.0f;
		[SerializeField] private bool moveDirectionally;

		[Header("Animation")]
		[SerializeField] protected AnimationClip idleAnimation;
		[SerializeField] protected AnimationClip walkAnimation;
		[SerializeField] protected float walkMaxAnimationSpeed = 0.75f;
		[SerializeField] protected float runMaxAnimationSpeed = 1.0f;
		[SerializeField] protected float walkTimeStart;
		[SerializeField] protected Animation refAnimation;

		private CharacterState characterState;
		private Vector3 moveDirection = Vector3.zero;
		private float moveSpeed;

		[Header("Player Manager")]
		[SerializeField] protected BasePlayerManager myPlayerController;

		protected KeyboardInput defaultInput;

		private CollisionFlags collisionFlags;

		public float horizontal;
		public float vertical;

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

			if (refAnimation == null)
				refAnimation = GetComponent<Animation>();

			if (!refAnimation)
				Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");

			if (!idleAnimation)
			{
				refAnimation = null;
				Debug.Log("No idle animation found. Turning off animations.");
			}

			if (!walkAnimation)
			{
				refAnimation = null;
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

			if (!defaultInput)
			{
				defaultInput = myGameObject.AddComponent<KeyboardInput>();
			}

			if (!myPlayerController)
			{
				myPlayerController = myGameObject.GetComponent<BasePlayerManager>();
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
			horizontal = Mathf.Clamp(defaultInput.GetHorizontal(), -1, 1);
			vertical = Mathf.Clamp(defaultInput.GetVertical(), -1, 1);
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
			targetDirection = horizontal * Vector3.right;
			targetDirection += vertical * Vector3.forward;

			if (targetDirection != Vector3.zero)
			{
				moveDirection = Vector3.RotateTowards(moveDirection, targetDirection,
					rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
				moveDirection = moveDirection.normalized;
			}

			curSmooth = speedSmoothing * Time.deltaTime;

			targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);

			characterState = CharacterState.Idle;

			if (Time.time - runAfterSeconds > walkTimeStart)
			{
				targetSpeed *= runSpeed;
				characterState = CharacterState.Running;
			}
			else
			{
				targetSpeed *= walkSpeed;
				characterState = CharacterState.Walking;
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
			myTransform.Rotate(0, horizontal * rotateSpeed * Time.deltaTime, 0);
			curSpeed = moveSpeed * vertical;
			controller.SimpleMove(myTransform.forward * curSpeed);

			targetDirection = vertical * myTransform.forward;

			float currentSmooth = speedSmoothing * Time.deltaTime;

			targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);

			characterState = CharacterState.Idle;

			if (Time.time - runAfterSeconds > walkTimeStart)
			{
				targetSpeed *= runSpeed;
				characterState = CharacterState.Running;
			}
			else
			{
				targetSpeed *= walkSpeed;
				characterState = CharacterState.Walking;
			}

			moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, currentSmooth);

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

			if (refAnimation)
			{
				if (controller.velocity.sqrMagnitude < 0.1f)
				{
					refAnimation.CrossFade(idleAnimation.name);
				}
				else
				{
					if (characterState == CharacterState.Running)
					{
						refAnimation[walkAnimation.name].speed =
							Mathf.Clamp(controller.velocity.magnitude, 0.0f, runMaxAnimationSpeed);
						refAnimation.CrossFade(walkAnimation.name);
					}
					else if (characterState == CharacterState.Walking)
					{
						refAnimation[walkAnimation.name].speed =
							Mathf.Clamp(controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
						refAnimation.CrossFade(walkAnimation.name);
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
			return Mathf.Abs(vertical) + Mathf.Abs(horizontal) > 0.5f;
		}
	}
}