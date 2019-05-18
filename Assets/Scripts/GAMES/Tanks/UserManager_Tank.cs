using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager_Tank : BaseUserManager {

	[SerializeField]
	private float detaleHelth;
	[SerializeField]
	private float protection;

	// main logic
	public override void GetDefaultData()
	{
		base.GetDefaultData ();

		detaleHelth = 100;
		protection = 1;
	}

	//protection
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

	//detale helth
	public float GetDetaleHealth()
	{
		return detaleHelth;
	}

	public void AddDetaleHealth(float num)
	{
		detaleHelth += num;
	}

	public void ReduceDetaleHealth(float num)
	{
		detaleHelth -= num;
	}

	public void SetDetaleHealth(float num)
	{
		detaleHelth = num;
	}
}
