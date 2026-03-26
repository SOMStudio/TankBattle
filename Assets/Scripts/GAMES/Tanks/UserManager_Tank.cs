using UnityEngine;

public class UserManager_Tank : BaseUserManager {

	[SerializeField]
	private float detaleHelth;
	[SerializeField]
	private float protection;
	
	public override void GetDefaultData()
	{
		base.GetDefaultData ();

		detaleHelth = 100;
		protection = 1;
	}
	
	public float GetProtection()
	{
		return protection;
	}

	public void AddProtection(float num)
	{
		protection += num;
	}

	public void ReduceProtection(float num)
	{
		protection -= num;
	}

	public void SetProtection(float num)
	{
		protection = num;
	}
	
	public float GetDetailHealth()
	{
		return detaleHelth;
	}

	public void AddDetailHealth(float num)
	{
		detaleHelth += num;
	}

	public void ReduceDetailHealth(float num)
	{
		detaleHelth -= num;
	}

	public void SetDetailHealth(float num)
	{
		detaleHelth = num;
	}
}
