using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController_Tank : BaseGameController
{
	[Header("Main Settings")]
	[SerializeField] private string mainMenuSceneName = "Menu";
	[SerializeField] private float gameSpeed = 1;

	[Header("Player Settings")]
	[SerializeField] private GameObject[] playerPrefabList;
	[SerializeField] private Transform playerParent;
	[SerializeField] private Transform[] startPoints;

	private Vector3[] playerStarts;
	private Quaternion[] playerRotations;

	private ArrayList playerList;
	private ArrayList playerTransforms;

	private PlayerTopDown_Tank thePlayerScript;
	private PlayerTopDown_Tank focusPlayerScript;

	private int countPlayers;

	private UserManager_Tank mainPlayerDataManager1;
	private GameObject playerGameObject;

	[System.NonSerialized] public static GameController_Tank Instance;

	[Header("Managers")]
	[SerializeField] private UI_Tank menuManager;
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

		countPlayers = playerPrefabList.Length;
		
		Vector3[] playerStartPoints = new Vector3 [countPlayers];
		Quaternion[] playerStartRotations = new Quaternion [countPlayers];

		for (int i = 0; i < countPlayers; i++)
		{
			playerStartPoints[i] = startPoints[i].position;
			playerStartRotations[i] = startPoints[i].rotation;
		}

		SpawnController.Instance.SetUpPlayers(playerPrefabList, playerStartPoints, playerStartRotations, playerParent,
			countPlayers);

		playerTransforms = new ArrayList();
		
		playerTransforms = SpawnController.Instance.GetAllSpawnedPlayers();

		playerList = new ArrayList();

		for (int i = 0; i < countPlayers; i++)
		{
			Transform tempT = (Transform)playerTransforms[i];
			PlayerTopDown_Tank tempController = tempT.GetComponent<PlayerTopDown_Tank>();
			
			playerList.Add(tempController);
			tempController.Init();
		}
		
		playerGameObject = SpawnController.Instance.GetPlayerGameObject(0);
		
		thePlayerScript = playerGameObject.GetComponent<PlayerTopDown_Tank>();
		
		thePlayerScript.SetID(0);
		
		thePlayerScript.SetUserInput(true);
		
		focusPlayerScript = thePlayerScript;
		
		Transform aTarget = playerGameObject.transform.Find("CamTarget");

		if (aTarget != null)
		{
			Camera.main?.SendMessage("SetTarget", aTarget);
		}
		else
		{
			Camera.main?.SendMessage("SetTarget", playerGameObject.transform);
		}
		
		radarManager.SetCenterObject(playerGameObject.transform);
	}

	private void StartPlayer()
	{
		mainPlayerDataManager1 = playerGameObject.GetComponent<PlayerManager_Tank>().DataManager_Tank;
		
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
		return GetMainPlayerGameObject().transform;
	}

	public GameObject GetMainPlayerGameObject()
	{
		return playerGameObject;
	}

	public void PlayerDied(int whichID)
	{
		menuManager.ShowGameOver();
		
		Invoke(nameof(Exit), 5);
	}

	private void Exit()
	{
		SceneManager.LoadScene(mainMenuSceneName);
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
