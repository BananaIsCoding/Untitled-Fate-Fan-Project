using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class PlayerHitBoxScript : MonoBehaviour
{
    //variable
    public event Action EndAttackEvent;
    public bool _burnOn;
    public float _burnDamage = 1;
    protected float _damage = 5.0f;
    protected Collider2D _hitboxCollider;
    protected float _direction;
    [SerializeField] protected float _knockbackStrength = 1.0f;
    [SerializeField] protected PlayerAttackScript _attackScript;
    [SerializeField] protected float _stunTime = 1.0f;
    [SerializeField] protected TextMeshProUGUI _enemyNameDisplayer;
    [SerializeField] protected ComboScriptManager _comboScriptManager;
    [SerializeField] protected UltimateBarScript _ultScript;
    protected virtual void Start()
    {
        _attackScript.OnPlayerAttack += EnableHitBox;
        _hitboxCollider = GetComponent<Collider2D>();
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // move it out of the vicinity
            _enemyNameDisplayer.text = collision.gameObject.name;
            HealthSystem enemyHealthScript = collision.gameObject.GetComponent<HealthSystem>();
            enemyHealthScript.DamageTakenEvent.Invoke(_damage);
            if (_burnOn)
            {
                enemyHealthScript.BurnEvent.Invoke(_burnDamage);
            }
            _comboScriptManager.HitComboCounterEvent.Invoke();
            //_ultScript.UpdateUltBarEvent?.Invoke(.025f);
            _ultScript.UpdateUltBarEvent?.Invoke(1f);
            // apply knockback to the enemy and cancel enemy's movement
            collision.gameObject.GetComponent<EnemyDamageTakenHandler>().DamageTakenEvent?.Invoke(new Vector2 (_direction * _knockbackStrength, 1 * _knockbackStrength), _stunTime);
        }
    }
    /// <summary>
    /// Activates the hitbox, making it able to hit things
    /// </summary>
    /// <param name="lifetime"></param>
    /// <param name="direction"></param>
    /// <param name="knockbackStrength"></param>
    /// <param name="damage"></param>
    protected virtual void EnableHitBox(float lifetime, float direction, float knockbackStrength, float damage)
    {
        // set up hit box
        _damage = damage;
        _direction = direction;
        _knockbackStrength = knockbackStrength;
        _hitboxCollider.enabled = true;
        // disable hit box after a while
        StartCoroutine(LifeTimeBeforeDisable(lifetime));
    }
    protected virtual IEnumerator LifeTimeBeforeDisable(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        // disable hitbox
        _hitboxCollider.enabled = false;
        // invoke event which will tell other code that attack has ended
        EndAttackEvent?.Invoke();
        gameObject.transform.localPosition = new Vector3(0.0f, -10.0f, 0.0f);
    }
}
