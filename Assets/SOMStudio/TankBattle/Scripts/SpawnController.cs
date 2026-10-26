using SOMStudio.TankBattle.Scripts.Tools.Spawn;
using UnityEngine;

namespace SOMStudio.TankBattle.Scripts
{
	[AddComponentMenu("SOMStudio/TankBattle/Spawn Controller")]
	public class SpawnController : MonoBehaviour {

		[Header("Settings")]
		[SerializeField] private float timeBeforeFirstSpawn = 1f;
		[SerializeField] private int limitSpawnedObject = 10;

		[Header("Spawn")]
		[SerializeField] private GameObject[] spawnObjectPrefabs;
		[SerializeField] private GameObject[] spawnObjectPoints;
		[SerializeField] private Transform spawnParent;
		[SerializeField] private GameObject spawnParticle;

		private int countSpawnedObject;

		[System.NonSerialized]
		public static SpawnController Instance;

		private void Awake() {
			Init ();
		}

		private void Start ()
		{
			Invoke(nameof(SpawnRandomObjectWithLimit), timeBeforeFirstSpawn);
		}
	
		public void DecreaseCountSpawnedObject() {
			countSpawnedObject--;
		}

		public void SpawnRandomObjectWithLimit() {
			if (countSpawnedObject < limitSpawnedObject) {
				SpawnRandomObjectInRandomPlace ();
			}

			if (countSpawnedObject < limitSpawnedObject) {
				Invoke (nameof(SpawnRandomObjectWithLimit), 0.5f);
			}
		}

		private void Init() {
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
						var enemyTr = SpawnTool.Instance.Spawn (objectSp, pointSp.transform.position, pointSp.transform.rotation);
					
						if (spawnParent != null)
							enemyTr.parent = spawnParent;
					
						if (spawnParticle != null) {
							SpawnTool.Instance.Spawn (spawnParticle, pointSp.transform.position, pointSp.transform.rotation);
						}

						countSpawnedObject++;
					}
				}
			}
		}

		private void SpawnRandomObjectInRandomPlace() {
			if (spawnObjectPrefabs.Length > 0) {
				if (spawnObjectPoints.Length > 0) {
					int numObjectSpawn = Random.Range (0, spawnObjectPrefabs.Length);
					int numPointSpawn = Random.Range (0, spawnObjectPoints.Length);

					SpawnObjectWithNumberInPoint (numObjectSpawn, numPointSpawn);
				}
			}
		}
	}
}
