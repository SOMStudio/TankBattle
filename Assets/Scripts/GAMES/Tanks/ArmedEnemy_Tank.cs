using UnityEngine;

public class ArmedEnemy_Tank : BaseArmedEnemy
{
	[Header("Settings")] [SerializeField] private float thisEnemyDetaleStrange = 100;
	[SerializeField] private float thisEnemyProtection = 1;

	[SerializeField] private int contactDamage = 1;

	private PlayerManager_Tank myPlayerManager_New;
	private UserManager_Tank myDataManager_New;

	private bool isRespawning;

	public void OnCollisionEnter(Collision collider)
	{
		if (collider.gameObject.layer == 9 && !isRespawning)
		{
			ProjectileController projManager = collider.gameObject.GetComponent<ProjectileController>();
			if (projManager)
			{
				float reduceHels = myDataManager_New.GetProtection() * projManager.OverrideDamageValue;
				tempINT = int.Parse(collider.gameObject.name);

				ReduceLife(reduceHels);
			}
		}

		if (collider.gameObject.layer == 8 && !isRespawning)
		{
			TellGCEnemyDestroyed();

			if (isBoss)
				TellGCBossDestroyed();

			Destroy(gameObject);
		}

		if (collider.gameObject.layer == 20)
		{
			TellGCEnemyDestroyed();

			Destroy(gameObject);
		}
	}

	public override void Init()
	{
		base.Init();

		if (myPlayerManager_New == null)
		{
			myPlayerManager_New = (PlayerManager_Tank)myPlayerManager;

			if (myPlayerManager_New)
				myDataManager_New = (UserManager_Tank)myDataManager;
		}

		myDataManager_New.SetDetailHealth(thisEnemyDetaleStrange);
		myDataManager_New.SetProtection(thisEnemyProtection);

		BaseAIController aControl = (BaseAIController)gameObject.GetComponent<BaseAIController>();

		aControl.SetChaseTarget(GameController_Tank.Instance.GetMainPlayerTransform());

		aControl.SetAIState(AIStates.AIState.chasing_target);

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
		myDataManager_New.ReduceDetailHealth(val);

		if (myDataManager_New.GetDetailHealth() < 0)
		{
			LostLife();
		}
	}

	private void LostLife()
	{
		myDataManager_New.ReduceHealth(1);

		if (myDataManager_New.GetHealth() == 0)
		{
			TellGCEnemyDestroyed();

			if (isBoss)
				TellGCBossDestroyed();

			Destroy(gameObject);
		}
	}
}
