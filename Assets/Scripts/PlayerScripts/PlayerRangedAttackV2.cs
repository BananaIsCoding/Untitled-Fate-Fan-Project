using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerRangedAttackV2 : PlayerHitBoxScript
{
    public UnityEvent<float, float, float> StartAttack;
    private Rigidbody2D _rigidbody;
    private Coroutine _durationCoroutine;
    [SerializeField] private float _flyingSliceSpeedMultpier = 1.0f;
    protected override void Start()
    {
        _hitboxCollider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        StartAttack.AddListener(CommenceTheAttack);
    }
    private void CommenceTheAttack(float liftTime, float damage, float direction)
    {
        // check if the slash is already active
        if (_durationCoroutine != null)
        {
            StopCoroutine(_durationCoroutine);
            _rigidbody.linearVelocity = Vector2.zero;
        }
        _damage = damage;
        _hitboxCollider.enabled = true;
        // make the slash move
        _rigidbody.AddForceX(_flyingSliceSpeedMultpier * 300 * direction);
        _durationCoroutine = StartCoroutine(LifeTimeBeforeDisable(liftTime));
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
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
    protected override IEnumerator LifeTimeBeforeDisable(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        // disable hitbox
        _hitboxCollider.enabled = false;
        // disable the slash hitbox
        gameObject.transform.localPosition = new Vector3(0.0f, -10.0f, 0.0f);
        _rigidbody.linearVelocity = Vector2.zero;
    }
}
