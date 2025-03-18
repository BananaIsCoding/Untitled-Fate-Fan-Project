using UnityEngine;

public class PunchingBagScript : MonoBehaviour
{
    [SerializeField] private HealthSystem _healthScript;
    void Start()
    {
        _healthScript.DeadEvent += ResetHealth;
    }
    private void ResetHealth()
    {
        _healthScript.DamageTakenEvent.Invoke(-100.0f);
    }
}
