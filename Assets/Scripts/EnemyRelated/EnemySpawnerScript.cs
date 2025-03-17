using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnerScript : MonoBehaviour
{
    public GameObject _player;
    public Slider _refToEnemyHPBar;
    public TextMeshProUGUI _nameText;
    [SerializeField] private GameObject _enemyBase;
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // spawns enemy randomly across the map
            GameObject enemy = Instantiate(_enemyBase, new Vector3(Random.Range(-5,5), 5, 0), new Quaternion(), transform);
            HealthSystem enemyHealthScript = enemy.GetComponent<HealthSystem>();

            if (enemyHealthScript != null)
            {
                enemyHealthScript._healthSlider = _refToEnemyHPBar;
            }
            else
            {
                Debug.Log("Failed");
            }
            //enemyHealthScript._

            yield return new WaitForSeconds(Random.Range(20, 60));
        }
    }
}
