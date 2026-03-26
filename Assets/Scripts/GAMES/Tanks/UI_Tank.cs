using UnityEngine;
using UnityEngine.Serialization;

public class UI_Tank : BaseUIDataManager
{
	[SerializeField] private int playerLivesDetail;

	[Header("Message Texture")] [SerializeField]
	private GameObject gameOverMessage;

	[SerializeField] private GameObject getReadyMessage;

	[System.NonSerialized] public static UI_Tank Instance;

	private void Awake()
	{
		Init();
	}

	private void Start()
	{
		StartGame();
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 100, 50), "PLAYER 1");
		GUI.Label(new Rect(10, 40, 100, 50), "SCORE " + player_score);
		GUI.Label(new Rect(10, 70, 200, 50), "HIGH SCORE " + player_highscore);

		GUI.Label(new Rect(10, 100, 100, 50), "LIVES " + player_lives);
		GUI.Label(new Rect(10, 130, 100, 50), "LIVES % " + playerLivesDetail);
	}

	private void Init()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
	}

	private void StartGame()
	{
		LoadHighScore();

		HideMessages();

		Invoke(nameof(ShowGetReady), 1);
		Invoke(nameof(HideMessages), 2);
	}

	public void UpdateLivesDetail(float aLivesDetail)
	{
		playerLivesDetail = Mathf.RoundToInt(aLivesDetail);
	}

	public void HideMessages()
	{
		gameOverMessage.SetActive(false);
		getReadyMessage.SetActive(false);
	}

	public void ShowGetReady()
	{
		getReadyMessage.SetActive(true);
	}

	public void ShowGameOver()
	{
		SaveHighScore();

		gameOverMessage.SetActive(true);
	}
}
