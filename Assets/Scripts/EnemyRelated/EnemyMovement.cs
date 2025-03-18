
using UnityEngine;
public class EnemyMovement : MonoBehaviour
{
    // variables
    private GameObject _player;
    private Rigidbody2D _enemyRigidBody;
    private float _direction = 1.0f;
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
        _rightAttackRangeScript.CollisionEvent += StopEnemy;
        _leftAttackRangeScript.CollisionEvent += StopEnemy;
        _rightAttackRangeScript.CollisionEndedEvent += SetBackToMovingMode;
        _leftAttackRangeScript.CollisionEndedEvent += SetBackToMovingMode;
    }
    private void Update()
    {
        // based on states do things according to it
        switch (_currentState)
        {
            case EnemyStates.moving:
                // gets player direction
                if (_player.transform.position.x > transform.position.x)
                {
                    _enemyRigidBody.linearVelocityX = 2.0f * _enemySpeedMultipier;
                    _direction = 1.0f;
                }
                else
                {
                    _enemyRigidBody.linearVelocityX = -2.0f * _enemySpeedMultipier;
                    _direction = -1.0f;
                }
                // randomly generate chance to jump 
                if (Random.Range(1,720) == 1)
                {
                    _currentState = EnemyStates.jumping;
                }
                break;
            case EnemyStates.jumping:
                if (_player.transform.position.x > transform.position.x)
                {
                    _enemyRigidBody.linearVelocity = new Vector2 ( 2.0f, 10.0f);
                    _direction = 1.0f;
                }
                else
                {
                    _enemyRigidBody.linearVelocity = new Vector2(- 2.0f, 10.0f);
                    _direction = -1.0f;
                }
                _currentState = EnemyStates.inAir;
                break;
            // case EnemyStates.attacking:
            // break;
        }
    }
    private void StopEnemy(float dir)
    {
        if (dir == _direction)
        {
            _currentState = EnemyStates.stopped;
        }
    }
    private void SetBackToMovingMode()
    {
        _currentState = EnemyStates.moving;
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
