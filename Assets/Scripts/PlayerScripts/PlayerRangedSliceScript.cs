using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerRangedSliceScript : MonoBehaviour
{
    public UnityEvent<float, float, float> StartAttack;
    private float _damage = 1.0f;
    private Rigidbody2D _rigidbody;
    private Collider2D _slashCollider;
    private EnemyMovement _enemyMovementScript;
    private Coroutine _stunCoroutine;
    [SerializeField] private float _stunTime = 0.5f;
    [SerializeField] private float _flyingSliceSpeedMultpier = 1.0f;
    [SerializeField] private TextMeshProUGUI _enemyNameDisplayer;
    private void Start()
    {
        _slashCollider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        StartAttack.AddListener(CommenceTheAttack);
    }
    private void CommenceTheAttack(float liftTime, float damage, float direction)
    {
        _damage = damage;
        _slashCollider.enabled = true;
        _rigidbody.AddForceX(_flyingSliceSpeedMultpier * 300 * direction);
        StartCoroutine(LifeTimeBeforeDisable(liftTime));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            _enemyNameDisplayer.text = collision.gameObject.name;
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
    private IEnumerator LifeTimeBeforeDisable(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        // disable hitbox
        _slashCollider.enabled = false;
        // invoke event which will tell other code that attack has ended
        gameObject.transform.localPosition = new Vector3(0.0f, -10.0f, 0.0f);
    }
}
