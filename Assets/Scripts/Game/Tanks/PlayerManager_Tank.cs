public class PlayerManager_Tank : BasePlayerManager {

	public UserManager_Tank DataManager_Tank {
		get => (UserManager_Tank)dataManager;
		set => dataManager = value;
	}
}
