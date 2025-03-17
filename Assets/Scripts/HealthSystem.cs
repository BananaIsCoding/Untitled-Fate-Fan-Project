using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    // variables
    public UnityEvent<float> DamageTakenEvent;
    public event Action DeadEvent;
    private float _health;
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private Slider _healthSlider;
    void Start()
    {
        _health = _maxHealth;
        DamageTakenEvent.AddListener(TakeDamage);
    }
    private void TakeDamage(float dmg)
    {
        _health -= dmg;
        // to not over kill it
        if (_health < 0)
        {
            _health = 0;
            DeadEvent?.Invoke();
        }
        // adjust hp bar
        _healthSlider.value = _health / _maxHealth;
        // [Note] play hurt anim
    }
}
