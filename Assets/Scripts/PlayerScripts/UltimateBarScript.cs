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
    [SerializeField] private Animator _ultBarChargeAnimator;
    [SerializeField] private Image _ultChargeImage;

    void Start()
    {
        UpdateUltBarEvent.AddListener(UpdateUltBar);
        GetComponent<PlayerAttackScript>().UseUltEvent += UseUlt;
    }
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
    private void UseUlt()
    {
        _value = 0;
        _ultBarSlider.value = 0;
        _ultBarChargeAnimator.SetBool("Max", false);
        _ultChargeImage.enabled = false;
        UpdateUltBarEvent.AddListener(UpdateUltBar);
    }
}
