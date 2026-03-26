using UnityEngine;
using System.Collections;

[AddComponentMenu("Base/Weapon Controller")]
public class BaseWeaponController : MonoBehaviour
{
	[Header("Base")] [SerializeField] protected GameObject[] weapons;

	[SerializeField] protected int selectedWeaponSlot;
	[SerializeField] protected int lastSelectedWeaponSlot;

	[SerializeField] protected Vector3 offsetWeaponSpawnPosition;

	[SerializeField] protected Transform forceParent;

	private ArrayList weaponSlots;
	private ArrayList weaponScripts;
	private BaseWeaponScript TEMPWeapon;
	private Vector3 TEMPvector3;
	private Quaternion TEMProtation;
	private GameObject TEMPgameObject;

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

		TEMPvector3 = forceParent.position;
		TEMProtation = forceParent.rotation;

		for (int i = 0; i < weapons.Length; i++)
		{
			TEMPgameObject = (GameObject)Instantiate(weapons[i], TEMPvector3 + offsetWeaponSpawnPosition, TEMProtation);

			TEMPgameObject.transform.parent = forceParent;
			TEMPgameObject.layer = forceParent.gameObject.layer;
			TEMPgameObject.transform.position = forceParent.position;
			TEMPgameObject.transform.rotation = forceParent.rotation;

			weaponSlots.Add(TEMPgameObject);

			TEMPWeapon = TEMPgameObject.GetComponent<BaseWeaponScript>();
			weaponScripts.Add(TEMPWeapon);

			TEMPgameObject.SetActive(false);
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

		TEMPWeapon = (BaseWeaponScript)weaponScripts[selectedWeaponSlot];

		TEMPWeapon.Disable();

		TEMPgameObject = (GameObject)weaponSlots[selectedWeaponSlot];
		TEMPgameObject.SetActive(false);
	}

	public virtual void EnableCurrentWeapon()
	{
		if (weaponScripts.Count == 0)
			return;

		TEMPWeapon = (BaseWeaponScript)weaponScripts[selectedWeaponSlot];

		TEMPWeapon.Enable();

		TEMPgameObject = (GameObject)weaponSlots[selectedWeaponSlot];
		TEMPgameObject.SetActive(true);
	}

	public virtual void Fire()
	{
		if (weaponScripts == null)
			return;

		if (weaponScripts.Count == 0)
			return;

		TEMPWeapon = (BaseWeaponScript)weaponScripts[selectedWeaponSlot];

		theDir = transform.forward;

		if (useForceVectorDirection)
			theDir = forceVector;

		TEMPWeapon.Fire(theDir, ownerNum);
	}
}
