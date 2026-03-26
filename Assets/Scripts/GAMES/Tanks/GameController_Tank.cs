using UnityEngine;
using System.Collections;

public class GameController_Tank : BaseGameController
{
	[Header("Main Settings")] [SerializeField]
	private string mainMenuSceneName = "menu_Tanks";

	[SerializeField] private float gameSpeed = 1;

	[Header("Player Settings")] [SerializeField]
	private GameObject[] playerPrefabList;

	[SerializeField] private Transform playerParent;
	[SerializeField] private Transform[] startPoints;

	private Vector3[] playerStarts;
	private Quaternion[] playerRotations;

	private ArrayList playerList;
	private ArrayList playerTransforms;

	private PlayerTopDown_Tank thePlayerScript;
	private PlayerTopDown_Tank focusPlayerScript;

	private int numberOfPlayers;

	private UserManager_Tank mainPlayerDataManager1;
	private GameObject playerGO1;

	[System.NonSerialized] public static GameController_Tank Instance;

	[Header("Managers")] [SerializeField] private UI_Tank menuManager;
	[SerializeField] private SpawnController_Tank spawnManager;
	[SerializeField] private RadarGUI radarManager;
	[SerializeField] private BaseSoundController soundManager;

	private void Awake()
	{
		Init();
	}

	private void Start()
	{
		StartGame();

		Time.timeScale = gameSpeed;
	}

	private void Init()
	{
		if (Instance == null)
		{
			Instance = this;

			InitManagers();
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
	}

	void InitManagers()
	{
		if (!menuManager)
		{
			menuManager = UI_Tank.Instance;
		}

		if (!soundManager)
		{
			soundManager = BaseSoundController.Instance;
		}
	}

	public override void StartGame()
	{
		Invoke(nameof(StartPlayer), 1);

		SpawnController.Instance.Restart();

		numberOfPlayers = playerPrefabList.Length;
		
		Vector3[] playerStarts = new Vector3 [numberOfPlayers];
		Quaternion[] playerRotations = new Quaternion [numberOfPlayers];
		

		for (int i = 0; i < numberOfPlayers; i++)
		{
			playerStarts[i] = (Vector3)startPoints[i].position;
			playerRotations[i] = (Quaternion)startPoints[i].rotation;
		}

		SpawnController.Instance.SetUpPlayers(playerPrefabList, playerStarts, playerRotations, playerParent,
			numberOfPlayers);

		playerTransforms = new ArrayList();
		
		playerTransforms = SpawnController.Instance.GetAllSpawnedPlayers();

		playerList = new ArrayList();

		for (int i = 0; i < numberOfPlayers; i++)
		{
			Transform tempT = (Transform)playerTransforms[i];
			PlayerTopDown_Tank tempController = tempT.GetComponent<PlayerTopDown_Tank>();
			playerList.Add(tempController);
			tempController.Init();
		}
		
		playerGO1 = SpawnController.Instance.GetPlayerGO(0);
		
		thePlayerScript = (PlayerTopDown_Tank)playerGO1.GetComponent<PlayerTopDown_Tank>();
		
		thePlayerScript.SetID(0);
		
		thePlayerScript.SetUserInput(true);
		
		focusPlayerScript = thePlayerScript;
		
		Transform aTarget = playerGO1.transform.Find("CamTarget");

		if (aTarget != null)
		{
			Camera.main.SendMessage("SetTarget", aTarget);
		}
		else
		{
			Camera.main.SendMessage("SetTarget", playerGO1.transform);
		}
		
		radarManager.SetCenterObject(playerGO1.transform);
	}

	private void StartPlayer()
	{
		mainPlayerDataManager1 = playerGO1.GetComponent<PlayerManager_Tank>().DataManager_Tank;
		
		thePlayerScript.GameStart();
	}

	public override void EnemyDestroyed(Vector3 aPosition, int pointsValue, int hitByID)
	{
		BaseSoundController.Instance.PlaySoundByIndex(1, aPosition);
		
		mainPlayerDataManager1.AddScore(pointsValue);
		
		UpdateScoreP1(mainPlayerDataManager1.GetScore());
		
		Explode(aPosition);
		
		spawnManager.DecreaseCountSpawnedObject();
		spawnManager.SpawnRandomObjectWithLimit();
	}

	public void PlayerHit(Transform whichPlayer)
	{
		BaseSoundController.Instance.PlaySoundByIndex(2, whichPlayer.position);
		
		Explode(whichPlayer.position);
	}

	public void AddEnemyToRadar(Transform aTransform)
	{
		radarManager.AddEnemyBlipToList(aTransform);
	}

	public void RemoveEnemyFromRadar(Transform aTransform)
	{
		radarManager.RemoveEnemyBlip(aTransform);
	}

	public PlayerTopDown_Tank GetMainPlayerScript()
	{
		return focusPlayerScript;
	}

	public Transform GetMainPlayerTransform()
	{
		return playerGO1.transform;
	}

	public GameObject GetMainPlayerGO()
	{
		return playerGO1;
	}

	public void PlayerDied(int whichID)
	{
		menuManager.ShowGameOver();
		
		Invoke(nameof(Exit), 5);
	}

	private void Exit()
	{
		Application.LoadLevel(mainMenuSceneName);
	}
	
	public void UpdateScoreP1(int aScore)
	{
		menuManager.UpdateScoreP1(aScore);
	}

	public void UpdateLivesP1(int aLives)
	{
		menuManager.UpdateLivesP1(aLives);
	}

	public void UpdateLivesDetailP1(float aLivesDetail)
	{
		menuManager.UpdateLivesDetail(aLivesDetail);
	}
}
