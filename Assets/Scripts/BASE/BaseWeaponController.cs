using UnityEngine;
using System.Collections;

[AddComponentMenu("Base/Weapon Controller")]
public class BaseWeaponController : MonoBehaviour
{
	[Header("Base")]
	[SerializeField] protected GameObject[] weapons;
	[SerializeField] protected int selectedWeaponSlot;
	[SerializeField] protected int lastSelectedWeaponSlot;
	[SerializeField] protected Vector3 offsetWeaponSpawnPosition;
	[SerializeField] protected Transform forceParent;

	private ArrayList weaponSlots;
	private ArrayList weaponScripts;
	private BaseWeaponScript tempWeapon;
	private Vector3 tempVector3;
	private Quaternion tempRotation;
	private GameObject tempGameObject;

	private Transform myTransform;
	private int ownerNum;

	[SerializeField] protected bool useForceVectorDirection;
	[SerializeField] protected Vector3 forceVector;

	private Vector3 theDir;

	private void Start()
	{
		Init();
	}

	protected virtual void Init()
	{
		selectedWeaponSlot = 0;
		lastSelectedWeaponSlot = -1;

		weaponSlots = new ArrayList();

		weaponScripts = new ArrayList();

		myTransform = transform;

		if (forceParent == null)
		{
			forceParent = myTransform;
		}

		tempVector3 = forceParent.position;
		tempRotation = forceParent.rotation;

		for (int i = 0; i < weapons.Length; i++)
		{
			tempGameObject = Instantiate(weapons[i], tempVector3 + offsetWeaponSpawnPosition, tempRotation);

			tempGameObject.transform.parent = forceParent;
			tempGameObject.layer = forceParent.gameObject.layer;
			tempGameObject.transform.position = forceParent.position;
			tempGameObject.transform.rotation = forceParent.rotation;

			weaponSlots.Add(tempGameObject);

			tempWeapon = tempGameObject.GetComponent<BaseWeaponScript>();
			weaponScripts.Add(tempWeapon);

			tempGameObject.SetActive(false);
		}

		SetWeaponSlot(0);
	}

	public void SetOwner(int aNum)
	{
		ownerNum = aNum;
	}

	public virtual void SetWeaponSlot(int slotNum)
	{
		if (slotNum == lastSelectedWeaponSlot)
			return;

		DisableCurrentWeapon();

		selectedWeaponSlot = slotNum;

		if (selectedWeaponSlot < 0)
			selectedWeaponSlot = weaponSlots.Count - 1;

		if (selectedWeaponSlot > weaponSlots.Count - 1)
			selectedWeaponSlot = weaponSlots.Count - 1;

		lastSelectedWeaponSlot = selectedWeaponSlot;

		EnableCurrentWeapon();
	}

	public virtual void NextWeaponSlot(bool shouldLoop)
	{
		DisableCurrentWeapon();

		selectedWeaponSlot++;

		if (selectedWeaponSlot == weaponScripts.Count)
		{
			if (shouldLoop)
			{
				selectedWeaponSlot = 0;
			}
			else
			{
				selectedWeaponSlot = weaponScripts.Count - 1;
			}
		}

		lastSelectedWeaponSlot = selectedWeaponSlot;

		EnableCurrentWeapon();
	}

	public virtual void PrevWeaponSlot(bool shouldLoop)
	{
		DisableCurrentWeapon();

		selectedWeaponSlot--;

		if (selectedWeaponSlot < 0)
		{
			if (shouldLoop)
			{
				selectedWeaponSlot = weaponScripts.Count - 1;
			}
			else
			{
				selectedWeaponSlot = 0;
			}
		}

		lastSelectedWeaponSlot = selectedWeaponSlot;

		EnableCurrentWeapon();
	}

	public virtual void DisableCurrentWeapon()
	{
		if (weaponScripts.Count == 0)
			return;

		tempWeapon = (BaseWeaponScript)weaponScripts[selectedWeaponSlot];

		tempWeapon.Disable();

		tempGameObject = (GameObject)weaponSlots[selectedWeaponSlot];
		tempGameObject.SetActive(false);
	}

	public virtual void EnableCurrentWeapon()
	{
		if (weaponScripts.Count == 0)
			return;

		tempWeapon = (BaseWeaponScript)weaponScripts[selectedWeaponSlot];

		tempWeapon.Enable();

		tempGameObject = (GameObject)weaponSlots[selectedWeaponSlot];
		tempGameObject.SetActive(true);
	}

	public virtual void Fire()
	{
		if (weaponScripts == null)
			return;

		if (weaponScripts.Count == 0)
			return;

		tempWeapon = (BaseWeaponScript)weaponScripts[selectedWeaponSlot];

		theDir = transform.forward;

		if (useForceVectorDirection)
			theDir = forceVector;

		tempWeapon.Fire(theDir, ownerNum);
	}
}
