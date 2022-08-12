using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraHandler : MonoBehaviour
{
	[SerializeField] private CinemachineVirtualCamera _virtualCamera;

	// Virtual Cam Body settings
	// Lookahead Time 0.6
	// Lookahead Smoothing 5 

	public CinemachineVirtualCamera VirtualCamera;
	public float InitialSize = 7;

	private void Awake()
	{
		Assert.IsNotNull(_virtualCamera);
	}

	private void Start()
	{
		VirtualCamera = _virtualCamera;
		InitialSize = VirtualCamera.m_Lens.OrthographicSize;
	}

	public void StopFollowing()
	{
		_virtualCamera.enabled = false;
	}

	public void ContinueFollowing()
	{
		_virtualCamera.enabled = true;
	}
}
