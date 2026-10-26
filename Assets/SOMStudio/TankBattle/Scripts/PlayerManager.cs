using SOMStudio.TankBattle.Scripts.Base;
using UnityEngine;

namespace SOMStudio.TankBattle.Scripts
{
	[AddComponentMenu("SOMStudio/TankBattle/Player Manager")]
	public class PlayerManager : BasePlayerManager {

		public UserManager DataManager {
			get => (UserManager)dataManager;
			set => dataManager = value;
		}
	}
}
