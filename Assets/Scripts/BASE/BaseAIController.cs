using UnityEngine;
using System.Collections;
using AIStates;

[AddComponentMenu("Base/AI Controller")]

public class BaseAIController : ExtendedCustomMonoBehaviour {

	[Header("Move")]
	[SerializeField]
	private float horz;
	[SerializeField]
	private float vert;

	private Vector3 relativeTarget;
	private float targetAngle;
	private RaycastHit hit;
	private Vector3 tempDirVec;
	private int obstacleHitType;

	[Header("Settings")]
	[SerializeField]
	private bool isStationary;
	[SerializeField]
	private AIState currentAIState;

	[SerializeField]
	private float modelRotateSpeed = 15f;
	[SerializeField]
	private int followTargetMaxTurnAngle = 120;

	[Header("Patrol")]
	[SerializeField]
	private float patrolSpeed = 5f;
	[SerializeField]
	private float patrolTurnSpeed = 10f;
	[SerializeField]
	private float wallAvoidDistance = 40f;

	[Header("Chase Target")]
	[SerializeField]
	private Transform followTarget;

	[SerializeField]
	private float minChaseDistance = 2f;
	[SerializeField]
	private float maxChaseDistance = 10f;
	[SerializeField]
	private float visionHeightOffset = 1f;
	
	[Header("Waypoint")]
	[SerializeField]
	private Waypoints_Controller myWayControl;
	
	[SerializeField]
	private int currentWaypointNum;

	[SerializeField]
	private float waypointDistance= 5f;
	[SerializeField]
	private float moveSpeed= 30f;
	[SerializeField]
	private float pathSmoothing= 2f;
	[SerializeField]
	private bool shouldReversePathFollowing;
	[SerializeField]
	private bool loopPath;
	[SerializeField]
	private bool destroyAtEndOfWaypoints;

	[SerializeField]
	private bool faceWaypoints;
	[SerializeField]
	private bool startAtFirstWaypoint;
	
	[System.NonSerialized]
	public Transform currentWaypointTransform;
	
	private int totalWaypoints;
	
	private Vector3 nodePosition;
	private Vector3 myPosition;
	private float currentWayDist;
	
	[System.NonSerialized]
	[SerializeField]
	public bool reachedLastWaypoint;
	private Vector3 moveVec;
	private Vector3 targetMoveVec;
	private float distanceToChaseTarget;

	[System.NonSerialized]
	public bool isRespawning;
	
	private int obstacleFinderResult;	
	[SerializeField]
	private Transform rotateTransform;
    	
	[System.NonSerialized]
	public Vector3 RelativeWaypointPosition;
	
	public bool AIControlled;
	public bool useFixedUpdateForAIState = false;

	// main event
	void LateUpdate() {
		LateUpdateAIState ();
	}

	// main logic

	public override void Init () 
	{		
		if (!didInit) {
			// cache ref to gameObject
			myGO = gameObject;
		
			// cache ref to transform
			myTransform = transform;
		
			// rotateTransform may be set if the object we rotate is different to the main transform
			if (rotateTransform == null)
				rotateTransform = myTransform;
		
			// cache a ref to our rigidbody
			myBody = myTransform.GetComponent<Rigidbody> ();
		
			// init done!
			didInit = true;
		}
	}

	public void SetCanControl (bool val) {
		canControl = val;
	}

	public void SetAIControl (bool val)
	{
		AIControlled = val;
	}
	
	// set up AI parameters --------------------
	
	public void SetPatrolSpeed (float aNum)
	{
		patrolSpeed = aNum;
	}
	
	public void SetPatrolTurnSpeed (float aNum)
	{
		patrolTurnSpeed = aNum;
	}
	
	public void SetWallAvoidDistance (float aNum)
	{
		wallAvoidDistance = aNum;
	}
	
	public void SetWaypointDistance (float aNum)
	{
		waypointDistance = aNum;
	}

	public void SetMoveSpeed (float aNum)
	{
		moveSpeed = aNum;
	}
	
	public void SetMinChaseDistance (float aNum)
	{
		minChaseDistance = aNum;
	}
	
