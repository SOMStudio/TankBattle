using SOMStudio.TankBattle.Scripts.Base;
using UnityEngine;

namespace SOMStudio.TankBattle.Scripts.Tools.Weapon
{
	[AddComponentMenu("Common/Weapons/Slot Weapon Controller")]
	public class SlotWeaponController : BaseWeaponController
	{
		[Header("Settings")]
		[SerializeField] private bool allowWeaponSwitchKeys = true;

		private void Update()
		{
			if (!allowWeaponSwitchKeys)
				return;

			if (UnityEngine.Input.GetKey("1"))
			{
				SetWeaponSlot(0);
			}

			if (UnityEngine.Input.GetKey("2"))
			{
				SetWeaponSlot(1);
			}

			if (UnityEngine.Input.GetKey("3"))
			{
				SetWeaponSlot(2);
			}

			if (UnityEngine.Input.GetKey("4"))
			{
				SetWeaponSlot(3);
			}

			if (UnityEngine.Input.GetKey("5"))
			{
				SetWeaponSlot(4);
			}

			if (UnityEngine.Input.GetKey("6"))
			{
				SetWeaponSlot(5);
			}

			if (UnityEngine.Input.GetKey("7"))
			{
				SetWeaponSlot(6);
			}

			if (UnityEngine.Input.GetKey("8"))
			{
				SetWeaponSlot(7);
			}

			if (UnityEngine.Input.GetKey("9"))
			{
				SetWeaponSlot(8);
			}
		}
	}
}
