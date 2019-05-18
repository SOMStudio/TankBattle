using UnityEngine;
using System.Collections;

public class UI_Tank : BaseUIDataManager
{
	[SerializeField]
	private int player_livesdetale;

	[Header("Message Texture")]
	[SerializeField]
	private GameObject gameOverMessage;
	[SerializeField]
	private GameObject getReadyMessage;

	[System.NonSerialized]
	public static UI_Tank Instance;

	// main event
	void Awake()
	{
		Init ();
	}

	void Start() {
		StartGame ();
	}

	void OnGUI()
	{
		GUI.Label(new Rect (10,10,100,50),"PLAYER 1");
		GUI.Label(new Rect (10,40,100,50),"SCORE "+player_score);
		GUI.Label(new Rect (10,70,200,50),"HIGH SCORE "+player_highscore);

		GUI.Label(new Rect (10,100,100,50),"LIVES "+player_lives);
		GUI.Label(new Rect (10,130,100,50),"LIVES % "+player_livesdetale);
	}

	// main logic
	void Init()
	{
		// activate instance
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);
		}
	}

	void StartGame() {
		LoadHighScore();

		HideMessages ();

		Invoke("ShowGetReady",1);
		Invoke("HideMessages",2);
	}

	public void UpdateLivesDetale( float alifeDetaleNum )
	{
		player_livesdetale = Mathf.RoundToInt (alifeDetaleNum);
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
		
		// show the game over message
		gameOverMessage.SetActive(true);
	}
}
