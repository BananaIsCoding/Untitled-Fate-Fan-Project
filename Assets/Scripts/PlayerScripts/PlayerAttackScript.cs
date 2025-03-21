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
    private bool _attackSetUpStage;
    private bool _cooldownOn;
    private Coroutine _attackCoroutine;
    private MovementScript _movementScript;
    private float _direction = 1.0f;
    private float _knockbackStrength = 5.0f;
    private InputAction _moveAction;
    private InputAction _crouchAction;
    private PlayerHitBoxScript _hitboxScript;
    private PlayerRangedSliceScript _rangedSliceScript;
    private Animator _playerAnimator;
    private Rigidbody2D _playerRigidBody;
    private string _attackCombo;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _originalYAxis = 2.904933f; 
    [SerializeField] private GameObject _hitbox;
    [SerializeField] private GameObject _rangeSlice;
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
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _hitboxScript = _hitbox.GetComponent<PlayerHitBoxScript>();
        _rangedSliceScript = _rangeSlice.GetComponent<PlayerRangedSliceScript>();
        _hitboxScript.EndAttackEvent += EndTheAttack;
    }
    #region Attack conditions
    private void DefaultAttack(InputAction.CallbackContext context)
    {
        if (!_cooldownOn)
        {
            if (!_attackSetUpStage)
            {
                _attackSetUpStage = true;
                _attackCombo = "Z";
            }
            else
            {
                _attackCombo += "Z";
            }
            _attackCoroutine ??= StartCoroutine(PerformAttack());
        }
    }
    private void Jumped(InputAction.CallbackContext context)
    {
        _jumping = true;
        _attackCombo += "8";
    }
    private void GetDirection(InputAction.CallbackContext context)
    {
        // call event so that if attack then it will be in right direction
        if (_moveAction.ReadValue<Vector2>().x != 0)
        {
            _direction = _moveAction.ReadValue<Vector2>().x;
        }
        switch (_direction)
        {
            case -1.0f:
                _attackCombo += "4";
                break;
            case 1.0f:
                _attackCombo += "6";
                break;
        }
    }
    private void TurnOnCrouch(InputAction.CallbackContext context)
    {
        // play crouching anim
        _crouching = true;
        _attackCombo += "2";
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
        _movementScript.enabled = false;
        _knockbackStrength = 1.0f;
        if (gameObject.transform.position.y > _originalYAxis + 0.01f)
        {
            _attackSetUpStage = false;
            Debug.Log("Jump Attack!");
            // Play Animation
            _playerAnimator.Play("NeroAirKick");
            // Place hitbox a bit infront of the player 
            _hitbox.transform.localPosition = new Vector3(0.50f * _direction, -0.75f, 0);
            _hitbox.transform.localScale = new Vector3(1.75f, 0.75f, 1.0f);
        }
        else 
        {
            yield return new WaitForSeconds(0.5f);
            _attackSetUpStage = false;
            switch (_attackCombo)
            {
                case "Z268":
                    Debug.Log("Right Ranged Attack!");
                    _spriteRenderer.flipX = false;
                    _playerAnimator.Play("360TurnSlash");
                    yield return new WaitForSeconds(.38f);
                    _hitbox.transform.localPosition = new Vector3(1.0f, 0.0f, 0.0f);
                    _hitbox.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
                    _rangeSlice.transform.localPosition = new Vector3(1.0f, 0.0f, 0.0f);
                    _rangedSliceScript.StartAttack?.Invoke(2.5f, 1.0f, 1.0f);
                    break;
                case "Z248":
                    Debug.Log("Left Ranged Attack!");
                    _spriteRenderer.flipX = true;
                    _playerAnimator.Play("360TurnSlash");
                    yield return new WaitForSeconds(.38f);
                    _hitbox.transform.localPosition = new Vector3(-1.0f, 0.0f, 0.0f);
                    _hitbox.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
                    _rangeSlice.transform.localPosition = new Vector3(-1.0f, 0.0f, 0.0f);
                    _rangedSliceScript.StartAttack?.Invoke(2.5f, 1.0f, -1.0f);
                    break;
                case "ZZ8":
                    Debug.Log("Up special Attack!");
                    _playerAnimator.Play("NeroFlipAttack");
                    yield return new WaitForSeconds(.35f);
                    _playerRigidBody.linearVelocity = new Vector2(5 * _direction,15);
                    _playerRigidBody.gravityScale *= 1.75f;
                    yield return new WaitForSeconds(.75f);
                    _hitbox.transform.localPosition = new Vector3(_direction, -0.25f, 0.0f);
                    _hitbox.transform.localScale = new Vector3(2.0f, 1.75f, 1.0f);
                    break;
                case "Z22":
                    Debug.Log("Downslam Attack!");
                    // check in air
                    break;
                case "Z66":
                    Debug.Log("Right Dash Attack!");
                    break;
                case "Z44":
                    Debug.Log("Left Dash Attack!");
                    break;
                case "Z4268":
                    Debug.Log("Right Dash Attack!");
                    break;
                default:
                    /*if (_crouching)
                    {
                        Debug.Log("Crouch Attack!");
                    }
                    else */
                    if (_moveAction.ReadValue<Vector2>().x != 0)
                    {
                        Debug.Log("Move Attack!");
                        // Play Animation
                        _playerAnimator.Play("MiddleJabAttack");
                        yield return new WaitForSeconds(.4f);
                        // Place hitbox a bit infront of the player 
                        _hitbox.transform.localPosition = new Vector3(1.55f * _direction, 0.45f, 0.0f);
                        _hitbox.transform.localScale = new Vector3(3.0f, 1.0f, 1.0f);
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
                    break;
            }
        }
        OnPlayerAttack.Invoke(1.0f, _direction, _knockbackStrength);
        Debug.Log(_attackCombo);
        StartCoroutine(AttackCooldown());
    } 
    private void EndTheAttack()
    {
        _attackCoroutine = null;
        _movementScript.enabled = true;
        _playerRigidBody.gravityScale = 2;
    }
    private IEnumerator AttackCooldown()
    {
        _cooldownOn = true;
        yield return new WaitForSeconds(_attackCooldown);
        _cooldownOn = false;
    }
    #endregion
}
