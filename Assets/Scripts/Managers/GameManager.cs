using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;

	
	[SerializeField]
	private GameObject timeBoostPrefab = null;
	[SerializeField]
	private GameObject groundPrefab = null;
	[SerializeField]
	private GameObject levelPrefab = null;
	[SerializeField]
	private GameObject endgameCanvas = null;
	[SerializeField]
	private GameObject finishCanvas = null;
	
	public GameObject Player { get; private set; }
	public Transform PlayerSpawn { get; private set; }
	public PlayerMovement PlayerMovement { get; private set; }
	public bool PlayerFinished { get; private set; }

	private GameObject level = null;
	private PlayerBehavior playerBehavior;
	private SpriteRenderer endScreenSprite;
	

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		Assert.IsNotNull(timeBoostPrefab);
		Assert.IsNotNull(groundPrefab);
		Assert.IsNotNull(levelPrefab);
		Assert.IsNotNull(endgameCanvas);
		Assert.IsNotNull(finishCanvas);
	}
	private void Start()
	{
		finishCanvas.gameObject.SetActive(false);
		finishCanvas.GetComponent<CanvasGroup>().alpha = 0f;
		endgameCanvas.gameObject.SetActive(false);
		endgameCanvas.GetComponent<CanvasGroup>().alpha = 0f;
		Player = GameObject.FindGameObjectWithTag(Constants.Player);
		PlayerSpawn = GameObject.FindGameObjectWithTag(Constants.Respawn).GetComponent<Transform>();
		level = GameObject.FindGameObjectWithTag(Constants.Level);
		playerBehavior = Player.GetComponent<PlayerBehavior>();
		PlayerMovement = Player.GetComponent<PlayerMovement>();
		endScreenSprite = Helper.FindComponentInChildWithTag<SpriteRenderer>(Camera.main.gameObject, "EndGame");
		endScreenSprite.gameObject.SetActive(false);
	}
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.R))
		{
			SceneManager.LoadScene("Level");
		}

		if (Input.GetKeyUp(KeyCode.Escape))
		{
			SceneManager.LoadScene("Start");
		}
	}

	public GameObject Ground
	{
		get { return groundPrefab; }
	}

	public void PlayerHitGround()
	{
		StartCoroutine(GameEnd());
	}

	private IEnumerator GameEnd()
	{
		endScreenSprite.color = new Color(endScreenSprite.color.r, endScreenSprite.color.g, endScreenSprite.color.b, 0);
		endScreenSprite.gameObject.SetActive(true);
		Player.SetActive(false);
		while (endScreenSprite.color.a < 1)
		{
			endScreenSprite.color = new Color(endScreenSprite.color.r, endScreenSprite.color.g, endScreenSprite.color.b, endScreenSprite.color.a + .2f);
			yield return new WaitForSeconds(0.1f);
		}
		endgameCanvas.gameObject.SetActive(true);
		var canvasGroup = endgameCanvas.GetComponent<CanvasGroup>();
		StartCoroutine(FadeInFinishCanvas(canvasGroup));
	}

	private void ResetEndScreen()
	{
		endScreenSprite.color = new Color(endScreenSprite.color.r, endScreenSprite.color.g, endScreenSprite.color.b, 0);
		endScreenSprite.gameObject.SetActive(false);
	}

	public IEnumerator InitiatePlayerFinish()
	{
		PlayerFinished = true;
		PlayerMovement.RB.gravityScale = 0;
		PlayerMovement.RB.AddForce(Vector2.up*50f, ForceMode2D.Impulse);
		playerBehavior.DeactivatePlayer();
		yield return new WaitForSeconds(1f);
		Player.SetActive(false);
		finishCanvas.gameObject.SetActive(true);
		var canvasGroup = finishCanvas.GetComponent<CanvasGroup>();
		StartCoroutine(FadeInFinishCanvas(canvasGroup));
	}

	private IEnumerator FadeInFinishCanvas(CanvasGroup group)
	{
		while (group.alpha < 1f)
		{
			group.alpha = group.alpha + 0.1f;
			yield return new WaitForSeconds(0.1f);
		}
	}
}
