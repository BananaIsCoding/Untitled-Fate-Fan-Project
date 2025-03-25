
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
public class EnemyMovement : MonoBehaviour
{
    // variables
    private GameObject _player;
    private Rigidbody2D _enemyRigidBody;
    private float _direction = 1.0f;
    private bool _allyOnLeft;
    private bool _allyOnRight;
    private Animator _animator;
    private SpriteRenderer _enemySprite;

    private enum EnemyStates
    {
        moving,
        jumping,
        inAir,
        stopped
    }
    private EnemyStates _currentState = EnemyStates.inAir;
    [SerializeField] private EnemyHitboxDetectScript _rightAttackRangeScript;
    [SerializeField] private EnemyHitboxDetectScript _leftAttackRangeScript;
    [SerializeField] private float _enemySpeedMultipier = 1.0f;
    void Start()
    {
        // set ups
        _player = transform.parent.GetComponent<EnemySpawnerScript>()._player;
        _enemyRigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _enemySprite = GetComponent<SpriteRenderer>();
        _rightAttackRangeScript.CollisionEvent += StopEnemy;
        _leftAttackRangeScript.CollisionEvent += StopEnemy;
        _rightAttackRangeScript.CollisionEndedEvent += SetBackToMovingMode;
        _leftAttackRangeScript.CollisionEndedEvent += SetBackToMovingMode;
    }
    private void Update()
    {
        float newDirection;
        // gets player direction
        if (_player.transform.position.x > transform.position.x)
        {
            newDirection = 1.0f;
            _enemySprite.flipX = false;

        }
        else
        {
            newDirection = -1.0f;
            _enemySprite.flipX = true;
        }
        // based on states do things according to it
        switch (_currentState)
        {
            case EnemyStates.moving:
                if (_direction != newDirection)
                {
                    if ((_direction == -1.0f && _allyOnLeft) || (_direction == 1.0f && _allyOnRight))
                    {
                        _currentState = EnemyStates.stopped;
                    }
                }
                _enemyRigidBody.linearVelocityX = 3.0f * _enemySpeedMultipier * _direction;
                // randomly generate chance to jump 
                if (Random.Range(1,720) == 1)
                {
                    _currentState = EnemyStates.jumping;
                }
                
                break;
            case EnemyStates.jumping:
                _enemyRigidBody.linearVelocity = new Vector2(3.0f * newDirection, 10.0f);
                _currentState = EnemyStates.inAir;
                break;
            case EnemyStates.stopped:
                if (_direction != newDirection)
                {
                    _direction = newDirection;
                    if ((_direction == -1.0f && !_allyOnLeft) || (_direction == 1.0f && !_allyOnRight))
                    {
                        _currentState = EnemyStates.moving;
                    }
                }
                break;
        }
        _direction = newDirection;
        _animator.SetFloat("Speed", _enemyRigidBody.linearVelocity.magnitude);
    }
    private void StopEnemy(float dir, bool isAlly)
    {
        if (isAlly)
        {
            if (dir == _direction)
            {
                _currentState = EnemyStates.stopped;
            }
            switch (dir)
            {
                case -1.0f:
                    _allyOnLeft = true;
                    break;
                case 1.0f:
                    _allyOnRight = true;
                    break;
            }
        }
        else
        {
            _currentState = EnemyStates.stopped;
        }
    }
    private void SetBackToMovingMode(float dir, bool isAlly)
    {
        if (isAlly)
        {
            switch (dir)
            {
                case -1.0f:
                    _allyOnLeft = false;
                    break;
                case 1.0f:
                    _allyOnRight = false;
                    break;
            }
        }
        if (dir == _direction)
        {
            if ((!_allyOnLeft && _direction == -1.0f) || (!_allyOnRight && _direction == 1.0f))
            {
                _currentState = EnemyStates.moving;
            }
        }
    }
    // to tell code that the jump is over
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            if (_currentState != EnemyStates.stopped)
            {
                _currentState = EnemyStates.moving;
            }
        }
    }
}
