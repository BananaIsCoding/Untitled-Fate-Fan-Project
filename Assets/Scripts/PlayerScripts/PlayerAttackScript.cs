using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackScript : MonoBehaviour
{
    #region Variables
    public event Action<float, float, float> OnPlayerAttack;
    private bool _jumping;
    private bool _moveAttacking;
    private bool _crouching;
    private bool _attackStarted;
    private Coroutine _attackCoroutine;
    private MovementScript _movementScript;
    private float _direction = 1.0f;
    private float _knockbackStrength = 5.0f;
    private InputAction _moveAction;
    private InputAction _crouchAction;
    private PlayerHitBoxScript _hitboxScript;
    private Animator _playerAnimator;
    private Rigidbody2D _playerRigidBody;
    [SerializeField] private GameObject _hitbox;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackCooldown = 0.8f;
    #endregion
    private void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _crouchAction = InputSystem.actions.FindAction("Crouch");
        InputSystem.actions.FindAction("Jump").started += Jumped;
        InputSystem.actions.FindAction("Attack").performed += DefaultAttack;
        _moveAction.started += GetDirection;
        _crouchAction.started += TurnOnCrouch;
        _crouchAction.canceled += TurnOffCrouch;
        _movementScript = GetComponent<MovementScript>();
        _playerAnimator = GetComponent<Animator>();
        _playerRigidBody = GetComponent<Rigidbody2D>();
        _hitboxScript = _hitbox.GetComponent<PlayerHitBoxScript>();
        _hitboxScript.EndAttackEvent += EndTheAttack;
    }
    #region Attack conditions
    private void DefaultAttack(InputAction.CallbackContext context)
    {
        if (!_attackStarted)
        {
            _attackCoroutine ??= StartCoroutine(PerformAttack());   
        }
    }
    private void Jumped(InputAction.CallbackContext context)
    {
        if (!_attackStarted)
        {
            _jumping = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            _jumping = false;
        }
    }
    private void GetDirection(InputAction.CallbackContext context)
    {
        // call event so that if attack then it will be in right direction
        if (_moveAction.ReadValue<Vector2>().x != 0)
        {
            _direction = _moveAction.ReadValue<Vector2>().x;
        }
    }
    private void TurnOnCrouch(InputAction.CallbackContext context)
    {
        // play crouching anim
        _crouching = true;
    }
    private void TurnOffCrouch(InputAction.CallbackContext context)
    {
        // play uncrouching anim
        _crouching = false;
    }
    #endregion
    #region Main Attack Code
    private IEnumerator PerformAttack()
    {
        _knockbackStrength = 1.0f;
        // make hitbox
        if (_jumping)
        {
            _attackStarted = true;
            Debug.Log("Jump Attack!");
            // Play Animation
            _playerAnimator.Play("NeroAirKick");
            // Place hitbox a bit infront of the player 
            _hitbox.transform.localPosition = new Vector3(0.50f * _direction, -0.75f, 0);
            _hitbox.transform.localScale = new Vector3(1.75f, 0.75f, 1.0f);
        }
        else 
        {
            yield return new WaitForSeconds(.1f);
            _attackStarted = true;
            if (_crouching)
            {
                Debug.Log("Crouch Attack!");
            }
            else if (_moveAction.ReadValue<Vector2>().x != 0) 
            {
                Debug.Log("Move Attack!");
                // Play Animation
                _playerAnimator.Play("MiddleJabAttack");
                yield return new WaitForSeconds(.4f);
                // Place hitbox a bit infront of the player 
                _hitbox.transform.localPosition = new Vector3(1.55f * _direction, 0.45f, 0.0f);
                _hitbox.transform.localScale = new Vector3( 3.0f, 1.0f, 1.0f);
            }
            else
            {
                Debug.Log("Attack!");
                // Play the animation
                _playerAnimator.Play("NeroBasicAttack");
                // Place hitbox a bit infront of the player
                _hitbox.transform.localPosition = new Vector3(0.9f * _direction, -0.35f, 0.0f);
                _hitbox.transform.localScale = new Vector3(1.5f, 1.25f, 1.0f);
            }
        }
        _movementScript.enabled = false;
        OnPlayerAttack.Invoke(.5f, _direction, _knockbackStrength);
        StartCoroutine(AttackCooldown());
    } 
    private void EndTheAttack()
    {
        _attackCoroutine = null;
        _movementScript.enabled = true;
    }
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(_attackCooldown);
        _attackStarted = false;
    }
    #endregion
}