	public void SetMaxChaseDistance (float aNum)
	{
		maxChaseDistance = aNum;
	}
	
	public void SetPathSmoothing (float aNum)
	{
		pathSmoothing = aNum;
	}
	
	// -----------------------------------------
	
	public virtual void SetAIState( AIState newState )
	{
		// update AI state
		currentAIState= newState;
	}
	
	public virtual void SetChaseTarget( Transform theTransform )
	{
		// set a target for this AI to chase, if required
		followTarget= theTransform;
	}

	protected virtual void LateUpdateAIState () 
	{
		// make sure we have initialized before doing anything
		if (!didInit)
			Init ();

		// check to see if we're supposed to be controlling the player
		if (!AIControlled)
			return;

		// do AI updates
		UpdateAI ();
	}
	
	protected virtual void UpdateAI()
	{
		// reset our inputs
		horz = 0;
		vert = 0;

		if (followTarget != null) {
			obstacleFinderResult = IsObstacleAhead ();
		}
		
		switch (currentAIState) {
		// -----------------------------
		case AIState.moving_looking_for_target:
			// look for chase target
			if (followTarget != null)
				LookAroundFor (followTarget);
			
			// the AvoidWalls function looks to see if there's anything in-front. If there is,
			// it will automatically change the value of moveDirection before we do the actual move
			if (obstacleFinderResult == 1) { // GO LEFT
				SetAIState (AIState.stopped_turning_left);
			}
			if (obstacleFinderResult == 2) { // GO RIGHT
				SetAIState (AIState.stopped_turning_right);
			}
			
			if (obstacleFinderResult == 3) { // BACK UP
				SetAIState (AIState.backing_up_looking_for_target);
			}
			
			// all clear! head forward
			MoveForward ();
			break; 
		case AIState.chasing_target:
			// chasing
			// in case mode, we point toward the target and go right at it!
			
			// quick check to make sure that we have a target (if not, we drop back to patrol mode)
			if (followTarget == null)
				SetAIState (AIState.moving_looking_for_target);
			
			// the TurnTowardTarget function does just that, so to chase we just throw it the current target
			TurnTowardTarget (followTarget);
			
			// find the distance between us and the chase target to see if it is within range
			distanceToChaseTarget = Vector3.Distance (myTransform.position, followTarget.position);
			
			// check the range
			if (distanceToChaseTarget > minChaseDistance) {
				// keep charging forward
				MoveForward ();
			}
		
			// here we do a quick check to test the distance between AI and target. If it's higher than
			// our maxChaseDistance variable, we drop out of chase mode and go back to patrolling.
			if (distanceToChaseTarget > maxChaseDistance || CanSee (followTarget) == false) {
				// set our state to 1 - moving_looking_for_target
				SetAIState (AIState.moving_looking_for_target);
			}
			
			break;
		// -----------------------------
		
		case AIState.backing_up_looking_for_target:
			
			// look for chase target
			if (followTarget != null)
				LookAroundFor (followTarget);
			
			// backing up
			MoveBack ();
			
			if (obstacleFinderResult == 0) {
				// now we've backed up, lets randomize whether to go left or right
				if (Random.Range (0, 100) > 50) {
					SetAIState (AIState.stopped_turning_left);	
				} else {
					SetAIState (AIState.stopped_turning_right);							
				}
			}
			break;
		case AIState.stopped_turning_left:
			// look for chase target
			if (followTarget != null)
				LookAroundFor (followTarget);
			
			// stopped, turning left
			TurnLeft ();
			
			if (obstacleFinderResult == 0) {
				SetAIState (AIState.moving_looking_for_target);	
			}
			break;
			
		case AIState.stopped_turning_right:
			// look for chase target
			if (followTarget != null)
				LookAroundFor (followTarget);
			
			// stopped, turning right
			TurnRight ();
			
			// check results from looking, to see if path ahead is clear
			if (obstacleFinderResult == 0) {
				SetAIState (AIState.moving_looking_for_target);	
			}
			break;
		case AIState.paused_looking_for_target:
			// standing still, with looking for chase target
			// look for chase target
			if (followTarget != null)
				LookAroundFor (followTarget);
			break;
		
		case AIState.translate_along_waypoint_path:
			// following waypoints (moving toward them, not pointing at them) at the speed of
			// moveSpeed
			
			// make sure we have been initialized before trying to access waypoints
			if (!didInit && !reachedLastWaypoint)
				return;
		
			UpdateWaypoints ();
			
			// move the ship
			if (!isStationary
			   && currentWaypointTransform != null) {
				targetMoveVec = Vector3.Normalize (currentWaypointTransform.position - myTransform.position);
				moveVec = Vector3.Lerp (moveVec, targetMoveVec, Time.deltaTime * pathSmoothing);
				myTransform.Translate (moveVec * moveSpeed * Time.deltaTime);
				MoveForward ();
				
				if (faceWaypoints) {
					TurnTowardTarget (currentWaypointTransform);
				}
			}
			break;
		
		case AIState.steer_to_waypoint:
			
			// make sure we have been initialized before trying to access waypoints
			if (!didInit && !reachedLastWaypoint)
				return;
			
			UpdateWaypoints ();
			
			if (currentWaypointTransform == null) {
				// it may be possible that this function gets called before waypoints have been set up, so we catch any nulls here
				return;
			}
			
			// now we just find the relative position of the waypoint from the car transform,
			// that way we can determine how far to the left and right the waypoint is.
			RelativeWaypointPosition = myTransform.InverseTransformPoint (currentWaypointTransform.position);
																				
			// by dividing the horz position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
			horz = (RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude);
			
			// now we do the same for torque, but make sure that it doesn't apply any engine torque when going around a sharp turn...
			if (Mathf.Abs (horz) < 0.5f) {
				vert = RelativeWaypointPosition.z / RelativeWaypointPosition.magnitude - Mathf.Abs (horz);
			} else {
				NoMove ();
			}
			break;
		
		case AIState.steer_to_target:
			
			// make sure we have been initialized before trying to access waypoints
			if (!didInit)
				return;
			
			if (followTarget == null) {
				// it may be possible that this function gets called before a targer has been set up, so we catch any nulls here
				return;
			}
			
			// now we just find the relative position of the waypoint from the car transform,
			// that way we can determine how far to the left and right the waypoint is.
			RelativeWaypointPosition = transform.InverseTransformPoint (followTarget.position);
																				
			// by dividing the horz position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
			horz = (RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude);
						
			// if we're outside of the minimum chase distance, drive!
			if (Vector3.Distance (followTarget.position, myTransform.position) > minChaseDistance) {
				MoveForward ();
			} else {
				NoMove ();	
			}
			
			if (followTarget != null)
				LookAroundFor (followTarget);
			
			// the AvoidWalls function looks to see if there's anything in-front. If there is,
			// it will automatically change the value of moveDirection before we do the actual move
			if (obstacleFinderResult == 1) { // GO LEFT
				TurnLeft ();
			}
			
			if (obstacleFinderResult == 2) { // GO RIGHT
				TurnRight ();
			}
			
			if (obstacleFinderResult == 3) { // BACK UP
				MoveBack ();
			}
			
			break;
			
		case AIState.paused_no_target:
			// paused_no_target
			break;
			
		default:
			// idle (do nothing)
			break;
		}
	}
	
