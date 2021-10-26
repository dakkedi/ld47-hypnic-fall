using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerBoost : MonoBehaviour
{
	// Editable fields
	[SerializeField]
	[Tooltip("The strengh of the boost. Used in calculating the player boost curve")]
	private float _boostPower = 10f;
	[SerializeField]
	[Tooltip("Powe value to manipulate the boost curve")]
	private float _boostPowerManipulator = 4f;
	[SerializeField]
	private PlayerBehavior _playerBehavior;

	// privates
	private int _collectedBoost;

	// publics
	public int _maxBoostCapacity { get; private set; }

	private void Awake()
	{
		Assert.IsNotNull(_playerBehavior);
	}

	private void Start()
	{
		_collectedBoost = 0;
		_maxBoostCapacity = 8;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == Constants.TimeBoost)
		{
			PlayerHitBoost(collision.gameObject);
		}
	}

	public int CollectedBoost()
	{
		return _collectedBoost;
	}

	public void ResetBoost()
	{
		_collectedBoost = 0;
		_playerBehavior.UpdatePlayerEffects();
		UIManager.Instance.ResetCurrentBoostUI();
	}

	/// <summary>
	/// Formula used y+(x*y*(x/4))
	/// </summary>
	public float CalculateBoost()
	{
		var currentBoostPower = _collectedBoost == 1 ? _boostPower * 0.5f : _boostPower;
		var boost = currentBoostPower + (_collectedBoost * currentBoostPower * (_collectedBoost / _boostPowerManipulator));
		return boost;
	}

	private void PlayerHitBoost(GameObject boostObject)
	{
		// TODO boostObject, give some kind of hit state
		if (_collectedBoost == _maxBoostCapacity) return;
		GameManager.instance.PlayAudioBoostPickup();
		_collectedBoost++;

		_playerBehavior.UpdatePlayerEffects();
		// update ui
		UIManager.Instance.SetCurrentBoostUI(CalculateBoost());
	}
}
