using SOMStudio.TankBattle.Scripts.Base;

namespace SOMStudio.TankBattle.Scripts
{
	public class PlayerManager : BasePlayerManager {

		public UserManager DataManager {
			get => (UserManager)dataManager;
			set => dataManager = value;
		}
	}
}
