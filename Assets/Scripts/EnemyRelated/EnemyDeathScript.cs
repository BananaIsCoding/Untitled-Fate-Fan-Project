using UnityEngine;

public class EnemyDeathScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<HealthSystem>().DeadEvent += OnDeath;
    }

    private void OnDeath()
    {
        Destroy(gameObject);
    }
}
