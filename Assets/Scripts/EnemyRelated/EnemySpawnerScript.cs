using System.Collections;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    public GameObject _player;
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
            Instantiate(_enemyBase, new Vector3(Random.Range(-5,5), 5, 0), new Quaternion(), transform);
            yield return new WaitForSeconds(Random.Range(20, 60));
        }
    }
}
