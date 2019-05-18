using UnityEngine;
using System.Collections;

public class ArmedEnemy_Tank : BaseArmedEnemy 
{
	[Header("Settings")]
	[SerializeField]
	private float thisEnemyDetaleStrange = 100;
	[SerializeField]
	private float thisEnemyProtection = 1;

	[SerializeField]
	private int contactDamage = 1;

	private PlayerManager_Tank myPlayerManager_New;
	private UserManager_Tank myDataManager_New;

	private bool isRespawning;
	
	// main event
	public void OnCollisionEnter(Collision collider) 
	{
		// when something collides with us, we check its layer to see if it is on 9
		if( collider.gameObject.layer==9 && !isRespawning )
		{
			ProjectileController projManager = collider.gameObject.GetComponent<ProjectileController> ();
			if (projManager) {
				float reduceHels = myDataManager_New.GetProtection () * projManager.OverrideDamageValue;
				tempINT= int.Parse( collider.gameObject.name );

				ReduceLife (reduceHels);
			}
		}

		// we contact with player
		if( collider.gameObject.layer==8 && !isRespawning )
		{
			// tell game controller to make an explosion at our position and to award the player points for hitting us
			TellGCEnemyDestroyed();

			// if this is a boss enemy, tell the game controller when we get destroyed so it can end the level
			if( isBoss )
				TellGCBossDestroyed();

			// destroy this
			Destroy(gameObject);
		}

		// we contact with player
		if (collider.gameObject.layer == 20)
		{
			// tell game controller to make an explosion at our position and to award the player points for hitting us
			TellGCEnemyDestroyed();

			// destroy this
			Destroy(gameObject);
		}
	}

	// main logic
	public override void Init ()
	{
		base.Init ();

		// init player and data managers
		if (myPlayerManager_New == null) {
			myPlayerManager_New = (PlayerManager_Tank)myPlayerManager;

			if (myPlayerManager_New)
				myDataManager_New = (UserManager_Tank)myDataManager;
		}

		myDataManager_New.SetDetaleHealth (thisEnemyDetaleStrange);
		myDataManager_New.SetProtection (thisEnemyProtection);

		// lets find our ai controller
		BaseAIController aControl = (BaseAIController)gameObject.GetComponent<BaseAIController> ();

		// and tell it to chase our player around the screen (we get the player transform from game controller)
		aControl.SetChaseTarget (GameController_Tank.Instance.GetMainPlayerTransform ());
		
		// now get on and chase it!
		aControl.SetAIState (AIStates.AIState.chasing_target);
		
		// we also need to add this enemy to the radar, so we will tell game controller about it and 
		// some code in game controller will do this for us
		GameController_Tank.Instance.AddEnemyToRadar (myTransform);
	}

	public void SetContactDamage(int val) {
		contactDamage = val;
	}

	public int GetContactDamage() {
		return contactDamage;
	}

	public void TellGCEnemyDestroyed()
	{
		// tell the game controller we have been destroyed
		GameController_Tank.Instance.EnemyDestroyed( myTransform.position, pointsValue, tempINT );

		// remove this enemy from the radar
		GameController_Tank.Instance.RemoveEnemyFromRadar( myTransform );
	}

	public void TellGCBossDestroyed()
	{
		// tell the game controller we have been destroyed (and that we are a boss!)
		GameController_Tank.Instance.BossDestroyed();

		// remove this enemy from the radar
		GameController_Tank.Instance.RemoveEnemyFromRadar( myTransform );
	}

	void ReduceLife(float val) {
		myDataManager_New.ReduceDetaleHealth (val);

		if (myDataManager_New.GetDetaleHealth () < 0) {
			LostLife ();
		}
	}

	void LostLife()
	{
		myDataManager_New.ReduceHealth(1);

		if( myDataManager_New.GetHealth()==0 )
		{
			// tell game controller to make an explosion at our position and to award the player points for hitting us
			TellGCEnemyDestroyed();

			// if this is a boss enemy, tell the game controller when we get destroyed so it can end the level
			if( isBoss )
				TellGCBossDestroyed();

			// destroy this
			Destroy(gameObject);
		}
	}
	
}
