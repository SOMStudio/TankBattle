using UnityEngine;

public class AutomaticDestroyObject : MonoBehaviour
{
	[SerializeField] private float timeBeforeObjectDestroys;

	private void Start()
	{
		Invoke(nameof(DestroyGameObject), timeBeforeObjectDestroys);
	}

	private void DestroyGameObject()
	{
		Destroy(gameObject);
	}
}
