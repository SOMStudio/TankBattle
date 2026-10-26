using SOMStudio.TankBattle.Scripts.Tools.Spawn;
using SOMStudio.TankBattle.Scripts.Tools.Weapon;
using UnityEngine;

namespace SOMStudio.TankBattle.Scripts.Base
{
	[AddComponentMenu("SOMStudio/TankBattle/Base/Weapon Script")]
	public class BaseWeaponScript : ExtendedCustomMonoBehaviour
	{
		private bool canFire;

		[Header("Ammo")]
		[SerializeField] protected int ammo = 100;
		[SerializeField] protected int maxAmmo = 100;

		[SerializeField] protected bool isInfiniteAmmo;
		[SerializeField] protected Collider parentCollider;

		private Vector3 fireVector;
		private int myLayer;

		[Header("Projectile")]
		[SerializeField] protected GameObject projectilePrefab;
		[SerializeField] protected Vector3 spawnPosOffset;
		[SerializeField] protected float forwardOffset = 1.5f;
		[SerializeField] protected float reloadTime = 0.2f;
		[SerializeField] protected float projectileSpeed = 10f;
		[SerializeField] protected bool inheritVelocity;

		private Transform projectileTransform;
		private GameObject projectileGameObject;
		private bool isLoaded;
		private ProjectileController theProjectileController;
	
		public override void Init()
		{
			base.Init();
		
			myLayer = myGameObject.layer;
			Reloaded();
		}

		public virtual void Enable()
		{
			canFire = true;
		}

		public virtual void Disable()
		{
			canFire = false;
		}

		public virtual void Reloaded()
		{
			isLoaded = true;
		}

		public virtual void SetCollider(Collider aCollider)
		{
			parentCollider = aCollider;
		}

		public virtual void Fire(Vector3 aDirection, int ownerID)
		{
			if (!canFire)
				return;

			if (!isLoaded)
				return;

			if (ammo <= 0 && !isInfiniteAmmo)
				return;

			ammo--;

			FireProjectile(aDirection, ownerID);

			isLoaded = false;

			CancelInvoke(nameof(Reloaded));
			Invoke(nameof(Reloaded), reloadTime);
		}

		public virtual void FireProjectile(Vector3 fireDirection, int ownerID)
		{
			projectileTransform = MakeProjectile(ownerID);

			projectileTransform.LookAt(projectileTransform.position + fireDirection);

			Rigidbody rb = projectileTransform.GetComponent<Rigidbody>();
			rb.velocity = fireDirection * projectileSpeed;
		}

		public virtual Transform MakeProjectile(int ownerID)
		{
			projectileTransform = SpawnTool.Instance.Spawn(projectilePrefab,
				myTransform.position + spawnPosOffset + (myTransform.forward * forwardOffset), myTransform.rotation);
			projectileGameObject = projectileTransform.gameObject;
			projectileGameObject.layer = myLayer;

			theProjectileController = projectileGameObject.GetComponent<ProjectileController>();

			theProjectileController.SetOwnerType(ownerID);

			Physics.IgnoreLayerCollision(myTransform.gameObject.layer, myLayer);

			if (parentCollider != null)
			{
				Physics.IgnoreCollision(projectileTransform.GetComponent<Collider>(), parentCollider);
			}

			return projectileTransform;
		}
	}
}