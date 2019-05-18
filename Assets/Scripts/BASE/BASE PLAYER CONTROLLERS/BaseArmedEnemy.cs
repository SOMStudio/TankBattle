using UnityEngine;
using System.Collections;
using AIAttackStates;

[AddComponentMenu("Base/AI Weapon Controller")]

public class BaseArmedEnemy : ExtendedCustomMonoBehaviour
{
	[Header("Main value")]
	[SerializeField]
	protected int pointsValue = 50;
	[SerializeField]
	private int thisEnemyStrength = 1;
	[SerializeField]
	private GameObject mesh_parentGO;

	// we use a renderer to test whether or not the ship is on screen
	protected Renderer rendererToTestAgainst;

	[Header("Fire value")]
	[SerializeField]
	protected Standard_SlotWeaponController weaponControl;
	[SerializeField]
	protected AIAttackState currentState = AIAttackState.random_fire; // default action is to attack nothing
	[SerializeField]
	private bool thisGameObjectShouldFire;
	[SerializeField]
	private bool onlyFireWhenOnscreen;
	[SerializeField]
	private float fireDelayTime = 1f;
	[SerializeField]
	protected string tagOfTargetsToShootAt;

	[Header("Player Manager")]
	[SerializeField]
	protected BasePlayerManager myPlayerManager;
	[SerializeField]
	protected BaseUserManager myDataManager;
	[SerializeField]
	protected bool isBoss = false;
	
	protected int tempINT;

	private RaycastHit rayHit;
	private bool doFire;
	private bool canFire;

	// main event
	void Update() {
		UpdateState ();
	}

	// main logic
	public override void Init()
	{
		base.Init ();

		didInit = false;
		
		if (weaponControl == null) {
			// try to find weapon controller on this gameobject
			weaponControl = myGO.GetComponent<Standard_SlotWeaponController> ();
		}
		
		if (rendererToTestAgainst == null) {
			// we need a renderer to find out whether or not we are on-screen
			rendererToTestAgainst = myGO.GetComponentInChildren<Renderer> ();
		}
				
		// if a player manager is not set in the editor, let's try to find one
		if (myPlayerManager == null) {
			myPlayerManager = myGO.AddComponent<BasePlayerManager> ();
		}
		
		myDataManager = myPlayerManager.GetDataManager ();
		myDataManager.SetName("Enemy");
		myDataManager.SetHealth(thisEnemyStrength);

		canFire=true;
		didInit=true;
	}
	
	protected virtual void UpdateState ()
	{
		// if we are not allowed to control the weapon, we drop out here
		if(!canControl)
			return;
		
		if(thisGameObjectShouldFire)
		{
			// we use doFire to determine whether or not to fire right now
			doFire=false;
			
			// canFire is used to control a delay between firing
			if (canFire) {
				if (currentState == AIAttackState.random_fire) {
					// if the random number is over x, fire
					if (Random.Range (0, 100) > 98) {
						doFire = true;
					}
				} else if (currentState == AIAttackState.look_and_destroy) {
					if (Physics.Raycast (myTransform.position, myTransform.forward, out rayHit)) {
						// is it an opponent to be shot at?
						if (rayHit.transform.CompareTag (tagOfTargetsToShootAt)) {
							//	we have a match on the tag, so let's shoot at it
							doFire = true;
						}
					}
				} else {
					// if we're not set to random fire or look and destroy, just fire whenever we can
					doFire = true;	
				}
			}
				
			if (doFire) {
				// we only want to fire if we are on-screen, visible on the main camera
				if (onlyFireWhenOnscreen && !rendererToTestAgainst.IsVisibleFrom (Camera.main)) {
					doFire = false;
					return;
				}
				
				// tell weapon control to fire, if we have a weapon controller
				if (weaponControl != null) {
					// tell weapon to fire
					weaponControl.Fire ();
				}
				// set a flag to disable firing temporarily (providing a delay between firing)
				canFire = false;
				// invoke a function call in <fireDelayTime> to reset canFire back to true, allowing another firing session
				Invoke ("ResetFire", fireDelayTime);
			}
		}
	}

	/// <summary>
	/// Sets AI state for Gun.
	/// </summary>
	/// <param name="val">AIAttackState</param>
	public void SetAIState(AIAttackState val) {
		currentState = val;
	}

	/// <summary>
	/// Sets AI control for Gun.
	/// </summary>
	/// <param name="val">If set to <c>true</c> AI controll auto shot.</param>
	public void CanControl(bool val) {
		canControl = val;
	}

	/// <summary>
	/// Sets AI can fire.
	/// </summary>
	/// <param name="val">If set to <c>true</c> AI can fire.</param>
	public void SetFire (bool val) {
		canFire = val;
	}

	/// <summary>
	/// Resets AI can fire.
	/// </summary>
	public void ResetFire () {
		SetFire (true);
	}
	
}
