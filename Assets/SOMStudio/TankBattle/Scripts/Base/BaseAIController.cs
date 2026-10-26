using SOMStudio.TankBattle.Scripts.Enums;
using SOMStudio.TankBattle.Scripts.Tools.Utility;
using UnityEngine;

namespace SOMStudio.TankBattle.Scripts.Base
{
	[AddComponentMenu("SOMStudio/TankBattle/Base/AI Controller")]
	public class BaseAIController : ExtendedCustomMonoBehaviour
	{
		[Header("Move")]
		[SerializeField] private float horz;
		[SerializeField] private float vert;

		private Vector3 relativeTarget;
		private float targetAngle;
		private RaycastHit hit;
		private Vector3 tempDirVec;
		private int obstacleHitType;

		[Header("Settings")]
		[SerializeField] private bool isStationary;
		[SerializeField] private AiState currentAIState;
		[SerializeField] private float modelRotateSpeed = 15f;
		[SerializeField] private int followTargetMaxTurnAngle = 120;

		[Header("Patrol")]
		[SerializeField] private float patrolSpeed = 5f;
		[SerializeField] private float patrolTurnSpeed = 10f;
		[SerializeField] private float wallAvoidDistance = 40f;

		[Header("Chase Target")]
		[SerializeField] private Transform followTarget;

		[SerializeField] private float minChaseDistance = 2f;
		[SerializeField] private float maxChaseDistance = 10f;
		[SerializeField] private float visionHeightOffset = 1f;

		[Header("Waypoint")]
		[SerializeField] private WaypointsController myWayControl;
		[SerializeField] private int currentWaypointNum;
		[SerializeField] private float waypointDistance = 5f;
		[SerializeField] private float moveSpeed = 30f;
		[SerializeField] private float pathSmoothing = 2f;
		[SerializeField] private bool shouldReversePathFollowing;
		[SerializeField] private bool loopPath;
		[SerializeField] private bool destroyAtEndOfWaypoints;
		[SerializeField] private bool faceWaypoints;
		[SerializeField] private bool startAtFirstWaypoint;

		[System.NonSerialized] public Transform currentWaypointTransform;

		private int totalWaypoints;

		private Vector3 nodePosition;
		private Vector3 myPosition;
		private float currentWayDist;

		[System.NonSerialized]
		public bool reachedLastWaypoint;

		private Vector3 moveVec;
		private Vector3 targetMoveVec;
		private float distanceToChaseTarget;

		[System.NonSerialized] public bool isRespawning;

		private int obstacleFinderResult;
		[SerializeField] private Transform rotateTransform;

		[System.NonSerialized] public Vector3 relativeWaypointPosition;

		public bool aiControlled = true;

		private void LateUpdate()
		{
			LateUpdateAIState();
		}

		public override void Init()
		{
			if (!didInit)
			{
				myGameObject = gameObject;

				myTransform = transform;

				if (rotateTransform == null)
					rotateTransform = myTransform;

				myBody = myTransform.GetComponent<Rigidbody>();

				didInit = true;
			}
		}

		public void SetCanControl(bool val)
		{
			canControl = val;
		}

		public void SetAIControl(bool val)
		{
			aiControlled = val;
		}

		public void SetPatrolSpeed(float aNum)
		{
			patrolSpeed = aNum;
		}

		public void SetPatrolTurnSpeed(float aNum)
		{
			patrolTurnSpeed = aNum;
		}

		public void SetWallAvoidDistance(float aNum)
		{
			wallAvoidDistance = aNum;
		}

		public void SetWaypointDistance(float aNum)
		{
			waypointDistance = aNum;
		}

		public void SetMoveSpeed(float aNum)
		{
			moveSpeed = aNum;
		}

		public void SetMinChaseDistance(float aNum)
		{
			minChaseDistance = aNum;
		}

		public void SetMaxChaseDistance(float aNum)
		{
			maxChaseDistance = aNum;
		}

		public void SetPathSmoothing(float aNum)
		{
			pathSmoothing = aNum;
		}

		public virtual void SetAIState(AiState newState)
		{
			currentAIState = newState;
		}

		public virtual void SetChaseTarget(Transform theTransform)
		{
			followTarget = theTransform;
		}

		protected virtual void LateUpdateAIState()
		{
			if (!didInit)
				Init();

			if (!aiControlled)
				return;

			UpdateAI();
		}

