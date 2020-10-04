using UnityEngine;

public class Swirl : MonoBehaviour
{
	[SerializeField]
	private float rotationSpeed;

	private void Start()
	{
		if (rotationSpeed == 0)
		{
			rotationSpeed = Random.Range(1, 10);
		}
	}
	// Update is called once per frame
	void Update()
    {
		transform.Rotate(rotationSpeed * Time.deltaTime, rotationSpeed * Time.deltaTime, rotationSpeed * Time.deltaTime);
    }
}
