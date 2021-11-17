﻿using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	// Editable fields
	[SerializeField, Range(1f, 10f)]
	private float _horizontalMoveSpeed = 5f;
	[SerializeField]
	private float _maxFallingVelocity = -10f;

	// Privates
	private Vector2 _movementInput;
	private bool _started;

	// Publics
	public Rigidbody2D PlayerRigidBody { get; private set; }
	public float MaxFallingVelocity => _maxFallingVelocity;

	private void Start()
	{
		PlayerRigidBody = gameObject.GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		if (GameManager.instance.PlayerFinished) return;
		CapPlayerFallingVelocity();

		if (!_started)
		{
			CheckPlayerInitialInput();
			return;
		}
		
		SetMovementInput();
	}

	private void FixedUpdate()
	{
		HandlePlayerHorizontal();
	}

	private void CheckPlayerInitialInput()
	{
		if (Input.GetButtonDown("Horizontal"))
		{
			_started = true;
		}
	}

	private void CapPlayerFallingVelocity()
	{
		Debug.Log(PlayerRigidBody.velocity.y);
		if (PlayerRigidBody.velocity.y < _maxFallingVelocity)
		{
			PlayerRigidBody.velocity = new Vector2(PlayerRigidBody.velocity.x, _maxFallingVelocity);
		}
	}

	private void HandlePlayerHorizontal()
	{
		PlayerRigidBody.velocity = new Vector2(_movementInput.x * _horizontalMoveSpeed, PlayerRigidBody.velocity.y);
		PlayerRigidBody.position = new Vector2(Mathf.Clamp(PlayerRigidBody.position.x, -4.2f, 4.2f), PlayerRigidBody.position.y);
	}

	private void  SetMovementInput()
	{
		_movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
	}
}
