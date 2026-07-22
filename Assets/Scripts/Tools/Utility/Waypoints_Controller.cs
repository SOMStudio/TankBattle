using UnityEngine;

[AddComponentMenu("Utility/Waypoints Controller")]
public class Waypoints_Controller : MonoBehaviour
{
	[SerializeField] private Transform[] transforms;
	[SerializeField] private bool closed = true;
	[SerializeField] private bool shouldReverse = false;

	private Vector3 firstPoint;
	private float distance;
	private Transform TEMPtrans;
	private int TEMPindex;
	private int totalTransforms;

	private Vector3 diff;
	private float curDistance;
	private Transform closest;

	private Vector3 currentPos;
	private Vector3 lastPos;
	private Transform pointT;

	private void Start()
	{
		GetTransforms();
	}

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
			return;

		GetTransforms();

		if (totalTransforms < 2)
			return;

		TEMPtrans = (Transform)transforms[0];
		lastPos = TEMPtrans.position;

		pointT = (Transform)transforms[0];

		firstPoint = lastPos;

		for (int i = 1; i < totalTransforms; i++)
		{
			TEMPtrans = (Transform)transforms[i];
			if (TEMPtrans == null)
			{
				GetTransforms();
				return;
			}

			currentPos = TEMPtrans.position;

			Gizmos.color = Color.green;
			Gizmos.DrawSphere(currentPos, 2);

			Gizmos.color = Color.red;
			Gizmos.DrawLine(lastPos, currentPos);

			pointT.LookAt(currentPos);

			lastPos = currentPos;

			pointT = (Transform)transforms[i];
		}

		if (closed)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(currentPos, firstPoint);
		}
	}

	public bool Closed
	{
		get => closed;
		set => closed = value;
	}

	public bool ShouldReverse
	{
		get => shouldReverse;
		set => shouldReverse = value;
	}

	private void GetTransforms()
	{
		totalTransforms = transforms.Length;
	}

	public void SetReverseMode(bool rev)
	{
		shouldReverse = rev;
	}

	public int FindNearestWaypoint(Vector3 fromPos, float maxRange)
	{
		if (transforms == null)
			GetTransforms();

		if (totalTransforms == 0)
			GetTransforms();

		distance = Mathf.Infinity;

		for (int i = 0; i < transforms.Length; i++)
		{
			TEMPtrans = (Transform)transforms[i];

			diff = (TEMPtrans.position - fromPos);
			curDistance = diff.sqrMagnitude;

			if (curDistance < distance)
			{
				if (Mathf.Abs(TEMPtrans.position.y - fromPos.y) < maxRange)
				{
					closest = TEMPtrans;

					TEMPindex = i;

					distance = curDistance;
				}
			}
		}

		if (closest)
		{
			return TEMPindex;
		}
		else
		{
			return -1;
		}
	}

	public int FindNearestWaypoint(Vector3 fromPos, Transform exceptThis, float maxRange)
	{
		if (transforms == null)
			GetTransforms();

		if (totalTransforms == 0)
			GetTransforms();

		distance = Mathf.Infinity;

		for (int i = 0; i < totalTransforms; i++)
		{
			TEMPtrans = (Transform)transforms[i];

			diff = (TEMPtrans.position - fromPos);
			curDistance = diff.sqrMagnitude;

			if (curDistance < distance && TEMPtrans != exceptThis)
			{
				if (Mathf.Abs(TEMPtrans.position.y - fromPos.y) < maxRange)
				{
					closest = TEMPtrans;

					TEMPindex = i;

					distance = curDistance;
				}
			}
		}

		if (closest)
		{
			return TEMPindex;
		}
		else
		{
			return -1;
		}
	}

	public Transform GetWaypoint(int index)
	{
		if (totalTransforms == 0)
			GetTransforms();

		if (shouldReverse)
		{
			index = (totalTransforms - 1) - index;

			if (index < 0)
				index = 0;
		}

		if (transforms == null)
			GetTransforms();

		if (index > totalTransforms - 1)
			return null;

		return transforms[index];
	}

	public int GetTotal()
	{
		if (totalTransforms == 0)
			GetTransforms();

		return totalTransforms;
	}
}
