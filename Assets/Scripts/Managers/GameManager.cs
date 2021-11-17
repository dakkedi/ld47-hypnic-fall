using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;

	[SerializeField]
	private PlayerMovement _playerMovement;
	[SerializeField]
	private CameraHandler _cameraHandler;
	[SerializeField]
	private Text _timeCanvasText;
	[SerializeField]
	private Text _gameTimer;

	[Header("Game state screens")]
	[SerializeField]
	private GameObject endgameCanvas = null;
	[SerializeField]
	private GameObject finishCanvas = null;

	[Header("Audio clips")]
	[SerializeField]
	private AudioClip damagedSfx;
	[SerializeField]
	private AudioClip boostPickupSfx;
	[SerializeField]
	private AudioClip impulseSfx;
	[SerializeField]
	private AudioClip stoppingSfx;
	[SerializeField]
	private AudioClip playerStopSfx;


	// Not visible in the inspector // 

	// Properties reachable by other classes.
	// Public
	public GameObject Player { get; private set; }
	public Rigidbody2D PlayerRigidBody { get; private set; }
	public bool PlayerFinished { get; private set; }

	// Private
	private PlayerBehavior playerBehavior;
	private SpriteRenderer endGameScreenSprite;
	private AudioSource audio;
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

		Assert.IsNotNull(endgameCanvas);
		Assert.IsNotNull(finishCanvas);
		Assert.IsNotNull(damagedSfx);
		Assert.IsNotNull(_playerMovement);
		Assert.IsNotNull(_cameraHandler);
		Assert.IsNotNull(_timeCanvasText);
		Assert.IsNotNull(_gameTimer);
	}

	private void Start()
	{
		// Hide endgame and finish screens from start
		HideCanvasScreen(finishCanvas);
		HideCanvasScreen(endgameCanvas);

		// Setting publics
		Player = GameObject.FindGameObjectWithTag(Constants.Player);
		PlayerRigidBody = Player.GetComponent<Rigidbody2D>();
		
		// Setting privates
		playerBehavior = Player.GetComponent<PlayerBehavior>();
		endGameScreenSprite = Helper.FindComponentInChildWithTag<SpriteRenderer>(Camera.main.gameObject, Constants.EndGame);
		endGameScreenSprite.gameObject.SetActive(false);
		audio = GetComponent<AudioSource>();
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
	}

	public void UpdateGameTimer()
	{
		_gameTimer.text = _playerMovement.TimeSpent.ToString();
	}

	public float PlayerMaxFallingVelocity => _playerMovement.MaxFallingVelocity;

	/// <summary>
	/// Player hit the ground, initiate end game. (Game over)
	/// </summary>
	public void PlayerHitGround()
	{
		PlayAudioStopping();
		StartCoroutine(GameEnd());
	}

	/// <summary>
	/// Initiate end game animation and screen.
	/// </summary>
	/// <returns></returns>
	private IEnumerator GameEnd()
	{
		// Sets end game sprite to active and alpha 0
		endGameScreenSprite.color = new Color(endGameScreenSprite.color.r, endGameScreenSprite.color.g, endGameScreenSprite.color.b, 0);
		endGameScreenSprite.gameObject.SetActive(true);

		// Player died
		SetPlayerActive(false);

		// fade in end game sprite
		while (endGameScreenSprite.color.a < 1)
		{
			endGameScreenSprite.color = new Color(endGameScreenSprite.color.r, endGameScreenSprite.color.g, endGameScreenSprite.color.b, endGameScreenSprite.color.a + .2f);
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
		PlayerRigidBody.AddForce(Vector2.up*50f, ForceMode2D.Impulse);
		playerBehavior.DeactivatePlayerCollider();
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
		audio.PlayOneShot(damagedSfx);
	}

	public void PlayAudioBoostPickup()
	{
		audio.PlayOneShot(boostPickupSfx);
	}

	public void PlayAudioImpulse()
	{
		audio.PlayOneShot(impulseSfx);
	}

	public void PlayAudioStopping()
	{
		audio.PlayOneShot(stoppingSfx);
	}

	public void PlayAudioPlayerStop()
	{
		audio.PlayOneShot(playerStopSfx);
	}
	#endregion
}
