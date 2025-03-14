using System;
using UnityEngine;

public class EnemyHitboxDetectScript : MonoBehaviour
{
    public event Action CollisionEvent;
    public event Action CollisionEndedEvent;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CollisionEvent?.Invoke();
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CollisionEndedEvent?.Invoke();
        }
    }
}
