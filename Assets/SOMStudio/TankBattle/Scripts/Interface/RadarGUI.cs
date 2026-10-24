using System.Collections;
using UnityEngine;

namespace SOMStudio.TankBattle.Scripts.Interface
{
	public class RadarGUI : MonoBehaviour
	{
		private Transform centerObject;

		[SerializeField] private Texture enemyBlipTexture;
		[SerializeField] private Texture radarBackgroundTexture;

		private Vector2 drawCenterPosition;
	
		[SerializeField] private Vector2 drawOffset = new Vector2(5, 5);
		[SerializeField] private Vector2 drawBlipOffset;
		[SerializeField] private string defaultTagFilter = "enemy";
		[SerializeField] private float mapScale = 0.3f;
		[SerializeField] private float maxDist = 200;
		[SerializeField] private float mapWidth = 256;
		[SerializeField] private float mapHeight = 256;
		[SerializeField] private bool rotateAroundPlayer;
		[SerializeField] private Positioning drawPosition;

		private ArrayList radarList;
		private ArrayList textureList;

		private float dist;
		private float distX;
		private float distZ;
		private float deltaY;
		private float baseX;
		private float baseY;
		private Vector3 centerPos;
		private Vector3 extPos;

		public enum Positioning
		{
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight,
		}

		private void Start()
		{
			SetUpRadar();
		}

		private void OnGUI()
		{
			DrawRadar();
		}

		public void SetCenterObject(Transform aTransform)
		{
			centerObject = aTransform;
		}

		public void AddEnemyBlipToList(Transform transformToAdd)
		{
			radarList.Add(transformToAdd);

			textureList.Add(enemyBlipTexture);
		}

		public void RemoveEnemyBlip(Transform transformToRemove)
		{
			radarList.Remove(transformToRemove);
		}

		private void SetUpRadar()
		{
			radarList = new ArrayList();
			textureList = new ArrayList();

			GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(defaultTagFilter);

			foreach (GameObject go in gameObjects)
			{
				AddBlipToList(go.transform, enemyBlipTexture);
			}
		}

		private void AddBlipToList(Transform transformToAdd, Texture aBlip)
		{
			radarList.Add(transformToAdd);
			textureList.Add(aBlip);
		}

		private void DrawRadar()
		{
			CalcCenter();

			GUI.DrawTexture(
				new Rect(drawCenterPosition.x - (mapWidth / 2), drawCenterPosition.y - (mapHeight / 2), mapWidth,
					mapHeight), radarBackgroundTexture);

			for (int i = 0; i < radarList.Count; i++)
			{
				DrawBlip((Transform)radarList[i], (Texture)textureList[i]);
			}
		}

		private void DrawBlip(Transform go, Texture texture)
		{
			if (go == null) SetUpRadar();

			try
			{
				centerPos = centerObject.position;
				extPos = go.position;
			}
			catch
			{
				return;
			}

			dist = Vector3.Distance(centerPos, extPos);

			distX = centerPos.x - extPos.x;
			distZ = centerPos.z - extPos.z;

			if (rotateAroundPlayer)
			{
				deltaY = Mathf.Atan2(distX, distZ) * Mathf.Rad2Deg - 270 - centerObject.eulerAngles.y;
			}
			else
			{
				deltaY = Mathf.Atan2(distX, distZ) * Mathf.Rad2Deg - 270;
			}

			baseX = dist * Mathf.Cos(deltaY * Mathf.Deg2Rad);
			baseY = dist * Mathf.Sin(deltaY * Mathf.Deg2Rad);

			baseX = baseX * mapScale;
			baseY = baseY * mapScale;

			if (dist <= maxDist)
			{
				GUI.DrawTexture(
					new Rect(drawCenterPosition.x + baseX + drawBlipOffset.x, drawCenterPosition.y + baseY + drawBlipOffset.y,
						texture.width, texture.height), texture);
			}
		}

		private void CalcCenter()
		{
			switch (drawPosition)
			{
				case Positioning.TopLeft:
					drawCenterPosition.x = drawOffset.x + (mapWidth / 2);
					drawCenterPosition.y = drawOffset.y + (mapHeight / 2);
					break;
				case Positioning.TopRight:
					drawCenterPosition.x = Screen.width - drawOffset.x - (mapWidth / 2);
					drawCenterPosition.y = drawOffset.y + (mapHeight / 2);
					break;
				case Positioning.BottomLeft:
					drawCenterPosition.x = drawOffset.x + (mapWidth / 2);
					drawCenterPosition.y = Screen.height - (drawOffset.y + (mapHeight / 2));
					break;
				case Positioning.BottomRight:
					drawCenterPosition.x = Screen.width - drawOffset.x - (mapWidth / 2);
					drawCenterPosition.y = Screen.height - (drawOffset.y + (mapHeight / 2));
					break;
			}
		}
	}
}