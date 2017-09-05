using UnityEngine;
using System.Collections;

public class SpawnController_Tank : MonoBehaviour {
	public GameObject[] spawnObjectPrefabs;
	public GameObject[] spawnObjectPoints;
	public GameObject spawnParticle;

	public float timeBeforeFirstSpawn=1f;
	public int limitSpawnedObject = 10;

	public int countSpawnedObject = 0;

	public void DecriseCountSpawnedObject() {
		countSpawnedObject--;
	}

	void Start ()
	{
		// start spawn with delay
		Invoke("SpawnRandomObjectWithLimit", timeBeforeFirstSpawn);
	}

	private void SpawnObjectWithNumberInPoint(int numPrefab, int numPoint) {
		if (numPrefab < spawnObjectPrefabs.Length) {
			if (numPoint < spawnObjectPoints.Length) {
				GameObject objectSp = spawnObjectPrefabs [numPrefab];
				GameObject pointSp = spawnObjectPoints [numPoint];

				if (objectSp != null && pointSp != null) {
					SpawnController.Instance.Spawn (objectSp, pointSp.transform.position, pointSp.transform.rotation);

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

	public void SpawnRandomObjectWithLimit() {
		if (countSpawnedObject < limitSpawnedObject) {
			SpawnRandomObjectInRadomPlace ();
		}

		if (countSpawnedObject < limitSpawnedObject) {
			Invoke ("SpawnRandomObjectWithLimit", 0.5f);
		}
	}
}
