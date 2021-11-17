using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	#region singleton declaration
	private static UIManager _instance;
	public static UIManager Instance
	{
		get
		{
			if (_instance == null)
				Debug.LogError("UIManager is NULL");

			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;

		Assert.IsNotNull(_boostSlider);
	}
	#endregion

	// Boost slider component from canvas 
	[SerializeField]
	private Slider _boostSlider;

	/// <summary>
	/// Set the ui total boost
	/// </summary>
	/// <param name="newBoostValue"></param>
	public void SetCurrentBoostUI(float newBoostValue)
	{
		_boostSlider.value = newBoostValue;
	}

	/// <summary>
	/// Reset the ui boost bar
	/// </summary>
	public void ResetCurrentBoostUI()
	{
		_boostSlider.value = 0;
	}
}
