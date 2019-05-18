using UnityEngine;
using System.Collections;

[AddComponentMenu("Base/BaseWaypointFollower")]

// uses the translate along path function of the AIController to follow along a path

public class BaseWaypointFollower : ExtendedCustomMonoBehaviour 
{
	[Header("AI Controller")]
	public BaseAIController AIController;

	// main logic
	
	public virtual void Init ()
	{
		// cache our transform
		if (!myTransform) {
			myTransform = transform;
		}
		
		// cache our gameObject
		if (!myGO) {
			myGO = gameObject;
		}
		
		// cache a reference to the AI controller
		AIController= myGO.GetComponent<BaseAIController>();
			
		if(AIController==null)
			AIController= myGO.AddComponent<BaseAIController>();
		
		// run the Init function from our base class (BaseAIController.cs)
		AIController.Init();
		
		// tell AI controller that we want it to control this object
		AIController.SetAIControl(true);
		
		// tell our AI to follow waypoints
		AIController.SetAIState( AIStates.AIState.translate_along_waypoint_path );
		
		// set a flag to tell us that init has happened
		didInit= true;
	}
		
	public virtual void SetAIWayController( Waypoints_Controller aWaypointControl )
	{
		if(AIController==null)
			Init ();
		
		// pass this on to our waypoint controller, so that it can follow the waypoints
		AIController.SetWayController(aWaypointControl);
	}

	public virtual void ClearAIWayController() {
		if(AIController==null)
			Init ();

		// pass this on to our waypoint controller, so that it can follow the waypoints
		AIController.ClearWayController();
	}
}
