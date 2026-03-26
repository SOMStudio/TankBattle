public class PlayerManager_Tank : BasePlayerManager {

	public UserManager_Tank DataManager_Tank {
		get => (UserManager_Tank)DataManager;
		set => DataManager = value;
	}
}
