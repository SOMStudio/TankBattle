using UnityEngine;
using System.Collections;
 
public class RadarGUI : MonoBehaviour 
{
	private Transform centerObject;
	
	[SerializeField]
	private Texture enemyBlipTexture;
	[SerializeField]
	private Texture radarBackgroundTexture;
 
	private Vector2 drawCenterPosition;
	[SerializeField]
	private Vector2 drawOffset= new Vector2(5,5);
	
	[SerializeField]
	private Vector2 drawBlipOffset;
	
	[SerializeField]
	private string defaultTagFilter= "enemy";
	[SerializeField]
	private float mapScale= 0.3f;
	[SerializeField]
	private float maxDist= 200;
	[SerializeField]
	private float mapWidth= 256;
	[SerializeField]
	private float mapHeight= 256;
	
	[SerializeField]
	private bool rotateAroundPlayer;
	
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
	
	public enum Positioning {
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight,
	}
	
	[SerializeField]
	private Positioning drawPosition;

	// main event
	void Start()
	{
		SetUpRadar();
	}

	void OnGUI() 
	{
		// draw the radar
		DrawRadar();
	}

	// main logic 
	public void SetCenterObject( Transform aTransform )
	{
		centerObject = aTransform;
	}

	public void AddEnemyBlipToList( Transform transformToAdd )
	{
		// add transform and textures to arraylists
		radarList.Add ( transformToAdd );

		// for this, we will assume that we are adding an enemy and use the enemy blip
		textureList.Add ( enemyBlipTexture );
	}

	public void RemoveEnemyBlip( Transform transformToRemove )
	{
		radarList.Remove( transformToRemove );	
	}

	private void SetUpRadar()
	{
		// set up arraylists to hold transforms and textures
		radarList= new ArrayList();
		textureList= new ArrayList();

		// Find all game objects with default tag on
		GameObject[] gos = GameObject.FindGameObjectsWithTag(defaultTagFilter); 

		// Iterate through them
		foreach (GameObject go in gos)  
		{
			AddBlipToList(go.transform, enemyBlipTexture);
		}
	}

	private void AddBlipToList( Transform transformToAdd, Texture aBlip )
	{
		// add transform and textures to arraylists
		radarList.Add ( transformToAdd );
		textureList.Add ( aBlip );
	}
	
	private void DrawRadar()
	{
		// calculate center position
		CalcCenter();
		
		// draw our radar background
	 	GUI.DrawTexture( new Rect( drawCenterPosition.x - ( mapWidth / 2 ) , drawCenterPosition.y - ( mapHeight / 2 ), mapWidth, mapHeight ), radarBackgroundTexture );
		
		// now iterate through the radarList to draw each blip
		for(int i=0; i<radarList.Count; i++)
		{			
			// draw its blip on the radar
			drawBlip( ( Transform ) radarList[i], ( Texture ) textureList[i] );
		}
	}
		
	private void drawBlip ( Transform go, Texture aTexture )
	{
		// if this is null, we need to do another scan for blips
		if(go==null)
			SetUpRadar();
		
		try
		{
			
			centerPos= centerObject.position;
			extPos= go.position;
		} catch {
			return;	
		}
		
		// first we need to get the distance of the enemy from the player
		dist= Vector3.Distance( centerPos, extPos );
 
		dx= centerPos.x - extPos.x; // how far to the side of the player is the enemy?
		dz= centerPos.z - extPos.z; // how far in front or behind the player is the enemy?
 
		if(rotateAroundPlayer)
		{
			// what's the angle to turn to face the enemy - compensating for the player's turning?
			deltay= Mathf.Atan2( dx, dz ) * Mathf.Rad2Deg - 270 - centerObject.eulerAngles.y;
		} else {
			// what's the angle to turn to face the enemy - compensating for the player's turning?
			deltay= Mathf.Atan2( dx, dz ) * Mathf.Rad2Deg -270;
		}
		// just basic trigonometry to find the point x,y (enemy's location) given the angle deltay
		bX= dist * Mathf.Cos( deltay * Mathf.Deg2Rad );
		bY= dist * Mathf.Sin( deltay * Mathf.Deg2Rad );
 
		bX= bX * mapScale; // scales down the x-coordinate by half so that the plot stays within our radar
		bY= bY * mapScale; // scales down the y-coordinate by half so that the plot stays within our radar
 
		if( dist<= maxDist )
		{ 
			// draw the blip
		   GUI.DrawTexture( new Rect( drawCenterPosition.x + bX + drawBlipOffset.x, drawCenterPosition.y + bY + drawBlipOffset.y, aTexture.width, aTexture.height ), aTexture );
		}
	}
 
	private void CalcCenter()
	{
		switch( drawPosition )
		{
		case Positioning.TopLeft:
			// top left
			drawCenterPosition.x= drawOffset.x + ( mapWidth / 2 );
			drawCenterPosition.y= drawOffset.y + ( mapHeight / 2 );
			break;
			
		case Positioning.TopRight:
			// top right
			drawCenterPosition.x= Screen.width - drawOffset.x - ( mapWidth / 2 );
			drawCenterPosition.y= drawOffset.y + ( mapHeight / 2 );
			break;
			
		case Positioning.BottomLeft:
			// bottom left
			drawCenterPosition.x= drawOffset.x + ( mapWidth / 2 );
			drawCenterPosition.y= Screen.height - ( drawOffset.y + ( mapHeight / 2 ) );
			break;
			
		case Positioning.BottomRight:
			// bottom right
			drawCenterPosition.x= Screen.width - drawOffset.x - ( mapWidth / 2 );
			drawCenterPosition.y= Screen.height - ( drawOffset.y + ( mapHeight / 2 ) );
			break;
		}
	}
	
}