		protected virtual void UpdateAI()
		{
			horz = 0;
			vert = 0;

			if (followTarget != null)
			{
				obstacleFinderResult = IsObstacleAhead();
			}

			switch (currentAIState)
			{
				case AiState.moving_looking_for_target:
					if (followTarget != null)
						LookAroundFor(followTarget);

					if (obstacleFinderResult == 1)
					{
						SetAIState(AiState.stopped_turning_left);
					}

					if (obstacleFinderResult == 2)
					{
						SetAIState(AiState.stopped_turning_right);
					}

					if (obstacleFinderResult == 3)
					{
						SetAIState(AiState.backing_up_looking_for_target);
					}

					MoveForward();
					break;
				case AiState.chasing_target:
					if (followTarget == null)
						SetAIState(AiState.moving_looking_for_target);

					TurnTowardTarget(followTarget);

					distanceToChaseTarget = Vector3.Distance(myTransform.position, followTarget.position);

					if (distanceToChaseTarget > minChaseDistance)
					{
						MoveForward();
					}

					if (distanceToChaseTarget > maxChaseDistance || CanSee(followTarget) == false)
					{
						SetAIState(AiState.moving_looking_for_target);
					}

					break;
				case AiState.backing_up_looking_for_target:
					if (followTarget != null)
						LookAroundFor(followTarget);

					MoveBack();

					if (obstacleFinderResult == 0)
					{
						if (Random.Range(0, 100) > 50)
						{
							SetAIState(AiState.stopped_turning_left);
						}
						else
						{
							SetAIState(AiState.stopped_turning_right);
						}
					}

					break;
				case AiState.stopped_turning_left:
					if (followTarget != null)
						LookAroundFor(followTarget);

					TurnLeft();

					if (obstacleFinderResult == 0)
					{
						SetAIState(AiState.moving_looking_for_target);
					}

					break;
				case AiState.stopped_turning_right:
					if (followTarget != null)
						LookAroundFor(followTarget);

					TurnRight();

					if (obstacleFinderResult == 0)
					{
						SetAIState(AiState.moving_looking_for_target);
					}

					break;
				case AiState.paused_looking_for_target:
					if (followTarget != null)
						LookAroundFor(followTarget);
					break;
				case AiState.translate_along_waypoint_path:
					if (!didInit && !reachedLastWaypoint)
						return;

					UpdateWaypoints();

					if (!isStationary
					    && currentWaypointTransform != null)
					{
						targetMoveVec = Vector3.Normalize(currentWaypointTransform.position - myTransform.position);
						moveVec = Vector3.Lerp(moveVec, targetMoveVec, Time.deltaTime * pathSmoothing);
						myTransform.Translate(moveVec * (moveSpeed * Time.deltaTime));
						MoveForward();

						if (faceWaypoints)
						{
							TurnTowardTarget(currentWaypointTransform);
						}
					}

					break;
				case AiState.steer_to_waypoint:
					if (!didInit && !reachedLastWaypoint)
						return;

					UpdateWaypoints();

					if (currentWaypointTransform == null)
					{
						return;
					}

					relativeWaypointPosition = myTransform.InverseTransformPoint(currentWaypointTransform.position);

					horz = (relativeWaypointPosition.x / relativeWaypointPosition.magnitude);

					if (Mathf.Abs(horz) < 0.5f)
					{
						vert = relativeWaypointPosition.z / relativeWaypointPosition.magnitude - Mathf.Abs(horz);
					}
					else
					{
						NoMove();
					}

					break;
				case AiState.steer_to_target:
					if (!didInit)
						return;

					if (followTarget == null)
					{
						return;
					}

					relativeWaypointPosition = transform.InverseTransformPoint(followTarget.position);

					horz = (relativeWaypointPosition.x / relativeWaypointPosition.magnitude);

					if (Vector3.Distance(followTarget.position, myTransform.position) > minChaseDistance)
					{
						MoveForward();
					}
					else
					{
						NoMove();
					}

					if (followTarget != null)
						LookAroundFor(followTarget);

					if (obstacleFinderResult == 1)
					{
						TurnLeft();
					}
					if (obstacleFinderResult == 2)
					{
						TurnRight();
					}
					if (obstacleFinderResult == 3)
					{
						MoveBack();
					}

					break;
				case AiState.paused_no_target:
					break;
			}
		}

		protected virtual void TurnLeft()
		{
			horz = -1;
		}

		protected virtual void TurnRight()
		{
			horz = 1;
		}

		protected virtual void MoveForward()
		{
			vert = 1;
		}

		protected virtual void MoveBack()
		{
			vert = -1;
		}

		protected virtual void NoMove()
		{
			vert = 0;
		}

		protected virtual void LookAroundFor(Transform aTransform)
		{
			if (Vector3.Distance(myTransform.position, aTransform.position) < maxChaseDistance)
			{
				if (CanSee(followTarget))
				{
					SetAIState(AiState.chasing_target);
				}
			}
		}

		private int obstacleFinding;

