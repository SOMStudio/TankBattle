using UnityEngine;
using System.Collections;

public class GameController_Tank : BaseGameController
{
	[Header("Main Settings")]
	[SerializeField]
	private string mainMenuSceneName = "menu_Tanks";
	[SerializeField]
	private float gameSpeed=1;

	[Header("Player Settings")]
	[SerializeField]
	private GameObject[] playerPrefabList;
	
	[SerializeField]
	private Transform playerParent;
	[SerializeField]
	private Transform [] startPoints;
    
	private Vector3[] playerStarts;
	private Quaternion[] playerRotations;
	
    private ArrayList playerList;
	private ArrayList playerTransforms;
	
	private PlayerTopDown_Tank thePlayerScript;
	private PlayerTopDown_Tank focusPlayerScript;

	private int numberOfPlayers;

	private UserManager_Tank mainPlayerDataManager1;
	private GameObject playerGO1;

	[System.NonSerialized]
	public static GameController_Tank Instance;

	[Header("Managers")]
	[SerializeField]
	private UI_Tank menuManager;
	[SerializeField]
	private SpawnController_Tank spawnManager;
	[SerializeField]
	private RadarGUI radarManager;
	[SerializeField]
	private BaseSoundController soundManager;

	// main event
	void Awake() {
		// init object
		Init ();
	}
	
	public void Start()
	{
		StartGame ();

		Time.timeScale = gameSpeed;
	}

	// main logic
	void Init() {
		// activate instance
		if (Instance == null) {
			Instance = this;

			InitManagers ();
		} else if (Instance != this) {
			Destroy (gameObject);
		}
	}

	void InitManagers() {
		if (!menuManager) {
			menuManager = UI_Tank.Instance;
		}
		if (!soundManager) {
			soundManager = BaseSoundController.Instance;
		}
	}

	public override void StartGame()
	{
		Invoke ("StartPlayer",1);
		
		SpawnController.Instance.Restart();
		
		numberOfPlayers= playerPrefabList.Length;
		
		// initialize some temporary arrays we can use to set up the players
        Vector3 [] playerStarts = new Vector3 [numberOfPlayers];
        Quaternion [] playerRotations = new Quaternion [numberOfPlayers];

        // we are going to use the array full of start positions that must be set in the editor, which means we always need to
        // make sure that there are enough start positions for the number of players

        for ( int i = 0; i < numberOfPlayers; i++ )
        {
            // grab position and rotation values from start position transforms set in the inspector
            playerStarts [i] = (Vector3) startPoints [i].position;
            playerRotations [i] = ( Quaternion ) startPoints [i].rotation;
        }
		
        SpawnController.Instance.SetUpPlayers( playerPrefabList, playerStarts, playerRotations, playerParent, numberOfPlayers );
		
		playerTransforms=new ArrayList();
		
		// now let's grab references to each player's controller script
		playerTransforms = SpawnController.Instance.GetAllSpawnedPlayers();
		
		playerList=new ArrayList();
		
		for ( int i = 0; i < numberOfPlayers; i++ )
        {
			Transform tempT= (Transform)playerTransforms[i];
			PlayerTopDown_Tank tempController= tempT.GetComponent<PlayerTopDown_Tank>();
			playerList.Add(tempController);
			tempController.Init ();
		}
		
        // grab a ref to the player's gameobject for later
        playerGO1 = SpawnController.Instance.GetPlayerGO( 0 );

        // grab a reference to the focussed player's car controller script, so that we can
        // do things like access its speed variable
        thePlayerScript = ( PlayerTopDown_Tank ) playerGO1.GetComponent<PlayerTopDown_Tank>();

        // assign this player the id of 0
        thePlayerScript.SetID( 0 );

        // set player control
        thePlayerScript.SetUserInput( true );

        // as this is the user, we want to focus on this for UI etc.
        focusPlayerScript = thePlayerScript;

		// see if we have a camera target object to look at
		Transform aTarget= playerGO1.transform.Find("CamTarget");

		if(aTarget!=null)
		{
			// if we have a camera target to aim for, instead of the main player, we use that instead
			Camera.main.SendMessage("SetTarget", aTarget );
		} else {
        	// tell the camera script to target the player
			Camera.main.SendMessage("SetTarget", playerGO1.transform );
		}

		// finally, tell the radar about the new player
		radarManager.SetCenterObject( playerGO1.transform );
	}
	
	void StartPlayer()
	{
		// grab a reference to the main player's data manager so we can update its values later on (scoring, lives etc.)
		mainPlayerDataManager1= playerGO1.GetComponent<PlayerManager_Tank>().DataManager_Tank;
		
		// all ready to play, let's go!
		thePlayerScript.GameStart();
	}
	
	public override void EnemyDestroyed ( Vector3 aPosition, int pointsValue, int hitByID )
	{
		// tell our sound controller to play an explosion sound
		BaseSoundController.Instance.PlaySoundByIndex( 1, aPosition );
		
		// tell main data manager to add score
		mainPlayerDataManager1.AddScore( pointsValue );
			
		// update the score on the UI
		UpdateScoreP1( mainPlayerDataManager1.GetScore() );
		
		// play an explosion effect at the enemy position
		Explode ( aPosition );
		
		// tell spawn controller that we're one enemy closer to the next wave
		spawnManager.DecriseCountSpawnedObject ();
		spawnManager.SpawnRandomObjectWithLimit ();
	}
	
	public void PlayerHit(Transform whichPlayer)
	{
		// tell our sound controller to play an explosion sound
		BaseSoundController.Instance.PlaySoundByIndex( 2, whichPlayer.position );
		
		// call the explosion function!
		Explode( whichPlayer.position );
	}
	
	public void AddEnemyToRadar( Transform aTransform )
	{
		radarManager.AddEnemyBlipToList( aTransform );
	}
	
	public void RemoveEnemyFromRadar( Transform aTransform )
	{
		radarManager.RemoveEnemyBlip( aTransform );
	}
	
	public PlayerTopDown_Tank GetMainPlayerScript ()
	{
		return focusPlayerScript;
	}
	
	public Transform GetMainPlayerTransform ()
	{
		return playerGO1.transform;
	}
	
	public GameObject GetMainPlayerGO ()
	{
		return playerGO1;
	}
	
	public void PlayerDied(int whichID)
	{
		// this is a single player game, so just end the game now
		// both players are dead, so end the game
		menuManager.ShowGameOver();
		Invoke ("Exit",5);
	}
	
	void Exit()
	{
		Application.LoadLevel( mainMenuSceneName );
	}
	
	// UI update calls
	// 
	public void UpdateScoreP1( int aScore )
	{
		menuManager.UpdateScoreP1( aScore );
	} 
	
	public void UpdateLivesP1( int aLives )
	{
		menuManager.UpdateLivesP1( aLives );
	}

	public void UpdateLivesDetaleP1( float aLivesDetale )
	{
		menuManager.UpdateLivesDetale( aLivesDetale );
	}

}
