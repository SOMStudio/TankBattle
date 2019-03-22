using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager_Tank : BasePlayerManager {

	public UserManager_Tank myDataManager;

	public override void Init ()
	{
		base.Init ();

		if (!myDataManager) {
			myDataManager = gameObject.GetComponent<UserManager_Tank> ();

			if (!myDataManager)
				myDataManager = gameObject.AddComponent<UserManager_Tank> ();
		}
	}
}
