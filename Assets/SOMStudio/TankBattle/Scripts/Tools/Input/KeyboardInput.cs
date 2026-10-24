using SOMStudio.TankBattle.Scripts.Base;
using UnityEngine;

namespace SOMStudio.TankBattle.Scripts.Tools.Input
{
	[AddComponentMenu("Common/Keyboard Input Controller")]
	public class KeyboardInput : BaseInputController
	{
		private void LateUpdate()
		{
			CheckInput();
		}

		protected override void CheckInput()
		{
			base.CheckInput();

			fire1 = UnityEngine.Input.GetButton("Fire1");
			shouldRespawn = UnityEngine.Input.GetButton("Fire3");
		}
	}
}
