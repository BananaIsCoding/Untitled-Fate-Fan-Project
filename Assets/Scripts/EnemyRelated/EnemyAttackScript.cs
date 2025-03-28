
using System.Collections;
using UnityEngine;

public class EnemyAttackScript : MonoBehaviour
{
    // variables
    private bool _stunned;
    private bool _inRange;
    private bool _attacking;
    private float _direction = 1.0f;
    private int _attackAmount;
    private Collider2D _hitboxCollider;
    private Animator _enemyAnimator;
    [SerializeField] private float _waitBeforeActualAttack = 3.0f;
    [SerializeField] private EnemyHitboxDetectScript _leftAttackRange;
    [SerializeField] private EnemyHitboxDetectScript _rightAttackRange;
    [SerializeField] private float _attackCooldown = 6.0f;
    [SerializeField] private float _damage = 6.0f;
    [SerializeField] private GameObject _hitbox;
    [SerializeField] private EnemyDamageHitBox _hitboxScript;
    void OnEnable()
    {
        _stunned = false;
        StartCoroutine(Attack());
    }
    void OnDisable()
    {
        // disable enemies abillity to attack
        _stunned = true;
        _attacking = false;
        _hitboxCollider.enabled = false;
        _hitbox.transform.localPosition = new Vector3( 0.0f, -10.0f, 0.0f);
        StopAllCoroutines();
    }
    // set up
    void Start()
    {
        _leftAttackRange.CollisionEvent += WithinRangeCall;
        _leftAttackRange.CollisionEndedEvent += OutOfRange;
        _rightAttackRange.CollisionEvent += WithinRangeCall;
        _rightAttackRange.CollisionEndedEvent += OutOfRange;
        _hitbox.transform.localPosition = new Vector3( 0.0f, -10.0f, 0.0f);
        _hitboxCollider = _hitbox.GetComponent<Collider2D>();
        _enemyAnimator = GetComponent<Animator>();
    }
    // if enemy discovered a player
    private void WithinRangeCall(float theDirection, bool isAlly)
    {
        if (!(isAlly || _attacking || _stunned))
        {
            // set values for attack and start it
            _direction = theDirection;
            _hitboxScript._direction = theDirection;
            _inRange = true;
            StopAllCoroutines();
            StartCoroutine(Attack());
        }
    }
    // if player goes out of attack range
    private void OutOfRange(float dir, bool isAlly)
    {
        if (!isAlly)
        {
            _inRange = false;
            _attacking = false;
        }
    }
    // the attack code
    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(1);
        while (_inRange && !_stunned)
        {
            // Start the attack
            _attacking = true;
            _hitboxScript._damage = _damage;
            _enemyAnimator.Play("Attack");
            yield return new WaitForSeconds(_waitBeforeActualAttack);
            // move hitbox to right position
            _hitbox.transform.localPosition = new Vector3(_direction * 0.2f, 0.1f, 0.0f);
            _hitboxCollider.enabled = true;
            // wait for how long the attack should last
            yield return new WaitForSeconds(.5f);
            // End the enemy attack
            _attacking = false;
            _hitboxCollider.enabled = false;
            _hitbox.transform.localPosition = new Vector3( 0.0f, -10.0f, 0.0f);
            // To allow player to counter attack
            _attackAmount++;
            if (_attackAmount < 3)
            {
                yield return new WaitForSeconds(_attackCooldown);
            }
            else
            {
                _attackAmount = 0;
                yield return new WaitForSeconds(_attackCooldown + 3);
            }
        }
    }
}
