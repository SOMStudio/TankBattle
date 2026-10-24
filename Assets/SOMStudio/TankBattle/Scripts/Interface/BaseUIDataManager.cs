using UnityEngine;

namespace SOMStudio.TankBattle.Scripts.Interface
{
	[AddComponentMenu("Base/UI Data Manager")]
	public class BaseUIDataManager : MonoBehaviour
	{
		[SerializeField] protected string gamePrefsName = "DefaultGame";

		[Header("Base settings")]
		[SerializeField] protected int playerScore;
		[SerializeField] protected int playerLives;
		[SerializeField] protected int playerHighScore;

		public void UpdateScoreP1(int aScore)
		{
			playerScore = aScore;
			if (playerScore > playerHighScore)
				playerHighScore = playerScore;
		}

		public void UpdateLivesP1(int aLifeNum)
		{
			playerLives = aLifeNum;
		}

		public void UpdateScore(int aScore)
		{
			playerScore = aScore;
		}

		public void UpdateLives(int aLifeNum)
		{
			playerLives = aLifeNum;
		}

		public void LoadHighScore()
		{
			if (PlayerPrefs.HasKey(gamePrefsName + "_highScore"))
			{
				playerHighScore = PlayerPrefs.GetInt(gamePrefsName + "_highScore");
			}
		}

		public void SaveHighScore()
		{
			PlayerPrefs.SetInt(gamePrefsName + "_highScore", playerHighScore);
		}
	}
}
