using System.Collections;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
	[SerializeField]
	private float boostPower = 10f;
	[SerializeField]
	private float boostPowerManipulator = 4f;
	[SerializeField]
	private float haloStartSize = 1f;
	[SerializeField]
	private float haloStartSizeMultiplier = 1.2f;
	private GameObject ground;
	private bool spaceDown;
	private Transform playerHalo;
	private Rigidbody2D rb;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	private CapsuleCollider2D collider = null;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
	private float defaultGravity;
	private Animator anim;
	private int maxBoostCapacity = 8;
	private AudioSource audio;
	private ParticleSystem particles;
	private float defaultShapeRadius;

	public int CollectedBoost { get; private set; }

	private void Start()
	{
		particles = GetComponentInChildren<ParticleSystem>();
		var shape = particles.shape;
		defaultShapeRadius = shape.radius;
		ground = GameManager.instance.Ground;
		playerHalo = Helper.FindComponentInChildWithTag<Transform>(gameObject, Constants.Halo);
		rb = gameObject.GetComponent<Rigidbody2D>();
		audio = GetComponent<AudioSource>();
		collider = gameObject.GetComponent<CapsuleCollider2D>();
		defaultGravity = rb.gravityScale;
		anim = gameObject.GetComponent<Animator>();
		SetHaloSize();
	}

	private void Update()
	{
		if (GameManager.instance.PlayerFinished) return;

		spaceDown = Input.GetKeyDown(KeyCode.Space);

		if (spaceDown && CollectedBoost > 0)
		{
			StartCoroutine(ActivateBoost());
		}

		CheckAnimation();
	}

	private void CheckAnimation()
	{
		anim.SetFloat("VelocityY", rb.velocity.y);
	}

	private void PlayerHitBoost(GameObject boostObject)
	{
		if (CollectedBoost == maxBoostCapacity) return;
		GameManager.instance.PlayAudioBoostPickup();
		CollectedBoost++;
		SetHaloSize();
	}

	private void PlayerHitObstacle(GameObject obstacle)
	{
		GameManager.instance.PlayAudioDamage();
		rb.velocity = Vector2.zero;
		var randomVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		rb.AddForce(randomVector * 3, ForceMode2D.Impulse);
		CollectedBoost = CollectedBoost > 1 ? 1 : 0;
		ResetBoost();
	}

	/// <summary>
	/// Formula used 10+(x*10*(x/y))
	/// </summary>
	private float CalculateBoost()
	{
		var currentBoostPower = CollectedBoost == 1 ? boostPower * 0.5f : boostPower;
		var boost = currentBoostPower + (CollectedBoost * currentBoostPower * (CollectedBoost / boostPowerManipulator));
		return boost;
	}

	private IEnumerator ActivateBoost()
	{
		// make sure one collected is not as strong.
		var boost = CalculateBoost();
		var newYPos = transform.position.y + boost;

		// Slow player down to a halt
		collider.enabled = false;
		rb.gravityScale = 0;
		GameManager.instance.PlayAudioImpulse();
		while (rb.velocity.y < -0.005)
		{
			rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, .5f);
			yield return new WaitForSeconds(0.05f);
		}

		ResetBoost();
		// thrust player upwards
		rb.AddForce(Vector2.up * 50, ForceMode2D.Impulse);
		audio.Play();
		while (transform.position.y < newYPos)
		{
			// do nothing? 
			yield return new WaitForSeconds(0.025f);
		}
		audio.Stop();
		rb.velocity = Vector2.zero;

		// activate collision
		collider.enabled = true;
		GameManager.instance.PlayAudioPlayerStop();

		yield return new WaitForSeconds(1f);
		rb.gravityScale = defaultGravity;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == Constants.Ground)
		{
			GameManager.instance.PlayerHitGround();
		}

		if (collision.gameObject.tag == Constants.TimeBoost)
		{
			PlayerHitBoost(collision.gameObject);
		}

		if (collision.gameObject.tag == Constants.Obstacle)
		{
			PlayerHitObstacle(collision.gameObject);
		}

		if (collision.gameObject.tag == Constants.Finish)
		{
			StartCoroutine(GameManager.instance.InitiatePlayerFinish());
		}
	}

	public void DeactivatePlayer()
	{
		collider.enabled = false;
	}

	private void ResetBoost()
	{
		CollectedBoost = 0;
		SetHaloSize();
	}

	private void SetHaloSize()
	{
		var haloSize = CollectedBoost + (haloStartSize * haloStartSizeMultiplier);
		if (haloSize > maxBoostCapacity) haloSize = maxBoostCapacity;
		playerHalo.localScale = new Vector2(haloSize, haloSize);
		var emission = particles.emission;
		emission.rateOverTime = 10 + (CollectedBoost * 3);
		var shape = particles.shape;
		shape.radius = defaultShapeRadius * CollectedBoost;

		// update ui
		UIManager.Instance.SetCurrentBoost(CalculateBoost());
	}
}
