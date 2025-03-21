using UnityEngine;

public class EnemyDeathScript : MonoBehaviour
{
    public ScoreManagerScript _scoreScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<HealthSystem>().DeadEvent += OnDeath;
    }
    private void OnDeath()
    {
        _scoreScript.AddScoreEvent.Invoke(200);
        Destroy(gameObject);
    }
}
