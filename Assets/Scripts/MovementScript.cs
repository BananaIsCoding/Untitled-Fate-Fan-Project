using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{
    public event Action JumpEvent;
    public event Action JumpEndedEvent;
    // variables
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private float _leftRightDirection;
    private Rigidbody2D _rigidbody;
    private Animator _charAnimator;
    private SpriteRenderer _charSpriteRenderer;
    private bool _jumped;
    private bool _doubleJumped;
    private bool _facingRight;
    [SerializeField] private float _jumpPower = 10;
    [Header("Movement parameters")]
    [SerializeField] private float _playerSpeed = 200f;
    [SerializeField] private float _playerMaxSpeed = 1000f;
    void Start()
    {
        // set up by getting required things and linking procedures/functions to certain input
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _moveAction = InputSystem.actions.FindAction("Move");
        _rigidbody = GetComponent<Rigidbody2D>();
        _charAnimator = GetComponent<Animator>();
        _charSpriteRenderer = GetComponent<SpriteRenderer>();
        _moveAction.performed += Move;
        _moveAction.canceled += StopMovement;
        _jumpAction.performed += Jump;
    }
    // gets the which direction player wants to move in
    private void Move(InputAction.CallbackContext context)
    {
        _leftRightDirection = _moveAction.ReadValue<Vector2>().x;
        switch (_leftRightDirection)
        {
            case -1:
                _facingRight = false;
                _charSpriteRenderer.flipX = true;
            break;
            case 1:
                _facingRight = true;
                _charSpriteRenderer.flipX = false;
            break;
        }
        _charAnimator.SetFloat("Speed", 1);
    }
    private void StopMovement(InputAction.CallbackContext context)
    {
        _leftRightDirection = 0;
        _charAnimator.SetFloat("Speed", 0);
    }
    private void FixedUpdate()
    {
        // move character
        float speed = _playerSpeed > _playerMaxSpeed ? _playerMaxSpeed : _playerSpeed;
        _rigidbody.linearVelocity = new Vector2(_leftRightDirection * (speed * Time.fixedDeltaTime), _rigidbody.linearVelocity.y);
    }
    private void Jump(InputAction.CallbackContext context)
    {
        _charAnimator.SetBool("Jumping", true);
        // check if jumped alr or double jumped yet and if not done yet then make character jump
        if (!_jumped)
        {
            _charAnimator.Play("NeroPrepJump");
            _jumpAction.performed -= Jump;
            _jumped = true;
            StartCoroutine(DelayedJump());
        }
        else if (!_doubleJumped) 
        {
            _doubleJumped = true;
            _charAnimator.Play("NeroJumpAnim");
            _rigidbody.linearVelocityY = _jumpPower;
        }
    }
    IEnumerator DelayedJump()
    {
        yield return new WaitForSeconds(0.8f);
        JumpEvent?.Invoke();
        _rigidbody.linearVelocityY = _jumpPower;
        _jumpAction.performed += Jump;
    }
    // used to check if player hits the ground (meaning their jump has ended)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            _jumped = false;
            _doubleJumped = false;
            _charAnimator.SetBool("Jumping", false);
            JumpEndedEvent?.Invoke();
        }
    }
}