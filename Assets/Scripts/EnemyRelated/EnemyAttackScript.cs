
using System.Collections;
using UnityEngine;

public class EnemyAttackScript : MonoBehaviour
{
    // variables
    private bool _inRange;
    private bool _attacking;
    private float _waitBeforeActualAttack = 3.0f;
    private float _direction = 1.0f;
    [SerializeField] private EnemyHitboxDetectScript _leftAttackRange;
    [SerializeField] private EnemyHitboxDetectScript _rightAttackRange;
    [SerializeField] private float _attackCooldown = 6.0f;
    [SerializeField] private float _damage = 6.0f;
    [SerializeField] private GameObject _hitbox;
    [SerializeField] private EnemyDamageHitBox _hitboxScript;
    private Collider2D _hitboxCollider;
    // set up
    void Start()
    {
        _leftAttackRange.CollisionEvent += WithinRangeCall;
        _leftAttackRange.CollisionEndedEvent += OutOfRange;
        _rightAttackRange.CollisionEvent += WithinRangeCall;
        _rightAttackRange.CollisionEndedEvent += OutOfRange;
        _hitboxScript.AttackEvent += EndAttack;
        _hitbox.transform.localPosition = new Vector3( 0.0f, -10.0f, 0.0f);
        _hitboxCollider = _hitbox.GetComponent<Collider2D>();
    }
    // if enemy discovered a player
    private void WithinRangeCall(float theDirection, bool isAlly)
    {
        if (!isAlly && !_attacking)
        {
            _direction = theDirection;
            _hitboxScript._direction = theDirection;
            _inRange = true;
            _attacking = true;
            StartCoroutine(Attack());
        }
    }
    // if player goes out of attack range
    private void OutOfRange(float dir, bool isAlly)
    {
        // because this only detects if player or enemy leaves the attack range
        // and so if it is not the enemy it must be the player
        _inRange = !isAlly;
    }
    // attack
    private IEnumerator Attack()
    {
        _hitboxScript._damage = _damage;
        yield return new WaitForSeconds(_waitBeforeActualAttack);
        _hitbox.transform.localPosition = new Vector3(_direction, 0.0f, 0.0f);
        _hitboxCollider.enabled = true;
        StartCoroutine(AttackDuration());
    }
    // goes how long the attack should last
    private IEnumerator AttackDuration()
    {
        yield return new WaitForSeconds(.5f);
        EndAttack();
    }
    // End the enemy attack
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
    // to prevent enemy from spamming
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
