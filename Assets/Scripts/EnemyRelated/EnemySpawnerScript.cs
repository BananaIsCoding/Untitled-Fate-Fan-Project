using System.Collections;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    public GameObject _player;
    [SerializeField] private GameObject _enemyBase;
    void Start()
    {
        Instantiate(_enemyBase, transform);
    }
    
}
