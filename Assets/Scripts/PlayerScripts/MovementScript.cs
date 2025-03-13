using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class MovementScript : MonoBehaviour
{
    public event Action JumpEvent;
    public event Action JumpEndedEvent;
    public event Action<float> SwitchDirectionEvent;
    // variables
    enum PlayerState
    {
        Stunned,
        Unstunned,
        Blocking
    }
    private PlayerState _currentState;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private float _leftRightDirection;
    private Rigidbody2D _rigidbody;
    private Animator _charAnimator;
    private SpriteRenderer _charSpriteRenderer;
    private bool _jumped;
    private bool _doubleJumped;
    private bool _facingRight;
    private bool _setUpComplete;
    [SerializeField] private float _jumpPower = 10;
    [Header("Movement parameters")]
    [SerializeField] private float _playerSpeed = 200f;
    [SerializeField] private float _playerMaxSpeed = 1000f;
    void Start()
    {
        _currentState = PlayerState.Unstunned;
        // set up by getting required things and linking procedures/functions to certain input
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _moveAction = InputSystem.actions.FindAction("Move");
        _rigidbody = GetComponent<Rigidbody2D>();
        _charAnimator = GetComponent<Animator>();
        _charSpriteRenderer = GetComponent<SpriteRenderer>();
        _moveAction.performed += Move;
        _moveAction.canceled += StopMovement;
        _jumpAction.performed += Jump;
        _setUpComplete = true;
    }
    // gets the which direction player wants to move in
    private void Move(InputAction.CallbackContext context)
    {
        // check if player is not stunned
        if (_currentState == PlayerState.Unstunned)
        {
            // get direction
            _leftRightDirection = _moveAction.ReadValue<Vector2>().x;
            // call event so that if attack then it will be in right direction
            SwitchDirectionEvent?.Invoke(_leftRightDirection);
            // based on the direction it will flip the character
            switch (_leftRightDirection)
            {
                case 0:
                    break;
                case -1:
                    _facingRight = false;
                    _charSpriteRenderer.flipX = true;
                    _charAnimator.SetFloat("Speed", 1);
                    break;
                case 1:
                    _facingRight = true;
                    _charSpriteRenderer.flipX = false;
                    _charAnimator.SetFloat("Speed", 1);
                    break;
            }
        }
    }
    private void StopMovement(InputAction.CallbackContext context)
    {
        // cancel movement and set animation to idle
        _leftRightDirection = 0;
        _charAnimator.SetFloat("Speed", 0);
    }
    private void FixedUpdate()
    {
        // moves player is not stunned
        if (_currentState == PlayerState.Unstunned)
        {
            // move character
            float speed = _playerSpeed > _playerMaxSpeed ? _playerMaxSpeed : _playerSpeed;
            _rigidbody.linearVelocity = new Vector2(_leftRightDirection * (speed * Time.fixedDeltaTime), _rigidbody.linearVelocity.y);
        }    
    }
    #region Jumping Related Functions
    private void Jump(InputAction.CallbackContext context)
    {
        // prep the jump if player is not stun
        if (_currentState == PlayerState.Unstunned)
        {
            _charAnimator.SetBool("Jumping", true);
            // check if jumped alr or double jumped yet and if not done yet then make character jump
            if (!_jumped)
            {
                _charAnimator.Play("NeroPrepJump");
                _jumpAction.performed -= Jump;
                _jumped = true;
                _currentState = PlayerState.Stunned;
                StartCoroutine(DelayedJump());
            }
            else if (!_doubleJumped)
            {
                _doubleJumped = true;
                _charAnimator.Play("NeroJumpAnim");
                _rigidbody.linearVelocityY = _jumpPower;
            }
        }
    }
    private IEnumerator DelayedJump()
    {
        yield return new WaitForSeconds(0.3f);
        JumpEvent?.Invoke();
        _currentState = PlayerState.Unstunned;
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
    #endregion
    #region Disabling And Re-enabling Movement
    // used for stun
    private void OnEnable()
    {
        if (_setUpComplete)
        {
            Debug.Log("Hi");
            _moveAction.performed += Move;
            _moveAction.canceled += StopMovement;
            _jumpAction.performed += Jump;
            Move(new InputAction.CallbackContext());
        }
    }
    private void OnDisable()
    {
        _moveAction.performed -= Move;
        _moveAction.canceled -= StopMovement;
        _jumpAction.performed -= Jump;
        _charAnimator.SetFloat("Speed", 0.0f);
        if (_rigidbody.linearVelocity.y > -0.5f && _rigidbody.linearVelocity.y < 0.5f)
        {
            Debug.Log(_rigidbody.linearVelocity.y);
            _rigidbody.linearVelocity = Vector3.zero;
        }
    }
    #endregion
}