using System.Collections;
using UnityEngine;

namespace SOMStudio.TankBattle.Scripts.Tools.Spawn
{
	public class SpawnTool
	{
		private ArrayList playerTransforms;
		private ArrayList playerGameObjects;

		private Transform tempTrans;
		private GameObject tempGameObject;

		private GameObject[] playerPrefabList;
		private Vector3[] startPositions;
		private Quaternion[] startRotations;

		private static SpawnTool instance;

		private SpawnTool()
		{
			if (instance != null)
			{
				Debug.LogWarning("Tried to generate more than one instance of singleton SpawnController.");
				return;
			}

			instance = this;
		}

		public static SpawnTool Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new SpawnTool();
				}

				return instance;
			}
		}

		public void Restart()
		{
			playerTransforms = new ArrayList();
			playerGameObjects = new ArrayList();
		}

		public void SetUpPlayers(GameObject[] playerPrefabs, Vector3[] playerStartPositions,
			Quaternion[] playerStartRotations, Transform theParentObj, int totalPlayers)
		{
			playerPrefabList = playerPrefabs;
			startPositions = playerStartPositions;
			startRotations = playerStartRotations;

			CreatePlayers(theParentObj, totalPlayers);
		}

		public void CreatePlayers(Transform theParent, int totalPlayers)
		{
			playerTransforms = new ArrayList();
			playerGameObjects = new ArrayList();

			for (int i = 0; i < totalPlayers; i++)
			{
				tempTrans = Spawn(playerPrefabList[i], startPositions[i], startRotations[i]);

				if (theParent != null)
				{
					tempTrans.parent = theParent;
					tempTrans.localPosition = startPositions[i];
				}

				playerTransforms.Add(tempTrans);

				playerGameObjects.Add(tempTrans.gameObject);
			}
		}

		public GameObject GetPlayerGameObject(int indexNum)
		{
			return (GameObject)playerGameObjects[indexNum];
		}

		public Transform GetPlayerTransform(int indexNum)
		{
			return GetPlayerGameObject(indexNum).transform;
		}

		public Transform Spawn(GameObject anObject, Vector3 aPosition, Quaternion aRotation)
		{
			tempGameObject = Object.Instantiate(anObject, aPosition, aRotation);
			tempTrans = tempGameObject.transform;

			return tempTrans;
		}

		public GameObject SpawnGameObject(GameObject anObject, Vector3 aPosition, Quaternion aRotation)
		{
			tempGameObject = Object.Instantiate(anObject, aPosition, aRotation);
			tempTrans = tempGameObject.transform;

			return tempGameObject;
		}

		public ArrayList GetAllSpawnedPlayers()
		{
			return playerTransforms;
		}
	}
}
