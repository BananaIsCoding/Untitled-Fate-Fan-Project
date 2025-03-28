using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UltimateBarScript : MonoBehaviour
{
    private void OnEnable()
    {
        UpdateUltBarEvent.AddListener(UpdateUltBar);
    }
    private void OnDisable()
    {
        UpdateUltBarEvent.RemoveListener(UpdateUltBar);
    }
    // variables and events
    public UnityEvent<float> UpdateUltBarEvent;
    public event Action UltBarMaxed;
    private float _value = 0;
    [SerializeField] private Slider _ultBarSlider;
    [SerializeField] private Animator _ultBarChargeAnimator;
    [SerializeField] private Image _ultChargeImage;
    void Start()
    {
        GetComponent<PlayerAttackScript>().UseUltEvent += UseUlt;
    }
    // procedure to add increase the ultimate bar
    private void UpdateUltBar(float amountToAdd)
    {
        // add value to 
        _value += amountToAdd;
        if (_value >= 1)
        {
            _value = 1;
            _ultBarChargeAnimator.SetBool("Max", true);
            _ultChargeImage.enabled = true;
            UltBarMaxed?.Invoke();
            UpdateUltBarEvent.RemoveListener(UpdateUltBar);
        }
        _ultBarSlider.value = _value;
    }
    // function to set the ult bar back to 0;
    private void UseUlt()
    {
        _value = 0;
        _ultBarSlider.value = 0;
        _ultBarChargeAnimator.SetBool("Max", false);
        _ultChargeImage.enabled = false;
        UpdateUltBarEvent.AddListener(UpdateUltBar);
        enabled = false;
    }
}
