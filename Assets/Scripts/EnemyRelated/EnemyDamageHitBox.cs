using System;
using System.Collections;
using UnityEngine;

public class EnemyDamageHitBox : MonoBehaviour
{
    public float _damage = 5.0f;
    public float _direction;
    [SerializeField] private float _stunTime = 0.5f;
    [SerializeField] private float _knockbackStrength = 1.0f;
    // variable to save player stuff so it does not need to re - fetch
    private HealthSystem _playerHealthSystem;
    private MovementScript _playerMovementScript;
    private Rigidbody2D _playerRigidbody;
    private Animator _playerAnimator;
    private PlayerAttackScript _playerAttackScript;
    private Coroutine _stunCoroutine;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.localPosition = new Vector3(0, -10, 0);
            if (_playerHealthSystem == null)
            {
                GameObject thePlayer = collision.gameObject;
                _playerHealthSystem = thePlayer.GetComponent<HealthSystem>();
                _playerMovementScript = thePlayer.GetComponent<MovementScript>();
                _playerRigidbody = thePlayer.GetComponent<Rigidbody2D>();
                _playerAnimator = thePlayer.GetComponent<Animator>();
                _playerAttackScript = thePlayer.GetComponent<PlayerAttackScript>();
            }
            _playerMovementScript.enabled = false;
            _playerRigidbody.linearVelocity = new Vector2(_direction * _knockbackStrength, 1.5f * _knockbackStrength);
            _playerHealthSystem.DamageTakenEvent.Invoke(_damage);
            _playerAnimator.Play("TakeDamage");
            _playerAttackScript.enabled = false;
            if (_stunCoroutine != null)
            {
                StopCoroutine(_stunCoroutine);
            }
            _stunCoroutine = StartCoroutine(EndHitStun());
        }
    }
    private IEnumerator EndHitStun()
    {
        yield return new WaitForSeconds(_stunTime);
        _playerMovementScript.enabled = true;
        _playerAttackScript.enabled = true;
        _stunCoroutine = null;
    }
}
