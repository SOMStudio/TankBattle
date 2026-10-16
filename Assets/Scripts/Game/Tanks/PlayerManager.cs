public class PlayerManager : BasePlayerManager {

	public UserManager DataManager {
		get => (UserManager)dataManager;
		set => dataManager = value;
	}
}
