using UnityEngine;

[AddComponentMenu("Base/BaseWaypointFollower")]
public class BaseWaypointFollower : ExtendedCustomMonoBehaviour
{
	[Header("AI Controller")] [SerializeField]
	private BaseAIController AIController;

	public override void Init()
	{
		base.Init();

		AIController = myGO.GetComponent<BaseAIController>();

		if (AIController == null)
			AIController = myGO.AddComponent<BaseAIController>();

		AIController.Init();

		AIController.SetAIControl(true);

		AIController.SetAIState(AIStates.AIState.translate_along_waypoint_path);

		didInit = true;
	}

	public virtual void SetAIWayController(Waypoints_Controller aWaypointControl)
	{
		if (AIController == null)
			Init();

		AIController.SetWayController(aWaypointControl);
	}

	public virtual void ClearAIWayController()
	{
		if (AIController == null)
			Init();

		AIController.ClearWayController();
	}
}
