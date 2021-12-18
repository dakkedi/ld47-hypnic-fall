using System.Collections;
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
		StartCoroutine(LerpBoostMeter(newBoostValue));
	}

	private IEnumerator LerpBoostMeter(float endValue)
	{
		float timeElapsed = 0;
		float duration = 0.5f;
		float startValue = _boostSlider.value;

		while (timeElapsed < duration)
		{
			float t = timeElapsed / duration;
			t = t * t * (3f - 2f * t);
			_boostSlider.value = Mathf.Lerp(startValue, endValue, t);
			timeElapsed += Time.deltaTime;

			yield return null;
		}

		_boostSlider.value = endValue;
	}

	/// <summary>
	/// Reset the ui boost bar
	/// </summary>
	public void ResetCurrentBoostUI()
	{
		StartCoroutine(LerpBoostMeter(0f));
	}
}
