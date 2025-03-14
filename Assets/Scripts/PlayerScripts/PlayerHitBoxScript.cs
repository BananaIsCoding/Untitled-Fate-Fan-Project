using System;
using System.Collections;
using UnityEngine;
public class PlayerHitBoxScript : MonoBehaviour
{
    public event Action EndAttackEvent;
    private Collider2D _hitboxCollider;
    private Coroutine _lifetimeCoroutine;
    private float _direction;
    private float _knockbackStrength = 1.0f;
    [SerializeField] private PlayerAttackScript _attackScript;
    private void Start()
    {
        _attackScript.OnPlayerAttack += EnableHitBox;
        _hitboxCollider = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            _hitboxCollider.enabled = false;
            gameObject.transform.localPosition = new Vector3(0.0f, -10.0f, 0.0f);
            collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2 (_direction * _knockbackStrength, 1 * _knockbackStrength);
        }
    }
    private void EnableHitBox(float lifetime, float direction, float knockbackStrength)
    {
        _direction = direction;
        _knockbackStrength = knockbackStrength;
        _hitboxCollider.enabled = true;
        StartCoroutine(LifeTimeBeforeDisable(lifetime));
    }
    private IEnumerator LifeTimeBeforeDisable(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        _hitboxCollider.enabled = false;
        EndAttackEvent?.Invoke();
        gameObject.transform.localPosition = new Vector3(0.0f, -10.0f, 0.0f);
    }
}
