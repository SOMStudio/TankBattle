using SOMStudio.TankBattle.Scripts.Base;
using UnityEngine;

namespace SOMStudio.TankBattle.Scripts
{
	[AddComponentMenu("SOMStudio/TankBattle/User Manager")]
	public class UserManager : BaseUserManager
	{
		[SerializeField] private float detailedHealth;
		[SerializeField] private float protection;

		public override void GetDefaultData()
		{
			base.GetDefaultData();

			detailedHealth = 100;
			protection = 1;
		}

		public float GetProtection()
		{
			return protection;
		}

		public void AddProtection(float num)
		{
			protection += num;
		}

		public void ReduceProtection(float num)
		{
			protection -= num;
		}

		public void SetProtection(float num)
		{
			protection = num;
		}

		public float GetDetailHealth()
		{
			return detailedHealth;
		}

		public void AddDetailHealth(float num)
		{
			detailedHealth += num;
		}

		public void ReduceDetailHealth(float num)
		{
			detailedHealth -= num;
		}

		public void SetDetailHealth(float num)
		{
			detailedHealth = num;
		}
	}
}
