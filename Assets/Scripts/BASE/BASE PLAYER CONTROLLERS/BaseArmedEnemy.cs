using UnityEngine;
using AIAttackStates;

[AddComponentMenu("Base/AI Weapon Controller")]

public class BaseArmedEnemy : ExtendedCustomMonoBehaviour
{
	[Header("Main value")] [SerializeField]
	protected int pointsValue = 50;

	[SerializeField] private int thisEnemyStrength = 1;
	[SerializeField] private GameObject mesh_parentGO;

	protected Renderer rendererToTestAgainst;

	[Header("Fire value")] [SerializeField]
	protected Standard_SlotWeaponController weaponControl;

	[SerializeField] protected AIAttackState currentState = AIAttackState.random_fire;
	[SerializeField] private bool thisGameObjectShouldFire;
	[SerializeField] private bool onlyFireWhenOnscreen;
	[SerializeField] private float fireDelayTime = 1f;
	[SerializeField] protected string tagOfTargetsToShootAt;

	[Header("Player Manager")] [SerializeField]
	protected BasePlayerManager myPlayerManager;

	[SerializeField] protected BaseUserManager myDataManager;
	[SerializeField] protected bool isBoss = false;

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
			weaponControl = myGO.GetComponent<Standard_SlotWeaponController>();
		}

		if (rendererToTestAgainst == null)
		{
			rendererToTestAgainst = myGO.GetComponentInChildren<Renderer>();
		}

		if (myPlayerManager == null)
		{
			myPlayerManager = myGO.AddComponent<BasePlayerManager>();
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
				if (currentState == AIAttackState.random_fire)
				{
					if (Random.Range(0, 100) > 98)
					{
						doFire = true;
					}
				}
				else if (currentState == AIAttackState.look_and_destroy)
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

	public void SetAIState(AIAttackState val)
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
