using UnityEngine;
public class EnemyMovement : MonoBehaviour
{
    private GameObject _player;
    private Rigidbody2D _enemyRigidBody;
    private enum EnemyStates
    {
        moving,
        jumping,
        attacking
    }
    private EnemyStates _currentState = EnemyStates.moving;
    [SerializeField] private EnemyHitboxDetectScript _rightAttackRangeScript;
    void Start()
    {
        _player = transform.parent.GetComponent<EnemySpawnerScript>()._player;
        _enemyRigidBody = GetComponent<Rigidbody2D>();
        _rightAttackRangeScript.CollisionEvent += SetAttackMode;
        _rightAttackRangeScript.CollisionEndedEvent += SetBackToMovingMode;
    }
    private void Update()
    {
        
        switch (_currentState)
        {
            case EnemyStates.moving:
                if (_player.transform.position.x > transform.position.x)
                {
                    _enemyRigidBody.linearVelocityX = 1;
                }
                else
                {
                    _enemyRigidBody.linearVelocityX = -1;
                }


            break;
            case EnemyStates.jumping:
            break;
            // case EnemyStates.attacking:
            // break;
        }
    }
    private void SetAttackMode()
    {
        _currentState = EnemyStates.attacking;
    }
    private void SetBackToMovingMode()
    {
        _currentState = EnemyStates.moving;
    }
}
