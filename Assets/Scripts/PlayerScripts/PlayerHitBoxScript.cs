using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class PlayerHitBoxScript : MonoBehaviour
{
    //variable
    public event Action EndAttackEvent;
    public float _damage = 5.0f;
    private Collider2D _hitboxCollider;
    private Coroutine _stunCoroutine;
    private EnemyMovement _enemyMovementScript;
    private float _direction;
    [SerializeField] private float _knockbackStrength = 1.0f;
    [SerializeField] private PlayerAttackScript _attackScript;
    [SerializeField] private float _stunTime = 1.0f;
    [SerializeField] private TextMeshProUGUI _enemyNameDisplayer;
    private void Start()
    {
        _attackScript.OnPlayerAttack += EnableHitBox;
        _hitboxCollider = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // turn off collider once it hits smth
            _hitboxCollider.enabled = false;
            // move it out of the vicinity
            gameObject.transform.localPosition = new Vector3(0.0f, -10.0f, 0.0f);
            _enemyNameDisplayer.text = collision.gameObject.name;
            collision.gameObject.GetComponent<HealthSystem>().DamageTakenEvent.Invoke(_damage);
            // apply knockback to the enemy
            collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2 (_direction * _knockbackStrength, 1 * _knockbackStrength);
            // get and cancel enemy's movement
            _enemyMovementScript = collision.gameObject.GetComponent<EnemyMovement>();
            if (_enemyMovementScript != null)
            {
                _enemyMovementScript.enabled = false;
                if (_stunCoroutine != null)
                {
                    StopCoroutine(_stunCoroutine);
                }
                // re-enable enemy movement after X Sec
                _stunCoroutine = StartCoroutine(EnemyStunOff());
            }
        }
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
    private void EnableHitBox(float lifetime, float direction, float knockbackStrength)
    {
        // set up hit box
        _direction = direction;
        _knockbackStrength = knockbackStrength;
        _hitboxCollider.enabled = true;
        // disable hit box after a while
        StartCoroutine(LifeTimeBeforeDisable(lifetime));
    }
    private IEnumerator LifeTimeBeforeDisable(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        // disable hitbox
        _hitboxCollider.enabled = false;
        // invoke event which will tell other code that attack has ended
        EndAttackEvent?.Invoke();
        gameObject.transform.localPosition = new Vector3(0.0f, -10.0f, 0.0f);
    }
}
