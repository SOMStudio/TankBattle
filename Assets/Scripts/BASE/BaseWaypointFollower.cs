using UnityEngine;
using UnityEngine.Serialization;

[AddComponentMenu("Base/BaseWaypointFollower")]
public class BaseWaypointFollower : ExtendedCustomMonoBehaviour
{
	[Header("AI Controller")]
	[SerializeField] private BaseAIController aiController;

	public override void Init()
	{
		base.Init();

		aiController = myGameObject.GetComponent<BaseAIController>();

		if (aiController == null)
			aiController = myGameObject.AddComponent<BaseAIController>();

		aiController.Init();

		aiController.SetAIControl(true);

		aiController.SetAIState(AIStates.AiState.translate_along_waypoint_path);

		didInit = true;
	}

	public virtual void SetAIWayController(WaypointsController aWaypointControl)
	{
		if (aiController == null)
			Init();

		aiController.SetWayController(aWaypointControl);
	}

	public virtual void ClearAIWayController()
	{
		if (aiController == null)
			Init();

		aiController.ClearWayController();
	}
}
