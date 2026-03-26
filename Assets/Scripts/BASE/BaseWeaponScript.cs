using UnityEngine;

[AddComponentMenu("Base/A Weapon Script")]
public class BaseWeaponScript : MonoBehaviour
{
	private bool canFire;

	[Header("Ammo")] [SerializeField] protected int ammo = 100;
	[SerializeField] protected int maxAmmo = 100;

	[SerializeField] protected bool isInfiniteAmmo;
	[SerializeField] protected Collider parentCollider;

	private Vector3 fireVector;
	private Transform myTransform;
	private int myLayer;

	[Header("Projectile")] [SerializeField]
	protected GameObject projectileGO;

	[SerializeField] protected Vector3 spawnPosOffset;
	[SerializeField] protected float forwardOffset = 1.5f;
	[SerializeField] protected float reloadTime = 0.2f;
	[SerializeField] protected float projectileSpeed = 10f;
	[SerializeField] protected bool inheritVelocity;

	private Transform theProjectile;
	private GameObject theProjectileGO;
	private bool isLoaded;
	private ProjectileController theProjectileController;

	public virtual void Start()
	{
		Init();
	}

	public virtual void Init()
	{
		myTransform = transform;

		myLayer = gameObject.layer;

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
		theProjectile = MakeProjectile(ownerID);

		theProjectile.LookAt(theProjectile.position + fireDirection);

		Rigidbody rb = theProjectile.GetComponent<Rigidbody>();
		rb.velocity = fireDirection * projectileSpeed;
	}

	public virtual Transform MakeProjectile(int ownerID)
	{
		theProjectile = SpawnController.Instance.Spawn(projectileGO,
			myTransform.position + spawnPosOffset + (myTransform.forward * forwardOffset), myTransform.rotation);
		theProjectileGO = theProjectile.gameObject;
		theProjectileGO.layer = myLayer;

		theProjectileController = theProjectileGO.GetComponent<ProjectileController>();

		theProjectileController.SetOwnerType(ownerID);

		Physics.IgnoreLayerCollision(myTransform.gameObject.layer, myLayer);

		if (parentCollider != null)
		{
			Physics.IgnoreCollision(theProjectile.GetComponent<Collider>(), parentCollider);
		}

		return theProjectile;
	}
}