using SOMStudio.TankBattle.Scripts.Base;
using UnityEngine;

namespace SOMStudio.TankBattle.Scripts.Tools.Weapon
{
	[AddComponentMenu("SOMStudio/TankBattle/Tools/Projectile Control")]
	public class ProjectileController : MonoBehaviour
	{
		[Header("Particle")]
		[SerializeField] private bool doProjectileHitParticle;
		[SerializeField] private GameObject particleEffectPrefab;

		[Header("Owner")]
		[SerializeField] private int ownerTypeId;

		private Transform myTransform;
		private Vector3 tempVector;

		[SerializeField] private int overrideDamageValue;
		[SerializeField] private int overridePoints;

		[Header("Ground")] [SerializeField] private bool shouldFollowGround;
		[SerializeField] private float groundHeightOffset = 15f;
		[SerializeField] private LayerMask groundLayerMask;

		private bool didPlaySound;
		private readonly int whichSoundToPlayOnStart = 0;

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
				tempVector = myTransform.position;

				RaycastHit hit;
				if (Physics.Raycast(tempVector, -Vector3.up, out hit, groundLayerMask))
				{
					tempVector.y = hit.point.y + groundHeightOffset;
					myTransform.position = tempVector;
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
			ownerTypeId = aNum;
			transform.name = aNum.ToString();
		}
	}
}
