using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerBehavior : MonoBehaviour
{
	// Editable fields
	[SerializeField, Range(0.1f, 10f)]
	private float haloStartSize = 1f;
	[SerializeField, Range(1f, 2f)]
	private float haloStartSizeMultiplier = 1.2f;
	[SerializeField]
	private PlayerBoost _playerBoost;
	[SerializeField]
	private Transform _playerHalo;
	[SerializeField]
	private ParticleSystem _playerParticles;

	// Privates
	private Animator _animator;
	private AudioSource _audio;
	private CapsuleCollider2D _collider;
	private float _defaultGravity;
	private float _defaultShapeRadius;
	private Rigidbody2D _rb;
	private bool _playerBusy;

	private int _collectedBoost => _playerBoost.CollectedBoost();
	private float _boostValue => _playerBoost.CalculateBoost();

	private void Awake()
	{
		Assert.IsNotNull(_playerBoost);
		Assert.IsNotNull(_playerHalo);
		Assert.IsNotNull(_playerParticles);
	}
	private void Start()
	{
		InitPrivateValues();
		InitPlayerEffects();
	}

	private void InitPlayerEffects()
	{
		UpdatePlayerEffects();
	}

	private void InitPrivateValues()
	{
		_animator = gameObject.GetComponent<Animator>();
		_audio = gameObject.GetComponent<AudioSource>();
		_collider = gameObject.GetComponent<CapsuleCollider2D>();
		_rb = gameObject.GetComponent<Rigidbody2D>();
		_defaultGravity = _rb.gravityScale;
		_defaultShapeRadius = _playerParticles.shape.radius;
	}

	private void Update()
	{
		if (GameManager.instance.PlayerFinished) return;
		PlayerInput();
		PlayerAnimation();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		CheckGroundCollision(collision);
		CheckObstacleCollision(collision);
		CheckFinishCollision(collision);
	}
	private static void CheckGroundCollision(Collider2D collision)
	{
		if (collision.gameObject.tag == Constants.Ground)
		{
			GameManager.instance.PlayerHitGround();
		}
	}

	private void CheckObstacleCollision(Collider2D collision)
	{
		if (collision.gameObject.tag == Constants.Obstacle)
		{
			PlayerHitObstacle(collision.gameObject);
		}
	}
	private void CheckFinishCollision(Collider2D collision)
	{
		if (collision.gameObject.tag == Constants.Finish)
		{
			StartCoroutine(GameManager.instance.InitiatePlayerFinish());
		}
	}

	private void PlayerInput()
	{
		if (Input.GetKeyDown(KeyCode.Space) && _collectedBoost > 0 && _playerBusy == false)
		{
			ActivateBoost();
		}
	}

	private void PlayerAnimation()
	{
		SetPlayerVelocityAnimation();
	}

	/// <summary>
	/// Sets VelocityY in the animator, used to transition the player to different animations
	/// </summary>
	private void SetPlayerVelocityAnimation()
	{
		_animator.SetFloat(Constants.VelocityY, _rb.velocity.y);
	}

	/// <summary>
	/// Player hits a negative boost
	/// </summary>
	/// <param name="obstacle"></param>
	private void PlayerHitObstacle(GameObject obstacle)
	{
		// TODO obstacle, give some kind of hit state, disable until player passed through
		GameManager.instance.PlayAudioDamage();
		StopPlayerMovement();
		_playerBoost.ResetBoost();
	}

	private void StopPlayerMovement()
	{
		_rb.velocity = Vector2.zero;
	}

	private void ActivateBoost()
	{
		_playerBusy = true;
		StartCoroutine(HaltPlayer());
	}

	private IEnumerator HaltPlayer()
	{
		_collider.enabled = false;
		_rb.gravityScale = 0;
		GameManager.instance.PlayAudioImpulse();
		while (_rb.velocity.y < -0.005)
		{
			_rb.velocity = Vector2.Lerp(_rb.velocity, Vector2.zero, .5f);
			yield return new WaitForSeconds(0.05f);
		}

		yield return ThrustPlayerUpwards();
	}

	private IEnumerator ThrustPlayerUpwards()
	{
		float newPlayerYPosition = transform.position.y + _boostValue;
		_playerBoost.ResetBoost();
		_rb.AddForce(Vector2.up * 50, ForceMode2D.Impulse);
		_audio.Play();
		while (transform.position.y < newPlayerYPosition)
		{
			// do nothing? 
			yield return new WaitForSeconds(0.025f);
		}

		yield return StopAndResetPlayerPhysics();
	}

	private IEnumerator StopAndResetPlayerPhysics()
	{
		_audio.Stop();
		_rb.velocity = Vector2.zero;
		_collider.enabled = true;
		GameManager.instance.PlayAudioPlayerStop();
		yield return new WaitForSeconds(1f);
		_rb.gravityScale = _defaultGravity;
		_playerBusy = false;
	}

	private void UpdateHaloSize()
	{
		var haloSize = _collectedBoost + (haloStartSize * haloStartSizeMultiplier);
		if (haloSize > _playerBoost._maxBoostCapacity) haloSize = _playerBoost._maxBoostCapacity;
		_playerHalo.localScale = new Vector2(haloSize, haloSize);
	}

	private void UpdateParticleSettings()
	{
		var emission = _playerParticles.emission;
		emission.rateOverTime = 10 + (_collectedBoost * 3);
		var shape = _playerParticles.shape;
		shape.radius = _defaultShapeRadius * _collectedBoost;
	}

	/// <summary>
	///  Deactivates the player collider, used when player finishes and flies upwards.
	/// </summary>
	public void DeactivatePlayerCollider()
	{
		_collider.enabled = false;
	}

	/// <summary>
	/// Updates halo and particles depending on current boost collected
	/// </summary>
	public void UpdatePlayerEffects()
	{
		UpdateHaloSize();
		UpdateParticleSettings();
	}
}
