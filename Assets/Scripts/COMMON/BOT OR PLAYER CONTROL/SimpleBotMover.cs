using UnityEngine;
using System.Collections;

// use this script in conjunction with the BaseAIController to make a bot move around

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

	// main event
	void Update ()
	{
		// turn the transform, if required
		myTransform.Rotate (new Vector3 (0, Time.deltaTime * AIController.GetHorizontal() * turnSpeed, 0));
		
		// if we have a rigidbody, move it if required
		if (myBody != null) {
			myBody.AddForce ((myTransform.forward * moveSpeed * Time.deltaTime) * AIController.GetVertical(), ForceMode.VelocityChange);
		}
	}

	// main logic
	public override void Init ()
	{
		base.Init ();

		// if it hasn't been set in the editor, let's try and find it on this transform
		if (!AIController) {
			AIController = myTransform.GetComponent<BaseAIController> ();
		}

		// set center of gravity
		if (!myBody) {
			myBody.centerOfMass = centerOfGravity;
		}
	}
}
