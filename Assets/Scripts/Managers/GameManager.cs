using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;

	[SerializeField] private PlayerMovement _playerMovement;
	[SerializeField] private PlayerBehavior _playerBehavior;
	[SerializeField] private CameraHandler _cameraHandler;
	[SerializeField] private Text _timeCanvasText;
	[SerializeField] private Text _gameTimer;
	[SerializeField] private AudioSource _audio;
	[SerializeField] private SpriteRenderer _endGameScreenSprite;

	[Header("Game state screens")]
	[SerializeField] private GameObject endgameCanvas = null;
	[SerializeField] private GameObject finishCanvas = null;

	[Header("Audio clips")]
	[SerializeField] private AudioClip damagedSfx;
	[SerializeField] private AudioClip boostPickupSfx;
	[SerializeField] private AudioClip impulseSfx;
	[SerializeField] private AudioClip stoppingSfx;
	[SerializeField] private AudioClip playerStopSfx;


	public GameObject Player { get; private set; }
	public Rigidbody2D PlayerRigidBody { get; private set; }
	public bool PlayerFinished { get; private set; }

	// Private
	private float _timeSpentOnRun;

	/// <summary>
	/// Checks if this class already exists and if the properties are set from the inspector.
	/// </summary>
	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		Assert.IsNotNull(_playerMovement);
		Assert.IsNotNull(_playerBehavior);
		Assert.IsNotNull(_cameraHandler);
		Assert.IsNotNull(_timeCanvasText);
		Assert.IsNotNull(_gameTimer);
		Assert.IsNotNull(_audio);
		Assert.IsNotNull(_endGameScreenSprite);

		Assert.IsNotNull(endgameCanvas);
		Assert.IsNotNull(finishCanvas);

		Assert.IsNotNull(damagedSfx);
		Assert.IsNotNull(boostPickupSfx);
		Assert.IsNotNull(impulseSfx);
		Assert.IsNotNull(stoppingSfx);
		Assert.IsNotNull(playerStopSfx);
	}

	private void Start()
	{
		// Hide endgame and finish screens from start
		HideCanvasScreen(finishCanvas);
		HideCanvasScreen(endgameCanvas);

		// Setting publics
		Player = _playerMovement.gameObject;
		PlayerRigidBody = Player.GetComponent<Rigidbody2D>();

		// this need to be set manually at start
		_endGameScreenSprite.gameObject.SetActive(false);
	}
	private void Update()
	{
		// Use R to reload the level scene
		if (Input.GetKeyUp(KeyCode.R))
		{
			SceneManager.LoadScene("Level");
		}

		// Use esc to load start menu scene
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			SceneManager.LoadScene("Start");
		}

		UpdateGameTimer();
		UpdateCameraSize();
	}

	public void UpdateCameraSize()
	{
		// falling between 0 - -10, boosting should go to max size set (10)
		var playerVelocity = Mathf.Abs(_playerMovement.PlayerRigidBody.velocity.y);
		//var newCameraSize = (playerVelocity / 3) + _cameraHandler.InitialSize;
		var currentSize = _cameraHandler.VirtualCamera.m_Lens.OrthographicSize;

		float cameraSizeSmoothingIn = 2f;
		float cameraSizeSmoothingOut = 6f;
		var newSize = Mathf.Clamp(playerVelocity, 7f, Mathf.Abs(_playerMovement.MaxFallingVelocity));
		var smoothing = newSize > currentSize ? cameraSizeSmoothingIn : cameraSizeSmoothingOut; // have a faster smoothing if camera zooms in
		_cameraHandler.VirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(currentSize, newSize, smoothing * Time.deltaTime);
	}

	public void UpdateGameTimer()
	{
		_gameTimer.text = _playerMovement.TimeSpent.ToString("0.0");
	}

	public float PlayerMaxFallingVelocity => _playerMovement.MaxFallingVelocity;

	/// <summary>
	/// Player hit the ground, initiate end game. (Game over)
	/// </summary>
	public void PlayerHitGround()
	{
		PlayAudioStopping();
		StartCoroutine(GameOver());
	}

	/// <summary>
	/// Initiate end game animation and screen.
	/// </summary>
	/// <returns></returns>
	private IEnumerator GameOver()
	{
		// Sets end game sprite to active and alpha 0
		_endGameScreenSprite.color = new Color(_endGameScreenSprite.color.r, _endGameScreenSprite.color.g, _endGameScreenSprite.color.b, 0);
		_endGameScreenSprite.gameObject.SetActive(true);

		// Player died
		SetPlayerActive(false);

		// fade in end game sprite
		while (_endGameScreenSprite.color.a < 1)
		{
			_endGameScreenSprite.color = new Color(_endGameScreenSprite.color.r, _endGameScreenSprite.color.g, _endGameScreenSprite.color.b, _endGameScreenSprite.color.a + .2f);
			yield return new WaitForSeconds(0.1f);
		}

		// show end game screen
		ActivateAndFadeinCanvasScreen(endgameCanvas);
	}

	/// <summary>
	/// Activates and then fades in the canvas object
	/// </summary>
	/// <param name="canvas"></param>
	private void ActivateAndFadeinCanvasScreen(GameObject canvas)
	{
		// Activate canvas
		canvas.gameObject.SetActive(true);
		var canvasGroup = canvas.GetComponent<CanvasGroup>();
		// Fade in canvas
		StartCoroutine(FadeInCanvas(canvasGroup));
	}

	/// <summary>
	/// Hides and sets alpha to 0 on canvas object
	/// </summary>
	/// <param name="canvas"></param>
	private void HideCanvasScreen(GameObject canvas)
	{
		canvas.gameObject.SetActive(false);
		canvas.GetComponent<CanvasGroup>().alpha = 0f;
	}

	/// <summary>
	/// Sets player movement/animation when player finishes. Activates finish canvas screen.
	/// </summary>
	/// <returns></returns>
	public IEnumerator InitiatePlayerFinish()
	{
		// Stop timer and save it. 
		_timeSpentOnRun = _playerMovement.TimeSpent;
		_timeCanvasText.text = _timeSpentOnRun.ToString();
		_cameraHandler.StopFollowing();

		PlayerFinished = true;
		PlayerRigidBody.gravityScale = 0;
		PlayerRigidBody.AddForce(Vector2.up * 50f, ForceMode2D.Impulse);
		_playerBehavior.DeactivatePlayerCollider();
		yield return new WaitForSeconds(1f);
		SetPlayerActive(false);

		ActivateAndFadeinCanvasScreen(finishCanvas);
	}

	/// <summary>
	/// Sets "SetActive" on player gameobject
	/// </summary>
	/// <param name="state"></param>
	private void SetPlayerActive(bool state)
	{
		Player.SetActive(state);
	}

	/// <summary>
	/// Fades in canvas group
	/// </summary>
	/// <param name="group"></param>
	/// <returns></returns>
	private IEnumerator FadeInCanvas(CanvasGroup group)
	{
		while (group.alpha < 1f)
		{
			group.alpha = group.alpha + 0.1f;
			yield return new WaitForSeconds(0.1f);
		}
	}

	// Audio can be placed in a different class?
	// Can be made into one dynamic method? 
	#region Play audio
	public void PlayAudioDamage()
	{
		_audio.PlayOneShot(damagedSfx);
	}

	public void PlayAudioBoostPickup()
	{
		_audio.PlayOneShot(boostPickupSfx);
	}

	public void PlayAudioImpulse()
	{
		_audio.PlayOneShot(impulseSfx);
	}

	public void PlayAudioStopping()
	{
		_audio.PlayOneShot(stoppingSfx);
	}

	public void PlayAudioPlayerStop()
	{
		_audio.PlayOneShot(playerStopSfx);
	}
	#endregion
}
