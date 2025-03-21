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
    private Coroutine _durationCoroutine;
    [SerializeField] private float _stunTime = 0.5f;
    [SerializeField] private float _flyingSliceSpeedMultpier = 1.0f;
    [SerializeField] private TextMeshProUGUI _enemyNameDisplayer;
    [SerializeField] private ComboScriptManager _comboScriptManager;

    private void Start()
    {
        _slashCollider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        StartAttack.AddListener(CommenceTheAttack);
    }
    private void CommenceTheAttack(float liftTime, float damage, float direction)
    {
        if (_durationCoroutine != null)
        {
            StopCoroutine(_durationCoroutine);
            _rigidbody.linearVelocity = Vector2.zero;
        }
        _damage = damage;
        _slashCollider.enabled = true;
        _rigidbody.AddForceX(_flyingSliceSpeedMultpier * 300 * direction);
        _durationCoroutine = StartCoroutine(LifeTimeBeforeDisable(liftTime));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            _enemyNameDisplayer.text = collision.gameObject.name;
            collision.gameObject.GetComponent<HealthSystem>().DamageTakenEvent.Invoke(_damage);
            _comboScriptManager.HitComboCounterEvent.Invoke();
            // get and cancel enemy's movement
            collision.gameObject.GetComponent<EnemyDamageTakenHandler>().DamageTakenEvent?.Invoke(Vector2.zero, _stunTime);
        }
    }
    private IEnumerator LifeTimeBeforeDisable(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        // disable hitbox
        _slashCollider.enabled = false;
        // invoke event which will tell other code that attack has ended
        gameObject.transform.localPosition = new Vector3(0.0f, -10.0f, 0.0f);
        _rigidbody.linearVelocity = Vector2.zero;
    }
}
