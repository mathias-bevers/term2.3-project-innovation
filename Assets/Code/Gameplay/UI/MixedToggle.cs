using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MixedToggle : MonoBehaviour
{
	[field: SerializeField] public BoolEvent OnValueChanged { get; private set; }
	[field: SerializeField] public bool isOn
	{
		get => _isOn;
		private set
		{
			_isOn = value;
			OnValueChanged?.Invoke(_isOn);
		}
	}
	
	[SerializeField] private Sprite selectedSprite;
	[SerializeField] private Sprite idleSprite;
	[SerializeField] private Color highlightedTint;

	private bool _isOn;
}

public class BoolEvent : UnityEvent<bool> { }
