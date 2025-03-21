using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyDamageTakenHandler : MonoBehaviour
{
    public UnityEvent<Vector2, float> DamageTakenEvent;
    private EnemyMovement _enemyMovementScript;
    private Rigidbody2D _rigidbody;
    private Coroutine _stunCoroutine;
    private float _stunTime;
    void Start()
    {
        DamageTakenEvent.AddListener(KnockbackAndStun);
        _enemyMovementScript = gameObject.GetComponent<EnemyMovement>();
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
       // _healthScript
    }
    private void KnockbackAndStun(Vector2 direction, float stunTime)
    {
        _stunTime = stunTime;
        _rigidbody.linearVelocity = direction;
        // get and cancel enemy's movement
        _enemyMovementScript.enabled = false;
        if (_stunCoroutine != null)
        {
            StopCoroutine(_stunCoroutine);
        }
        // re-enable enemy movement after X Sec
        _stunCoroutine = StartCoroutine(EnemyStunOff());
    }
    private IEnumerator EnemyStunOff()
    {
        yield return new WaitForSeconds(_stunTime);
        // re-enable enemy movement
        if (_enemyMovementScript != null)
        {
            _enemyMovementScript.enabled = true;
        }
        _stunCoroutine = null;
    }
}
