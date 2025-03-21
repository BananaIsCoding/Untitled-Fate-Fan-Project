using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
