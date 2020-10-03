using System.Collections;
using System.Collections.Generic;
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
	private float defaultGravity;

	public int CollectedBoost { get; private set; }

	private void Start()
	{
		ground = GameManager.instance.Ground;
		playerHalo = Helper.FindComponentInChildWithTag<Transform>(gameObject, Constants.Halo);
		rb = gameObject.GetComponent<Rigidbody2D>();
		defaultGravity = rb.gravityScale;
		SetHaloSize();
	}

	private void Update()
	{
		spaceDown = Input.GetKeyDown(KeyCode.Space);

		if (spaceDown && CollectedBoost > 0)
		{
			ActivateBoost();
		}
	}

	private void PlayerHitBoost(GameObject boostObject)
	{
		CollectedBoost++;
		SetHaloSize();
	}

	private void PlayerHitObstacle(GameObject obstacle)
	{
		rb.velocity = Vector2.zero;
		var randomVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		rb.AddForce(randomVector * 3, ForceMode2D.Impulse);
		CollectedBoost = CollectedBoost > 1 ? 1 : 0;
		SetHaloSize();
	}

	private void ActivateBoost()
	{
		// 10+(x*10*(x/4))
		// make sure one collected is not as strong.
		var currentBoostPower = CollectedBoost == 1 ? boostPower * 0.5f : boostPower;
		var boost = currentBoostPower + (CollectedBoost * currentBoostPower * (CollectedBoost / boostPowerManipulator));
		var newYPos = transform.position.y + boost;
		transform.position = new Vector2(transform.position.x, newYPos);
		ResetBoost();
		StartCoroutine(CoyoteTime());
	}

	private IEnumerator CoyoteTime()
	{
		rb.gravityScale = 0.05f;
		rb.velocity = Vector2.zero;
		yield return new WaitForSeconds(1f);
		rb.gravityScale = defaultGravity;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == Constants.Ground)
		{
			GameManager.instance.PlayerHitGround();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == Constants.TimeBoost)
		{
			PlayerHitBoost(collision.gameObject);
		}

		if (collision.gameObject.tag == Constants.Obstacle)
		{
			PlayerHitObstacle(collision.gameObject);
		}
	}

	public void ResetPlayer()
	{
		ResetBoost();
		SetHaloSize();
	}

	private void ResetBoost()
	{
		CollectedBoost = 0;
		SetHaloSize();
	}

	private void SetHaloSize()
	{
		var haloSize = CollectedBoost + (haloStartSize * haloStartSizeMultiplier);
		if (haloSize > 8) haloSize = 8;
		playerHalo.localScale = new Vector2(haloSize, haloSize);
	}
}
