using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class PlayerHitBoxScript : MonoBehaviour
{
    //variable
    public event Action EndAttackEvent;
    public float _damage = 5.0f;
    protected Collider2D _hitboxCollider;
    protected float _direction;
    [SerializeField] protected float _knockbackStrength = 1.0f;
    [SerializeField] protected PlayerAttackScript _attackScript;
    [SerializeField] protected float _stunTime = 1.0f;
    [SerializeField] protected TextMeshProUGUI _enemyNameDisplayer;
    [SerializeField] protected ComboScriptManager _comboScriptManager;
    protected virtual void Start()
    {
        _attackScript.OnPlayerAttack += EnableHitBox;
        _hitboxCollider = GetComponent<Collider2D>();
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Lol");
            // turn off collider once it hits smth
            _hitboxCollider.enabled = false;
            // move it out of the vicinity
            _enemyNameDisplayer.text = collision.gameObject.name;
            collision.gameObject.GetComponent<HealthSystem>().DamageTakenEvent.Invoke(_damage);
            _comboScriptManager.HitComboCounterEvent.Invoke();
            // apply knockback to the enemy and cancel enemy's movement
            collision.gameObject.GetComponent<EnemyDamageTakenHandler>().DamageTakenEvent?.Invoke(new Vector2 (_direction * _knockbackStrength, 1 * _knockbackStrength), _stunTime);
        }
    }
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
