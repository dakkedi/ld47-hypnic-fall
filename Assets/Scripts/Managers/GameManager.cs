using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;

	
	[SerializeField]
	private GameObject timeBoostPrefab;
	[SerializeField]
	private GameObject groundPrefab;
	[SerializeField]
	private GameObject levelPrefab;
	
	public GameObject Player { get; private set; }
	public Transform PlayerSpawn { get; private set; }
	public PlayerMovement PlayerMovement { get; private set; }

	private GameObject level = null;
	private PlayerBehavior playerBehavior;
	

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(instance);

		Assert.IsNotNull(timeBoostPrefab);
		Assert.IsNotNull(groundPrefab);
		Assert.IsNotNull(levelPrefab);
	}
	private void Start()
	{
		Player = GameObject.FindGameObjectWithTag(Constants.Player);
		PlayerSpawn = GameObject.FindGameObjectWithTag(Constants.Respawn).GetComponent<Transform>();
		level = GameObject.FindGameObjectWithTag(Constants.Level);
		playerBehavior = Player.GetComponent<PlayerBehavior>();
		PlayerMovement = Player.GetComponent<PlayerMovement>();
	}
	private void Update()
	{
		
	}

	#region Getters
	public GameObject Ground
	{
		get { return groundPrefab; }
	}
	#endregion

	#region Publics
	

	public void PlayerHitGround()
	{
		ResetGame();
	}
	#endregion

	#region Privates

	
	private void ResetGame()
	{
		ResetPlayer();
		ResetLevel();
	}

	private void ResetLevel()
	{
		Destroy(level);
		level = Instantiate(levelPrefab);
	}

	private void ResetPlayer()
	{
		Player.transform.position = PlayerSpawn.position;
		playerBehavior.ResetPlayer();
	}
	#endregion
}
