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
    private Coroutine _attackCoroutine;
    private MovementScript _movementScript;
    private float _direction = 1.0f;
    private float _knockbackStrength = 5.0f;
    [SerializeField] private GameObject _hitbox;
    private PlayerHitBoxScript _hitboxScript;
    private InputAction _moveAction;
    private InputAction _crouchAction;
    #endregion
    private void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _crouchAction = InputSystem.actions.FindAction("Crouch");
        InputSystem.actions.FindAction("Jump").started += Jumped;
        InputSystem.actions.FindAction("Attack").performed += DefaultAttack;
        InputSystem.actions.FindAction("MoveAttack").performed += MoveAttack;
        _moveAction.started += GetDirection;
        _crouchAction.started += TurnOnCrouch;
        _crouchAction.canceled += TurnOffCrouch;
        _movementScript = GetComponent<MovementScript>();
        _hitboxScript = _hitbox.GetComponent<PlayerHitBoxScript>();
        _hitboxScript.EndAttackEvent += EndTheAttack;
    }
    #region Attack conditions
    private void DefaultAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Gonna Attack!");
        _attackCoroutine ??= StartCoroutine(PerformAttack());
    }
    private void MoveAttack(InputAction.CallbackContext context)
    {
        _moveAttacking = true;
    }
    private void Jumped(InputAction.CallbackContext context)
    {
        _jumping = true;
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
            Debug.Log("Jump Attack!");
            // Play Animation
            // Place hitbox a bit infront of the player 
            _hitbox.transform.localPosition = new Vector3(0.25f * _direction, -0.75f, 0);
            _hitbox.transform.localScale = new Vector3(1.5f, 0.75f, 1.0f);
        }
        else
        {
            yield return new WaitForSeconds(.1f);
            if (_crouching)
            {
                Debug.Log("Crouch Attack!");
            }
            else if (_moveAttacking) 
            {
                Debug.Log("Move Attack!");
                // Play the animation
                // Place hitbox a bit infront of the player
                _hitbox.transform.localPosition = new Vector3(0.8f * _direction, 0.1f, 0.0f);
                _hitbox.transform.localScale = new Vector3(1.75f, 1.0f, 1.0f);
                // Move the player
            }
            else
            {
                Debug.Log("Attack!");
                // Play Animation
                // Place hitbox a bit infront of the player 
                _hitbox.transform.localPosition = new Vector3(0.8f * _direction, 0.1f, 0.0f);
                _hitbox.transform.localScale = new Vector3( 2.25f, 1.0f, 1.0f);
            }
        }
        _movementScript.enabled = false;
        OnPlayerAttack.Invoke(.5f, _direction, _knockbackStrength);
    } 
    private void EndTheAttack()
    {
        _moveAttacking = false;
        _attackCoroutine = null;
        _movementScript.enabled = true;
    }
    #endregion
}
