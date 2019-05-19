using UnityEngine;

public class SpawnController_Tank : MonoBehaviour {

	[Header("Settings")]
	[SerializeField]
	private float timeBeforeFirstSpawn = 1f;
	[SerializeField]
	private int limitSpawnedObject = 10;

	[Header("Spawn")]
	[SerializeField]
	private GameObject[] spawnObjectPrefabs;
	[SerializeField]
	private GameObject[] spawnObjectPoints;
	[SerializeField]
	private Transform spawnParent;
	[SerializeField]
	private GameObject spawnParticle;

	private int countSpawnedObject = 0;

	[System.NonSerialized]
	public static SpawnController_Tank Instance;

	// main event
	void Awake() {
		// init object
		Init ();
	}

	void Start ()
	{
		// start spawn with delay
		Invoke("SpawnRandomObjectWithLimit", timeBeforeFirstSpawn);
	}

	// main logic
	public void DecriseCountSpawnedObject() {
		countSpawnedObject--;
	}

	public void SpawnRandomObjectWithLimit() {
		if (countSpawnedObject < limitSpawnedObject) {
			SpawnRandomObjectInRadomPlace ();
		}

		if (countSpawnedObject < limitSpawnedObject) {
			Invoke ("SpawnRandomObjectWithLimit", 0.5f);
		}
	}

	void Init() {
		// activate instance
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy (gameObject);
		}
	}

	private void SpawnObjectWithNumberInPoint(int numPrefab, int numPoint) {
		if (numPrefab < spawnObjectPrefabs.Length) {
			if (numPoint < spawnObjectPoints.Length) {
				GameObject objectSp = spawnObjectPrefabs [numPrefab];
				GameObject pointSp = spawnObjectPoints [numPoint];

				if (objectSp != null && pointSp != null) {
					var enemyTr = SpawnController.Instance.Spawn (objectSp, pointSp.transform.position, pointSp.transform.rotation);

					// set parent
					if (spawnParent != null)
						enemyTr.parent = spawnParent;

					// show particle
					if (spawnParticle != null) {
						SpawnController.Instance.Spawn (spawnParticle, pointSp.transform.position, pointSp.transform.rotation);
					}

					countSpawnedObject++;
				}
			}
		}
	}

	private void SpawnRandomObjectInRadomPlace() {
		if (spawnObjectPrefabs.Length > 0) {
			if (spawnObjectPoints.Length > 0) {
				int numObjectSpawn = Random.Range (0, spawnObjectPrefabs.Length);
				int numPointSpawn = Random.Range (0, spawnObjectPoints.Length);

				SpawnObjectWithNumberInPoint (numObjectSpawn, numPointSpawn);
			}
		}
	}
}
