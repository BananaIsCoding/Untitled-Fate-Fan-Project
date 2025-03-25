using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UltimateBarScript : MonoBehaviour
{
    public UnityEvent<float> UpdateUltBarEvent;
    public event Action UltBarMaxed;
    private float _value = 0;
    [SerializeField] private Slider _ultBarSlider;
    private void OnEnable()
    {
        UpdateUltBarEvent.AddListener(UpdateUltBar);
    }
    private void OnDisable()
    {
        UpdateUltBarEvent.RemoveListener(UpdateUltBar);
    }
    private void UpdateUltBar(float amountToAdd)
    {
        // add value to 
        _value += amountToAdd;
        if (_value >= 1)
        {
            _value = 0;
            _ultBarSlider.value = 100;
            UltBarMaxed?.Invoke();
            this.enabled = false;
        }
        _ultBarSlider.value = _value;
    }
    
}
