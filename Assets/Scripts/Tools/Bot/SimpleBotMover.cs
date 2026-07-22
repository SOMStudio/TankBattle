using UnityEngine;

public class SimpleBotMover : ExtendedCustomMonoBehaviour
{
	[SerializeField]
	private float turnSpeed= 0.5f;
	[SerializeField]
	private float moveSpeed= 0.5f;

	[SerializeField]
	private BaseAIController AIController;
	
	[SerializeField]
	private Vector3 centerOfGravity;

	private void Update ()
	{
		myTransform.Rotate (new Vector3 (0, Time.deltaTime * AIController.GetHorizontal() * turnSpeed, 0));
		
		if (myBody != null) {
			myBody.AddForce ((myTransform.forward * moveSpeed * Time.deltaTime) * AIController.GetVertical(), ForceMode.VelocityChange);
		}
	}
	
	public override void Init ()
	{
		base.Init ();
		
		if (!AIController) {
			AIController = myTransform.GetComponent<BaseAIController> ();
		}
		
		if (!myBody) {
			myBody.centerOfMass = centerOfGravity;
		}
	}
}
