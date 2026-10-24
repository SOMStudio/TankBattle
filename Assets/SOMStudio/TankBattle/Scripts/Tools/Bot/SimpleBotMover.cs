using SOMStudio.TankBattle.Scripts.Base;
using UnityEngine;

namespace SOMStudio.TankBattle.Scripts.Tools.Bot
{
	public class SimpleBotMover : ExtendedCustomMonoBehaviour
	{
		[SerializeField] private float turnSpeed = 0.5f;
		[SerializeField] private float moveSpeed = 0.5f;

		[SerializeField] private BaseAIController aiController;
		[SerializeField] private Vector3 centerOfGravity;

		private void Update()
		{
			myTransform.Rotate(new Vector3(0, Time.deltaTime * aiController.GetHorizontal() * turnSpeed, 0));

			if (myBody != null)
			{
				myBody.AddForce(myTransform.forward * (moveSpeed * Time.deltaTime * aiController.GetVertical()),
					ForceMode.VelocityChange);
			}
		}

		public override void Init()
		{
			base.Init();

			if (!aiController)
			{
				aiController = myTransform.GetComponent<BaseAIController>();
			}

			if (!myBody)
			{
				myBody.centerOfMass = centerOfGravity;
			}
		}
	}
}
