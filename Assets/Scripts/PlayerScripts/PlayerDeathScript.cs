using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathScript : MonoBehaviour
{
    void Start()
    {
        GetComponent<HealthSystem>().DeadEvent += OnDeath;
    }
    private void OnDeath()
    {
        Time.timeScale = 0.0f;
        SceneManager.LoadSceneAsync(4, LoadSceneMode.Additive);
    }
}
