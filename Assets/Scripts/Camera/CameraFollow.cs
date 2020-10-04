using UnityEngine;
using UnityEngine.Assertions;

public class CameraFollow : MonoBehaviour
{
	[SerializeField]
	private Transform cameraTarget = null;
	[SerializeField]
	private float cameraSmoothing = 5f;
	[SerializeField]
	private float cameraSizeSmoothing = 10f;
	[SerializeField]
	private float cameraAddedOffsetY = 2.5f;
	[SerializeField]
	private bool clampCameraValues = false;

	/// <summary>
	/// Makes sure the scene is always infront of the camera.
	/// </summary>
	private float cameraOffsetZ = -10;

	private void Awake()
	{
		Assert.IsNotNull(cameraTarget);
	}

	private void Update()
	{
		if (GameManager.instance.PlayerFinished) return;
		FollowTarget();
		LerpCameraSize();
	}

	private void LerpCameraSize()
	{
		var newSize = Mathf.Clamp(Mathf.Abs(GameManager.instance.PlayerMovement.RB.velocity.y), 5f, Mathf.Abs(GameManager.instance.PlayerMovement.MaxFallingVelocity));
		var smoothing = newSize > Camera.main.orthographicSize ? cameraSizeSmoothing : cameraSmoothing * 2; // have a faster smoothing if camera zooms in
		Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, newSize, smoothing * Time.deltaTime);
	}

	private void FollowTarget()
	{
		if (clampCameraValues)
		{
			// add y value to offset the camera to the player
			var targetOffsetPosition = new Vector2(cameraTarget.position.x, cameraTarget.position.y - cameraAddedOffsetY);
			// create transition vector to smoothly follow the player
			Vector2 lerpVector2 = Vector2.Lerp(transform.position, targetOffsetPosition, cameraSmoothing * Time.deltaTime);

			lerpVector2.x = ClampHorizontalValue(lerpVector2.x);

			// set lerp vector to new position
			transform.position = new Vector3(lerpVector2.x, lerpVector2.y, cameraOffsetZ);
		}
		else
		{
			transform.position = new Vector3(0, cameraTarget.position.y - cameraAddedOffsetY, cameraOffsetZ);
		}
	}

	private float ClampHorizontalValue(float lerpX)
	{
		return Mathf.Clamp(lerpX, -1.5f, 1.5f);
	}
}