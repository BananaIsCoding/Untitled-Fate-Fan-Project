using System;
using UnityEngine;

public class EnemyHitboxDetectScript : MonoBehaviour
{
    [SerializeField] private float _direction;
    public event Action<float, bool> CollisionEvent;
    public event Action<float, bool> CollisionEndedEvent;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CollisionEvent?.Invoke(_direction, false);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            CollisionEvent?.Invoke(_direction, true);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CollisionEndedEvent?.Invoke(_direction, false);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            CollisionEndedEvent?.Invoke(_direction, true);
        }
    }
}
