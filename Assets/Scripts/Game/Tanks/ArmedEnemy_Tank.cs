using UnityEngine;

public class ArmedEnemy_Tank : BaseArmedEnemy
{
	[Header("Settings")]
	[SerializeField] private float detailedStrange = 100;
	[SerializeField] private float protection = 1;
	[SerializeField] private int contactDamage = 1;

	private PlayerManager_Tank playerManager;
	private UserManager_Tank userManager;

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
			playerManager = (PlayerManager_Tank)myPlayerManager;

			if (playerManager)
				userManager = (UserManager_Tank)myDataManager;
		}

		userManager.SetDetailHealth(detailedStrange);
		userManager.SetProtection(protection);

		BaseAIController aControl = gameObject.GetComponent<BaseAIController>();

		aControl.SetChaseTarget(GameController_Tank.Instance.GetMainPlayerTransform());

		aControl.SetAIState(AIStates.AiState.chasing_target);

		GameController_Tank.Instance.AddEnemyToRadar(myTransform);
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
		GameController_Tank.Instance.EnemyDestroyed(myTransform.position, pointsValue, tempINT);

		GameController_Tank.Instance.RemoveEnemyFromRadar(myTransform);
	}

	public void TellGCBossDestroyed()
	{
		GameController_Tank.Instance.BossDestroyed();

		GameController_Tank.Instance.RemoveEnemyFromRadar(myTransform);
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
