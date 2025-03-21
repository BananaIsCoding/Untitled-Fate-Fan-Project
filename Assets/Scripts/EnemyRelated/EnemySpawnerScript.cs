using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnerScript : MonoBehaviour
{
    public GameObject _player;
    public Slider _refToEnemyHPBar;
    public TextMeshProUGUI _nameText;
    public ScoreManagerScript _scoreManagerScript;
    [SerializeField] private GameObject _enemyBase;
    [SerializeField] private float _minimunSpawnTime = 20;
    [SerializeField] private float _maximumSpawnTime = 60;
    private int _amountSpawned = 0;
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // spawns enemy randomly across the map
            _amountSpawned++;
            GameObject enemy = Instantiate(_enemyBase, new Vector3(Random.Range(-60, 60), 3, 0), new Quaternion(), transform);
            enemy.name = "Enemy " + _amountSpawned;
            enemy.GetComponent<HealthSystem>()._healthSlider = _refToEnemyHPBar;;
            enemy.GetComponent<EnemyDeathScript>()._scoreScript = _scoreManagerScript;
            yield return new WaitForSeconds(Random.Range(_minimunSpawnTime, _maximumSpawnTime));
        }
    }
}