	protected virtual void TurnLeft ()
	{
		horz= -1;
	}
	
	protected virtual void TurnRight ()
	{
		horz= 1;
	}
	
	protected virtual void MoveForward ()
	{
		vert= 1;
	}
	
	protected virtual void MoveBack ()
	{
		vert= -1;
	}

	protected virtual void NoMove ()
	{
		vert= 0;
	}
	
	protected virtual void LookAroundFor(Transform aTransform)
	{
		// here we do a quick check to test the distance between AI and target. If it's higher than
		// our maxChaseDistance variable, we drop out of chase mode and go back to patrolling.
		if( Vector3.Distance( myTransform.position, aTransform.position ) < maxChaseDistance )
		{
			// check to see if the target is visible before going into chase mode
			if( CanSee( followTarget )==true )
			{
				// set our state to chase the target
				SetAIState( AIState.chasing_target );
			}
		}
	}
	
	private int obstacleFinding;
	
	protected virtual int IsObstacleAhead()
	{
		obstacleHitType = 0;
		
		// quick check to make sure that myTransform has been set
		if (myTransform == null) {
			return 0;
		}
		
		// draw this raycast so we can see what it is doing
		#if UNITY_EDITOR
		Debug.DrawRay (myTransform.position, ((myTransform.forward + (myTransform.right * 0.5f)) * wallAvoidDistance));
		Debug.DrawRay (myTransform.position, ((myTransform.forward + (myTransform.right * -0.5f)) * wallAvoidDistance));
		#endif
		
		// cast a ray out forward from our AI and put the 'result' into the variable named hit
		if (Physics.Raycast (myTransform.position, myTransform.forward + (myTransform.right * 0.5f), out hit, wallAvoidDistance)) {
			// obstacle
			// it's a left hit, so it's a type 1 right now (though it could change when we check on the other side)
			obstacleHitType = 1;
		}
		
		if (Physics.Raycast (myTransform.position, myTransform.forward + (myTransform.right * -0.5f), out hit, wallAvoidDistance)) {
			// obstacle
			if (obstacleHitType == 0) {
				// if we haven't hit anything yet, this is a type 2
				obstacleHitType = 2;
			} else {
				// if we have hits on both left and right raycasts, it's a type 3
				obstacleHitType = 3;
			}
		}
		
		return obstacleHitType;
	}
	
