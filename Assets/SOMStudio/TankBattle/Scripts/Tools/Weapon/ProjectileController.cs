using UnityEngine;

[AddComponentMenu("Common/Projectile Control")]
public class ProjectileController : MonoBehaviour
{
	[Header("Particle")] [SerializeField] private bool doProjectileHitParticle;
	[SerializeField] private GameObject particleEffectPrefab;

	[Header("Owner")] [SerializeField] private int ownerType_id;

	private Transform myTransform;
	private Vector3 tempVEC;

	[SerializeField] private int overrideDamageValue;
	[SerializeField] private int overridePoints;

	[Header("Ground")] [SerializeField] private bool shouldFollowGround;
	[SerializeField] private float groundHeightOffset = 15f;
	[SerializeField] private LayerMask groundLayerMask;

	private bool didPlaySound;
	private int whichSoundToPlayOnStart = 0;

	private void Start()
	{
		myTransform = transform;

		didPlaySound = false;
	}

	private void Update()
	{
		if (!didPlaySound)
		{
			BaseSoundController.Instance.PlaySoundByIndex(whichSoundToPlayOnStart, myTransform.position);
			didPlaySound = true;
		}

		if (shouldFollowGround)
		{
			tempVEC = myTransform.position;

			RaycastHit hit;
			if (Physics.Raycast(tempVEC, -Vector3.up, out hit, groundLayerMask))
			{
				tempVEC.y = hit.point.y + groundHeightOffset;
				myTransform.position = tempVEC;
			}
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		if (doProjectileHitParticle)
			Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);

		Destroy(gameObject);
	}

	public int OverrideDamageValue => overrideDamageValue;

	public void SetOwnerType(int aNum)
	{
		ownerType_id = aNum;
		transform.name = aNum.ToString();
	}
}
