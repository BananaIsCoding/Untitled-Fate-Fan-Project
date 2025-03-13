using System;
using System.Collections;
using UnityEngine;
public class PlayerHitBoxScript : MonoBehaviour
{
    public event Action EndAttackEvent;
    private Collider2D _hitboxCollider;
    private Coroutine _lifetimeCoroutine;
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
            StopCoroutine(_lifetimeCoroutine);
            _hitboxCollider.enabled = false;
            EndAttackEvent?.Invoke();
            gameObject.transform.localPosition = new Vector3(0.0f, -10.0f, 0.0f);
        }
    }
    private void EnableHitBox(float lifetime)
    {
        _hitboxCollider.enabled = true;
        _lifetimeCoroutine = StartCoroutine(LifeTimeBeforeDisable(lifetime));
    }
    private IEnumerator LifeTimeBeforeDisable(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        _hitboxCollider.enabled = false;
        EndAttackEvent?.Invoke();
        gameObject.transform.localPosition = new Vector3(0.0f, -10.0f, 0.0f);
    }
}
