using System;
using UnityEngine;

public class RushHitDetectScript : PlayerHitBoxScript
{
    public event Action EnemyDetectedEvent;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // turn off collider once it hits smth
            _hitboxCollider.enabled = false;
            EnemyDetectedEvent?.Invoke();
        }
    }
}
