using UnityEngine;

public class PlayerManager_Tank : BasePlayerManager {

	public UserManager_Tank DataManager_Tank {
		get { return (UserManager_Tank)DataManager; }
		set { DataManager = value; }
	}
}
