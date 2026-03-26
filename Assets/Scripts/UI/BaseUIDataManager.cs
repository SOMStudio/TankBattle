using UnityEngine;

[AddComponentMenu("Base/UI Data Manager")]
public class BaseUIDataManager : MonoBehaviour
{
	[SerializeField] protected string gamePrefsName = "DefaultGame";

	[Header("Base settings")] [SerializeField]
	protected int player_score;

	[SerializeField] protected int player_lives;
	[SerializeField] protected int player_highscore;

	public void UpdateScoreP1(int aScore)
	{
		player_score = aScore;
		if (player_score > player_highscore)
			player_highscore = player_score;
	}

	public void UpdateLivesP1(int aLifeNum)
	{
		player_lives = aLifeNum;
	}

	public void UpdateScore(int aScore)
	{
		player_score = aScore;
	}

	public void UpdateLives(int aLifeNum)
	{
		player_lives = aLifeNum;
	}

	public void LoadHighScore()
	{
		if (PlayerPrefs.HasKey(gamePrefsName + "_highScore"))
		{
			player_highscore = PlayerPrefs.GetInt(gamePrefsName + "_highScore");
		}
	}

	public void SaveHighScore()
	{
		PlayerPrefs.SetInt(gamePrefsName + "_highScore", player_highscore);
	}
}
