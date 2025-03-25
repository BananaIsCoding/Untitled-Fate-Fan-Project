using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyDamageTakenHandler : MonoBehaviour
{
    public UnityEvent<Vector2, float> DamageTakenEvent;
    private EnemyMovement _enemyMovementScript;
    private EnemyAttackScript _enemyAttackScript;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private Coroutine _stunCoroutine;
    private float _stunTime;
    void Start()
    {
        // gets the movement script if it exist and use correct function based on if it exist
        _enemyMovementScript = gameObject.GetComponent<EnemyMovement>();
        if (_enemyMovementScript != null)
        {
            DamageTakenEvent.AddListener(KnockbackAndStun);
            _enemyAttackScript = gameObject.GetComponent<EnemyAttackScript>();
            _animator = gameObject.GetComponent<Animator>();
        }
        else
        {
            DamageTakenEvent.AddListener(KnockbackOnly);
        }
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
    }
    private void KnockbackAndStun(Vector2 direction, float stunTime)
    {
        _stunTime = stunTime;
        // knock enemy in a certain direction
        _rigidbody.linearVelocity = direction;
        //cancel enemy's movement
        _enemyMovementScript.enabled = false;
        _enemyAttackScript.enabled = false;
        if (_stunCoroutine != null)
        {
            StopCoroutine(_stunCoroutine);
        }
        // re-enable enemy movement after X Sec
        _stunCoroutine = StartCoroutine(EnemyStunOff());
    }
    private IEnumerator EnemyStunOff()
    {
        _animator.Play("Hurt");
        yield return new WaitForSeconds(_stunTime);
        // re-enable enemy movement
        _enemyMovementScript.enabled = true;
        _enemyAttackScript.enabled = true;
        _stunCoroutine = null;
    }
    private void KnockbackOnly(Vector2 direction, float stunTime)
    {
        _rigidbody.linearVelocity = direction;
    }
}
