using UnityEngine;
using System.Collections;

[AddComponentMenu("Common/Projectile Control")]

public class ProjectileController : MonoBehaviour
{
	[Header("Particle")]
	[SerializeField]
	private bool doProjectileHitParticle;
	[SerializeField]
	private GameObject particleEffectPrefab;

	[Header("Owner")]
	[SerializeField]
	private int ownerType_id;

	private Transform myTransform;
	private Vector3 tempVEC;

	[SerializeField]
	private int overrideDamageValue;
	[SerializeField]
	private int overridePoints;

	[Header("Ground")]
	[SerializeField]
	private bool shouldFollowGround;
	[SerializeField]
	private float groundHeightOffset = 15f;
	[SerializeField]
	private LayerMask groundLayerMask;

	private bool didPlaySound;
	private int whichSoundToPlayOnStart = 0;

	// main event
	void Start ()
	{
		myTransform = transform;

		didPlaySound = false;
	}

	void Update()
	{
		if (!didPlaySound) {
			// tell our sound controller to play a pew sound
			BaseSoundController.Instance.PlaySoundByIndex (whichSoundToPlayOnStart, myTransform.position);
			// we only want to play the sound once, so set didPlaySound here
			didPlaySound = true;
		}

		if (shouldFollowGround) {
			// cast a ray down from the waypoint to try to find the ground
			tempVEC = myTransform.position;

			RaycastHit hit;
			if (Physics.Raycast (tempVEC, -Vector3.up, out hit, groundLayerMask)) {
				tempVEC.y = hit.point.y + groundHeightOffset;
				myTransform.position = tempVEC;
			}
		}
	}

	void OnCollisionEnter(Collision col)
	{
		// if we have assigned a particle effect, we will instantiate one when a collision happens.
		if (doProjectileHitParticle)
			Instantiate (particleEffectPrefab, transform.position, Quaternion.identity);

		// destroy this game object after a collision
		Destroy (gameObject);
	}

	// main logic
	public int OverrideDamageValue {
		get { return overrideDamageValue; }
	}

	public void SetOwnerType(int aNum)
	{
		ownerType_id = aNum;
		transform.name = aNum.ToString ();
	}
}
