using System;
using UnityEngine;

public class EnemyDamageHitBox : MonoBehaviour
{
    public event Action AttackEvent;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit " + collision.name + "!");
            AttackEvent?.Invoke();
        }
    }
}
