using UnityEngine;
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
	}
	#endregion

	[SerializeField]
	private GameObject _gameplayCanvas;

	private Slider _boostSlider = null;
	private float _sliderValue = 0;
	private float _defaultSliderValue = 10;

	[SerializeField]
	private float _currentBoost;
	public float CurrentBoost
	{
		get 
		{
			return _currentBoost;
		}
	}

	private void Start()
	{
		_boostSlider = _gameplayCanvas.GetComponentInChildren<Slider>();
		_boostSlider.value = _currentBoost;
	}

	private void Update()
	{
		if (_sliderValue != _currentBoost)
		{
			_sliderValue = _currentBoost;
			UpdateSlider();
		}
	}

	private void UpdateSlider()
	{
		if (_sliderValue != _defaultSliderValue)
			_sliderValue += _defaultSliderValue;
		_boostSlider.value = _sliderValue;
		Debug.Log(_sliderValue);
	}

	public void SetCurrentBoost(float newBoostValue) => _currentBoost = newBoostValue;
}
