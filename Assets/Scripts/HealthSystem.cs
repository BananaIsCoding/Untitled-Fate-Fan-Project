using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    // variables
    public UnityEvent<float> DamageTakenEvent;
    public UnityEvent<float> BurnEvent;
    public event Action DeadEvent;
    public Slider _healthSlider;
    private float _health;
    [SerializeField] private float _maxHealth = 100;
    void Start()
    {
        _health = _maxHealth;
        DamageTakenEvent.AddListener(TakeDamage);
        BurnEvent.AddListener(StartBurn);
    }
    private void TakeDamage(float dmg)
    {
        _health -= dmg;
        // to not over kill it
        if (_health <= 0)
        {
            _health = 0;
            DeadEvent?.Invoke();
        }
        // adjust hp bar
        _healthSlider.value = _health / _maxHealth;
    }

    private void StartBurn(float BurnDamage = 1)
    {
        StartCoroutine(StartBurnDamage(BurnDamage));
    }
    private IEnumerator StartBurnDamage(float BurnDamage)
    {
        for (int i = 0; i < 8; i++)
        {
            yield return new WaitForSeconds(1);
            TakeDamage(BurnDamage);
        }
    }
}
