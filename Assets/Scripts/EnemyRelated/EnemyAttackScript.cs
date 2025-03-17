
using System.Collections;
using UnityEngine;

public class EnemyAttackScript : MonoBehaviour
{
    private bool _inRange;
    private bool _attacking;
    private float _waitBeforeActualAttack = 3.0f;
    private float _direction = 1.0f;
    [SerializeField] private EnemyHitboxDetectScript _leftAttackRange;
    [SerializeField] private float _attackCooldown = 6.0f;
    [SerializeField] private float _damage = 6.0f;
    [SerializeField] private GameObject _hitbox;
    [SerializeField] private EnemyDamageHitBox _hitboxScript;
    private Collider2D _hitboxCollider;
    void Start()
    {
        _leftAttackRange.CollisionEvent += WithinRangeCall;
        _leftAttackRange.CollisionEndedEvent += OutOfRange;
        _hitboxScript.AttackEvent += EndAttack;
        _hitbox.transform.localPosition = new Vector3( 0.0f, -10.0f, 0.0f);
        _hitboxCollider = _hitbox.GetComponent<Collider2D>();
    }
    private void WithinRangeCall(float theDirection)
    {
        _direction = theDirection;
        _hitboxScript._direction = theDirection;
        _inRange = true;
        _attacking = true;
        StartCoroutine(Attack());
    }
    private void OutOfRange()
    {
        _inRange = false;
    }
    private IEnumerator Attack()
    {
        _hitboxScript._damage = _damage;
        yield return new WaitForSeconds(_waitBeforeActualAttack);
        _hitbox.transform.localPosition = new Vector3(_direction, 0.0f, 0.0f);
        _hitboxCollider.enabled = true;
        StartCoroutine(AttackDuration());
    }
    private IEnumerator AttackDuration()
    {
        yield return new WaitForSeconds(.5f);
        EndAttack();
    }
    private void EndAttack()
    {
        if (_attacking)
        {
            _attacking = false;
            _hitboxCollider.enabled = false;
            _hitbox.transform.localPosition = new Vector3( 0.0f, -10.0f, 0.0f);
            StartCoroutine(Cooldown());
        }
    }
    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(_attackCooldown);
        if (_inRange)
        {
            _attacking = true;
            StartCoroutine(Attack());
        }
    }
}
