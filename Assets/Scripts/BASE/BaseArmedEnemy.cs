using UnityEngine;
using AIAttackStates;

[AddComponentMenu("Base/AI Weapon Controller")]
public class BaseArmedEnemy : ExtendedCustomMonoBehaviour
{
	[Header("Main value")]
	[SerializeField] protected int pointsValue = 50;
	[SerializeField] private int thisEnemyStrength = 1;
	[SerializeField] private GameObject meshParentGameObject;

	protected Renderer rendererToTestAgainst;

	[Header("Fire value")]
	[SerializeField] protected Standard_SlotWeaponController weaponControl;
	[SerializeField] protected AiAttackState currentState = AiAttackState.random_fire;
	[SerializeField] private bool thisGameObjectShouldFire;
	[SerializeField] private bool onlyFireWhenOnscreen;
	[SerializeField] private float fireDelayTime = 1f;
	[SerializeField] protected string tagOfTargetsToShootAt;

	[Header("Managers")]
	[SerializeField] protected BasePlayerManager myPlayerManager;
	[SerializeField] protected BaseUserManager myDataManager;
	[SerializeField] protected bool isBoss;

	protected int tempINT;

	private RaycastHit rayHit;
	private bool doFire;
	private bool canFire;

	private void Update()
	{
		UpdateState();
	}

	public override void Init()
	{
		base.Init();

		didInit = false;

		if (weaponControl == null)
		{
			weaponControl = myGameObject.GetComponent<Standard_SlotWeaponController>();
		}

		if (rendererToTestAgainst == null)
		{
			rendererToTestAgainst = myGameObject.GetComponentInChildren<Renderer>();
		}

		if (myPlayerManager == null)
		{
			myPlayerManager = myGameObject.AddComponent<BasePlayerManager>();
		}

		myDataManager = myPlayerManager.GetDataManager();
		myDataManager.SetName("Enemy");
		myDataManager.SetHealth(thisEnemyStrength);

		canFire = true;
		didInit = true;
	}

	protected virtual void UpdateState()
	{
		if (!canControl)
			return;

		if (thisGameObjectShouldFire)
		{
			doFire = false;

			if (canFire)
			{
				if (currentState == AiAttackState.random_fire)
				{
					if (Random.Range(0, 100) > 98)
					{
						doFire = true;
					}
				}
				else if (currentState == AiAttackState.look_and_destroy)
				{
					if (Physics.Raycast(myTransform.position, myTransform.forward, out rayHit))
					{
						if (rayHit.transform.CompareTag(tagOfTargetsToShootAt))
						{
							doFire = true;
						}
					}
				}
				else
				{
					doFire = true;
				}
			}

			if (doFire)
			{
				if (onlyFireWhenOnscreen && !rendererToTestAgainst.IsVisibleFrom(Camera.main))
				{
					doFire = false;
					return;
				}

				if (weaponControl != null)
				{
					weaponControl.Fire();
				}

				canFire = false;

				Invoke(nameof(ResetFire), fireDelayTime);
			}
		}
	}

	public void SetAIState(AiAttackState val)
	{
		currentState = val;
	}

	public void CanControl(bool val)
	{
		canControl = val;
	}

	public void SetFire(bool val)
	{
		canFire = val;
	}

	public void ResetFire()
	{
		SetFire(true);
	}
}
