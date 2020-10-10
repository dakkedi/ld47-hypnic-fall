using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField]
	private float horizontalMoveSpeed = 5f;
	[SerializeField]
	private bool snappyMovement = false;
	[SerializeField]
	private float maxFallingVelocity = -10f;

	private Vector2 movementInput;
	private bool started;

	public Rigidbody2D RB { get; private set; }
	public float MaxFallingVelocity
	{
		get { return maxFallingVelocity; }
	}

	private void Start()
	{
		RB = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		if (GameManager.instance.PlayerFinished) return;

		if (!started && Input.GetButtonDown("Horizontal"))
		{
			SetStarted(true);
		}
		else if (!started)
		{
			// Player not yet moved
			return;
		}
		movementInput = GetMovementInput();
		CheckPlayerFallingVelocity();
	}

	private void FixedUpdate()
	{
		HandlePlayerHorizontal();
	}

	private void CheckPlayerFallingVelocity()
	{
		if (RB.velocity.y < maxFallingVelocity)
		{
			RB.velocity = new Vector2(RB.velocity.x, maxFallingVelocity);
		}
	}

	private void HandlePlayerHorizontal()
	{
		RB.velocity = new Vector2(movementInput.x * horizontalMoveSpeed, RB.velocity.y);
		RB.position = new Vector2(Mathf.Clamp(RB.position.x, -4.2f, 4.2f), RB.position.y);
	}

	private Vector2 GetMovementInput()
	{
		return snappyMovement ?
			new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) :
			new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
	}

	public void SetStarted(bool value)
	{
		started = value;
	}
}
