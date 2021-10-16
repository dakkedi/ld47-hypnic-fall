using UnityEngine;
using UnityEngine.Assertions;

public class CameraFollow : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Target for camera, usually the player")]
	private Transform cameraTarget = null;
	[SerializeField]
	[Tooltip("Value for smooth player follow")]
	private float cameraFollowSmoothing = 5f;
	[SerializeField]
	[Tooltip("Value for smooth camera zoom")]
	private float cameraSizeSmoothingIn = 2f;
	[SerializeField]
	[Tooltip("Smooth camera zoom out factor")]
	private float cameraSizeSmoothingOut = 6f;
	[SerializeField]
	[Tooltip("Camera offset value in Y (Up and down)")]
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

	private void FollowTarget()
	{
		// If clamp is true, camera will follow horizontal a little.
		// Does not look pretty when player is boosting
		if (clampCameraValues)
		{
			// add y value to offset the camera to the player
			var targetOffsetPosition = new Vector2(cameraTarget.position.x, cameraTarget.position.y - cameraAddedOffsetY);
			// create transition vector to smoothly follow the player
			Vector2 lerpVector2 = Vector2.Lerp(transform.position, targetOffsetPosition, cameraFollowSmoothing * Time.deltaTime);

			lerpVector2.x = ClampHorizontalValue(lerpVector2.x);

			// set lerp vector to new position
			transform.position = new Vector3(lerpVector2.x, lerpVector2.y, cameraOffsetZ);
		}
		else
		{
			// will strictly follow the target/player, funtional but not pretty. Work towards a better solution to the code above.
			transform.position = new Vector3(0, cameraTarget.position.y - cameraAddedOffsetY, cameraOffsetZ);
		}
	}

	/// <summary>
	/// Zooms the camera in and out smoothly
	/// </summary>
	private void LerpCameraSize()
	{
		var newSize = Mathf.Clamp(Mathf.Abs(GameManager.instance.PlayerMovement.RB.velocity.y), 5f, Mathf.Abs(GameManager.instance.PlayerMovement.MaxFallingVelocity));
		var smoothing = newSize > Camera.main.orthographicSize ? cameraSizeSmoothingIn : cameraSizeSmoothingOut; // have a faster smoothing if camera zooms in
		Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, newSize, smoothing * Time.deltaTime);
	}

	private float ClampHorizontalValue(float lerpX)
	{
		return Mathf.Clamp(lerpX, -1.5f, 1.5f);
	}
}