	private void TurnTowardTarget( Transform aTarget )
	{
		if (aTarget == null)
			return;
		
		// Calculate the target position relative to the 
		// target this transforms coordinate system. 
		relativeTarget = rotateTransform.InverseTransformPoint (aTarget.position); // note we use rotateTransform as a rotation object rather than myTransform!
		
		// Calculate the target angle  
		targetAngle = Mathf.Atan2 (relativeTarget.x, relativeTarget.z);
		
		// Atan returns the angle in radians, convert to degrees 
		targetAngle *= Mathf.Rad2Deg;
		
		// The wheels should have a maximum rotation angle 
		targetAngle = Mathf.Clamp (targetAngle, -followTargetMaxTurnAngle - targetAngle, followTargetMaxTurnAngle);
		
		// turn towards the target at the rate of modelRotateSpeed
		rotateTransform.Rotate (0, targetAngle * modelRotateSpeed * Time.deltaTime, 0);
	}
	
	private bool CanSee( Transform aTarget )
	{
		// first, let's get a vector to use for raycasting by subtracting the target position from our AI position
		tempDirVec=Vector3.Normalize( aTarget.position - myTransform.position );
		
		// lets have a debug line to check the distance between the two manually, in case you run into trouble!
		#if UNITY_EDITOR
		Debug.DrawLine( myTransform.position, aTarget.position );
		#endif
		
		// cast a ray from our AI, out toward the target passed in (use the tempDirVec magnitude as the distance to cast)
		if( Physics.Raycast( myTransform.position + ( visionHeightOffset * myTransform.up ), tempDirVec, out hit, maxChaseDistance ))
		{
			// check to see if we hit the target
			if( hit.transform.gameObject == aTarget.gameObject )
			{
				return true;
			}
		}
		
		// nothing found, so return false
		return false;
	}
	
	public void SetWayController( Waypoints_Controller aControl )
	{
		if (aControl == null)
			return;

		myWayControl=aControl;
		
		// grab total waypoints
		totalWaypoints = myWayControl.GetTotal();

		if( shouldReversePathFollowing )
		{
			currentWaypointNum= totalWaypoints-1;
		} else {
			currentWaypointNum= 0;
		}

		if (myWayControl.Closed) {
			loopPath = true;
		} else {
			loopPath = false;
		}
		
		Init();
		
		// get the first waypoint from the waypoint controller
		currentWaypointTransform= myWayControl.GetWaypoint( currentWaypointNum );
		
		if( startAtFirstWaypoint )
		{
			// position at the currentWaypointTransform position
			myTransform.position= currentWaypointTransform.position;
		}
	}

