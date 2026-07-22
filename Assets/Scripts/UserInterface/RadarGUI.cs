using UnityEngine;
using System.Collections;

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

	private ArrayList radarList;
	private ArrayList textureList;

	private Transform tempTRANS;
	private Texture tempTEXTURE;

	private float dist;
	private float dx;
	private float dz;
	private float deltay;
	private float bX;
	private float bY;
	private Vector3 centerPos;
	private Vector3 extPos;

	public enum Positioning
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight,
	}

	[SerializeField] private Positioning drawPosition;

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

		GameObject[] gos = GameObject.FindGameObjectsWithTag(defaultTagFilter);

		foreach (GameObject go in gos)
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
			drawBlip((Transform)radarList[i], (Texture)textureList[i]);
		}
	}

	private void drawBlip(Transform go, Texture aTexture)
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

		dx = centerPos.x - extPos.x;
		dz = centerPos.z - extPos.z;

		if (rotateAroundPlayer)
		{
			deltay = Mathf.Atan2(dx, dz) * Mathf.Rad2Deg - 270 - centerObject.eulerAngles.y;
		}
		else
		{
			deltay = Mathf.Atan2(dx, dz) * Mathf.Rad2Deg - 270;
		}

		bX = dist * Mathf.Cos(deltay * Mathf.Deg2Rad);
		bY = dist * Mathf.Sin(deltay * Mathf.Deg2Rad);

		bX = bX * mapScale;
		bY = bY * mapScale;

		if (dist <= maxDist)
		{
			GUI.DrawTexture(
				new Rect(drawCenterPosition.x + bX + drawBlipOffset.x, drawCenterPosition.y + bY + drawBlipOffset.y,
					aTexture.width, aTexture.height), aTexture);
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