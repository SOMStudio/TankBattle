using SOMStudio.TankBattle.Scripts.Base;
using SOMStudio.TankBattle.Scripts.Enums;
using SOMStudio.TankBattle.Scripts.Tools.Weapon;
using UnityEngine;

namespace SOMStudio.TankBattle.Scripts
{
	public class ArmedEnemy : BaseArmedEnemy
	{
		[Header("Settings")]
		[SerializeField] private float detailedStrange = 100;
		[SerializeField] private float protection = 1;
		[SerializeField] private int contactDamage = 1;

		private PlayerManager playerManager;
		private UserManager userManager;

		private bool isRespawning;

		public void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.layer == 9 && !isRespawning)
			{
				ProjectileController projManager = collision.gameObject.GetComponent<ProjectileController>();
				if (projManager)
				{
					float reduceHealth = userManager.GetProtection() * projManager.OverrideDamageValue;
					tempINT = int.Parse(collision.gameObject.name);

					ReduceLife(reduceHealth);
				}
			}

			if (collision.gameObject.layer == 8 && !isRespawning)
			{
				TellGCEnemyDestroyed();

				if (isBoss)
					TellGCBossDestroyed();

				Destroy(gameObject);
			}

			if (collision.gameObject.layer == 20)
			{
				TellGCEnemyDestroyed();

				Destroy(gameObject);
			}
		}

		public override void Init()
		{
			base.Init();

			if (playerManager == null)
			{
				playerManager = (PlayerManager)myPlayerManager;

				if (playerManager)
					userManager = (UserManager)myDataManager;
			}

			userManager.SetDetailHealth(detailedStrange);
			userManager.SetProtection(protection);

			BaseAIController aControl = gameObject.GetComponent<BaseAIController>();

			aControl.SetChaseTarget(GameController.Instance.GetMainPlayerTransform());

			aControl.SetAIState(AiState.chasing_target);

			GameController.Instance.AddEnemyToRadar(myTransform);
		}

		public void SetContactDamage(int val)
		{
			contactDamage = val;
		}

		public int GetContactDamage()
		{
			return contactDamage;
		}

		public void TellGCEnemyDestroyed()
		{
			GameController.Instance.EnemyDestroyed(myTransform.position, pointsValue, tempINT);

			GameController.Instance.RemoveEnemyFromRadar(myTransform);
		}

		public void TellGCBossDestroyed()
		{
			GameController.Instance.BossDestroyed();

			GameController.Instance.RemoveEnemyFromRadar(myTransform);
		}

		private void ReduceLife(float val)
		{
			userManager.ReduceDetailHealth(val);

			if (userManager.GetDetailHealth() < 0)
			{
				LostLife();
			}
		}

		private void LostLife()
		{
			userManager.ReduceHealth(1);

			if (userManager.GetHealth() == 0)
			{
				TellGCEnemyDestroyed();

				if (isBoss)
					TellGCBossDestroyed();

				Destroy(gameObject);
			}
		}
	}
}
