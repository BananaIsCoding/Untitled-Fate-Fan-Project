using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackScript : MonoBehaviour
{
    public event Action<float, float, float> OnPlayerAttack;
    private bool _jumping;
    private bool _moveAttacking;
    private Coroutine _attackCoroutine;
    private MovementScript _movementScript;
    private float _direction = 1.0f;
    private float _knockbackStrength = 5.0f;
    [SerializeField] private GameObject _hitbox;
    private PlayerHitBoxScript _hitboxScript;
    private void Start()
    {
        InputSystem.actions.FindAction("Attack").performed += DefaultAttack;
        InputSystem.actions.FindAction("MoveAttack").performed += MoveAttack;
        _movementScript = GetComponent<MovementScript>();
        _movementScript.JumpEvent += Jumped;
        _movementScript.JumpEndedEvent += JumpingEnded;
        _movementScript.SwitchDirectionEvent += GetDirection;
        _hitboxScript = _hitbox.GetComponent<PlayerHitBoxScript>();
        _hitboxScript.EndAttackEvent += EndTheAttack;
    }
    private void DefaultAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Gonna Attack!");
        _attackCoroutine ??= StartCoroutine(PerformAttack());
    }
    private void MoveAttack(InputAction.CallbackContext context)
    {
        _moveAttacking = true;
    }
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
            if (_moveAttacking) 
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
    private void Jumped()
    {
        _jumping = true;
    }
    private void JumpingEnded()
    {
        _jumping = false;
    }
    private void GetDirection(float direction)
    {
        _direction = direction;
    }
}