		protected virtual int IsObstacleAhead()
		{
			obstacleHitType = 0;

			if (myTransform == null)
			{
				return 0;
			}

#if UNITY_EDITOR
			Debug.DrawRay(myTransform.position, ((myTransform.forward + (myTransform.right * 0.5f)) * wallAvoidDistance));
			Debug.DrawRay(myTransform.position, ((myTransform.forward + (myTransform.right * -0.5f)) * wallAvoidDistance));
#endif

			if (Physics.Raycast(myTransform.position, myTransform.forward + (myTransform.right * 0.5f), out hit,
				    wallAvoidDistance))
			{
				obstacleHitType = 1;
			}

			if (Physics.Raycast(myTransform.position, myTransform.forward + (myTransform.right * -0.5f), out hit,
				    wallAvoidDistance))
			{
				if (obstacleHitType == 0)
				{
					obstacleHitType = 2;
				}
				else
				{
					obstacleHitType = 3;
				}
			}

			return obstacleHitType;
		}

		private void TurnTowardTarget(Transform aTarget)
		{
			if (aTarget == null)
				return;

			relativeTarget = rotateTransform.InverseTransformPoint(aTarget.position);

			targetAngle = Mathf.Atan2(relativeTarget.x, relativeTarget.z);

			targetAngle *= Mathf.Rad2Deg;

			targetAngle = Mathf.Clamp(targetAngle, -followTargetMaxTurnAngle - targetAngle, followTargetMaxTurnAngle);

			rotateTransform.Rotate(0, targetAngle * modelRotateSpeed * Time.deltaTime, 0);
		}

		private bool CanSee(Transform aTarget)
		{
			tempDirVec = Vector3.Normalize(aTarget.position - myTransform.position);

#if UNITY_EDITOR
			Debug.DrawLine(myTransform.position, aTarget.position);
#endif

			if (Physics.Raycast(myTransform.position + (visionHeightOffset * myTransform.up), tempDirVec, out hit,
				    maxChaseDistance))
			{
				if (hit.transform.gameObject == aTarget.gameObject)
				{
					return true;
				}
			}

			return false;
		}

		public void SetWayController(WaypointsController aControl)
		{
			if (aControl == null)
				return;

			myWayControl = aControl;

			totalWaypoints = myWayControl.GetTotal();

			if (shouldReversePathFollowing)
			{
				currentWaypointNum = totalWaypoints - 1;
			}
			else
			{
				currentWaypointNum = 0;
			}

			if (myWayControl.Closed)
			{
				loopPath = true;
			}
			else
			{
				loopPath = false;
			}

			Init();

			currentWaypointTransform = myWayControl.GetWaypoint(currentWaypointNum);

			if (startAtFirstWaypoint)
			{
				myTransform.position = currentWaypointTransform.position;
			}
		}

		public void ClearWayController()
		{
			myWayControl = null;
		}

		public void SetReversePath(bool shouldRev)
		{
			shouldReversePathFollowing = shouldRev;
		}

		public void SetSpeed(float aSpeed)
		{
			moveSpeed = aSpeed;
		}

		public void SetPathSmoothingRate(float aRate)
		{
			pathSmoothing = aRate;
		}

		public void SetRotateSpeed(float aRate)
		{
			modelRotateSpeed = aRate;
		}

		public virtual void CurrentWaypointChanged()
		{

		}

		private void UpdateWaypoints()
		{
			if (myWayControl == null)
				return;

			if (reachedLastWaypoint && destroyAtEndOfWaypoints)
			{
				Destroy(gameObject);
				return;
			}
			else if (reachedLastWaypoint)
			{
				if (!loopPath)
				{
					SetAIState(AiState.paused_no_target);

					currentWaypointNum = 0;
					totalWaypoints = 0;
					reachedLastWaypoint = false;

					currentWaypointTransform = null;

					return;
				}
			}

			if (totalWaypoints == 0)
			{
				totalWaypoints = myWayControl.GetTotal();
				return;
			}

			if (currentWaypointTransform == null)
			{
				currentWaypointTransform = myWayControl.GetWaypoint(currentWaypointNum);
			}

			myPosition = myTransform.position;

			nodePosition = currentWaypointTransform.position;

			currentWayDist = Vector3.Distance(nodePosition, myPosition);

			if (currentWayDist < waypointDistance)
			{
				if (shouldReversePathFollowing)
				{
					currentWaypointNum--;

					if (currentWaypointNum < 0)
					{
						currentWaypointNum = 0;
						reachedLastWaypoint = true;

						if (loopPath)
						{
							currentWaypointNum = totalWaypoints;

							reachedLastWaypoint = false;

							currentWaypointTransform = myWayControl.GetWaypoint(currentWaypointNum);
						}

						return;
					}
				}
				else
				{
					currentWaypointNum++;

					if (currentWaypointNum >= totalWaypoints)
					{
						reachedLastWaypoint = true;

						if (loopPath)
						{
							currentWaypointNum = 0;

							reachedLastWaypoint = false;

							currentWaypointTransform = myWayControl.GetWaypoint(currentWaypointNum);
						}

						return;
					}
				}

				currentWaypointTransform = myWayControl.GetWaypoint(currentWaypointNum);

				CurrentWaypointChanged();
			}
		}

		public float GetHorizontal()
		{
			return horz;
		}

		public float GetVertical()
		{
			return vert;
		}
	}
}