	public void ClearWayController()
	{
		myWayControl = null;
	}
	
	public void SetReversePath( bool shouldRev )
	{
		shouldReversePathFollowing= shouldRev;
	}
	
	public void SetSpeed( float aSpeed )
	{
		moveSpeed= aSpeed;
	}
	
	public void SetPathSmoothingRate( float aRate )
	{
		pathSmoothing= aRate;
	}
	
	public void SetRotateSpeed( float aRate )
	{
		modelRotateSpeed= aRate;
	}

	public virtual void CurrentWaypointChanged() {

	}

	void UpdateWaypoints()
	{
		// If we don't have a waypoint controller, we safely drop out
		if( myWayControl==null )
			return;
		
		if( reachedLastWaypoint && destroyAtEndOfWaypoints )
		{
			// destroy myself(!)
			Destroy( gameObject );
			return;
		} else if( reachedLastWaypoint )
		{
			if (!loopPath) {
				SetAIState (AIState.paused_no_target);

				currentWaypointNum = 0;
				totalWaypoints = 0;
				reachedLastWaypoint = false;

				// grab our transform reference from the waypoint controller
				currentWaypointTransform = null;

				return;
			}
		}
		
		// because of the order that scripts run and are initialised, it is possible for this function
		// to be called before we have actually finished running the waypoints initialization, which
		// means we need to drop out to avoid doing anything silly or before it breaks the game.
		if( totalWaypoints==0 )
		{
			// grab total waypoints
			totalWaypoints= myWayControl.GetTotal();
			return;
		}
	
		if( currentWaypointTransform==null )
		{
			// grab our transform reference from the waypoint controller
			currentWaypointTransform= myWayControl.GetWaypoint( currentWaypointNum );
		}
		
		// now we check to see if we are close enough to the current waypoint
		// to advance on to the next one
	
		myPosition= myTransform.position;
		//myPosition.y= 0;
	
		// get waypoint position and 'flatten' it
		nodePosition= currentWaypointTransform.position;
		//nodePosition.y= 0;
	
		// check distance from this to the waypoint
	
		currentWayDist= Vector3.Distance( nodePosition,myPosition );
	
		if ( currentWayDist < waypointDistance ) {
			// we are close to the current node, so let's move on to the next one!
			
			if( shouldReversePathFollowing )
			{
				currentWaypointNum--;
				// now check to see if we have been all the way around
				if( currentWaypointNum<0 ){
					// just incase it gets referenced before we are destroyed, let's keep it to a safe index number
					currentWaypointNum= 0;
					// completed the route!
					reachedLastWaypoint= true;
					// if we are set to loop, reset the currentWaypointNum to 0
					if(loopPath)
					{
						currentWaypointNum= totalWaypoints;
						
						// the route keeps going in a loop, so we don't want reachedLastWaypoint to ever become true
						reachedLastWaypoint= false;

						// grab our transform reference from the waypoint controller
						currentWaypointTransform= myWayControl.GetWaypoint( currentWaypointNum );
					}
					// drop out of this function before we grab another waypoint into currentWaypointTransform, as
					// we don't need one and the index may be invalid
					return;
				}
			} else {
				currentWaypointNum++;
				// now check to see if we have been all the way around
				if( currentWaypointNum>=totalWaypoints ){
					// completed the route!
					reachedLastWaypoint= true;
					// if we are set to loop, reset the currentWaypointNum to 0
					if(loopPath)
					{
						currentWaypointNum= 0;
						
						// the route keeps going in a loop, so we don't want reachedLastWaypoint to ever become true
						reachedLastWaypoint= false;

						// grab our transform reference from the waypoint controller
						currentWaypointTransform= myWayControl.GetWaypoint( currentWaypointNum );
					}
					// drop out of this function before we grab another waypoint into currentWaypointTransform, as
					// we don't need one and the index may be invalid
					return;
				}
			}
			
			// grab our transform reference from the waypoint controller
			currentWaypointTransform= myWayControl.GetWaypoint( currentWaypointNum );

			//Event for override
			CurrentWaypointChanged ();
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
