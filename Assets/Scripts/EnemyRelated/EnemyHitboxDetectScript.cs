using System;
using UnityEngine;

public class EnemyHitboxDetectScript : MonoBehaviour
{
    [SerializeField] private float _direction;
    public event Action<float> CollisionEvent;
    public event Action CollisionEndedEvent;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            CollisionEvent?.Invoke(_direction);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            CollisionEndedEvent?.Invoke();
        }
    }
}
