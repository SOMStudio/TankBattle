using SOMStudio.TankBattle.Scripts.Base;
using SOMStudio.TankBattle.Scripts.Tools.Weapon;
using UnityEngine;

namespace SOMStudio.TankBattle.Scripts
{
	[AddComponentMenu("SOMStudio/TankBattle/Player Top Down")]
	public class PlayerTopDown : BaseTopDown
	{
		private bool isInvulnerable;
		private bool isRespawning;

		[Header("Managers")]
		[SerializeField] private PlayerManager myPlayerManager;
		[SerializeField] private UserManager myDataManager;

		private bool godMode = false;

		[Header("Weapon")]
		[SerializeField] private SlotWeaponController weaponControl;
		[SerializeField] private bool canFire;
	
		[SerializeField] private bool isFinished;

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.layer == 10 && !isRespawning && !isInvulnerable) {
				ArmedEnemy enemyManager = collision.gameObject.GetComponent<ArmedEnemy> ();
				if (enemyManager) {
					float reduceHels = myDataManager.GetProtection () * enemyManager.GetContactDamage ();

					ReduceLife (reduceHels);
				}
			}
		}
	
		public override void Init ()
		{
			base.Init ();
		
			if (!godMode) {
				MakeVulnerable ();
			} else {
				MakeInvulnerable ();
			}
		
			canControl = false;
		
			weaponControl = myGameObject.GetComponent<SlotWeaponController> ();
		
			if (myPlayerManager == null)
				myPlayerManager = myGameObject.GetComponent<PlayerManager> ();
		
			myDataManager = myPlayerManager.DataManager;
			myDataManager.SetName ("Player");
			myDataManager.SetHealth (3);
			myDataManager.SetDetailHealth (100);
			myDataManager.SetProtection (0.5f);
		
			isFinished = false;
		
			GameController.Instance.UpdateLivesP1 (myDataManager.GetHealth ());
			GameController.Instance.UpdateLivesDetailP1 (myDataManager.GetDetailHealth ());
		}
	
		protected override void GetInput()
		{
			if (isFinished || isRespawning) {
				horizontal = 0;
				vertical = 0;
				return;
			}
		
			if (!canControl)
				return;
		
			horizontal = Mathf.Clamp (defaultInput.GetHorizontal (), -1, 1);
			vertical = Mathf.Clamp (defaultInput.GetVertical (), -1, 1);
		
			if (defaultInput.GetFire () && canFire) {
				weaponControl.Fire ();
			}
		}
	
		public void GameStart()
		{
			canControl = true;
		}

		public void PlayerFinished()
		{
			GameController.Instance.PlayerDied (id);

			isFinished = true;
		}

		private void ReduceLife(float val) {
			myDataManager.ReduceDetailHealth (val);

			if (myDataManager.GetDetailHealth () < 0) {
				GameController.Instance.UpdateLivesDetailP1(0);

				LostLife ();
			} else {
				GameController.Instance.UpdateLivesDetailP1(myDataManager.GetDetailHealth());
			}
		}

		private void LostLife()
		{
			isRespawning = true;
		
			GameController.Instance.PlayerHit (myTransform);
		
			myDataManager.ReduceHealth (1);
		
			GameController.Instance.UpdateLivesP1 (myDataManager.GetHealth ());
		
			if (myDataManager.GetHealth () < 1) {
				if (myBody != null) {
					if (!myBody.isKinematic)
						myBody.velocity = Vector3.zero;
				}
			
				myGameObject.SetActive (false);
			
				weaponControl.DisableCurrentWeapon ();
			
				PlayerFinished ();
			} else {
				myGameObject.SetActive (false);
			
				weaponControl.DisableCurrentWeapon ();
			
				Invoke (nameof(Respawn), 2f);
			}
		}

		private void Respawn()
		{
			isRespawning = false;
		
			MakeInvulnerable ();
		
			Invoke (nameof(MakeVulnerable), 3);
		
			myDataManager.SetDetailHealth (100);
			GameController.Instance.UpdateLivesDetailP1 (myDataManager.GetDetailHealth ());
		
			myGameObject.SetActive (true);
		
			weaponControl.SetWeaponSlot (0);
		
			weaponControl.EnableCurrentWeapon ();
		}

		private void MakeInvulnerable()
		{
			isInvulnerable = true;
		}

		private void MakeVulnerable()
		{
			isInvulnerable = false;
		}
	}
}
