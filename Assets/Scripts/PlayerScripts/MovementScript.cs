using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class MovementScript : MonoBehaviour
{
    public event Action JumpEvent;
    public event Action JumpEndedEvent;
    // variables
    enum PlayerState
    {
        MovementLocked,
        Unstunned,
        Blocking,
        Dashing
    }
    private PlayerState _currentState;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _crouchAction;
    private float _leftRightDirection;
    private float _oldDirection;
    private float _previousMovementPressTime;
    private Rigidbody2D _rigidbody;
    private Animator _charAnimator;
    private SpriteRenderer _charSpriteRenderer;
    private bool _jumped;
    private bool _doubleJumped;
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
        _crouchAction = InputSystem.actions.FindAction("Crouch");
        _rigidbody = GetComponent<Rigidbody2D>();
        _charAnimator = GetComponent<Animator>();
        _charSpriteRenderer = GetComponent<SpriteRenderer>();
        _moveAction.performed += Move;
        _moveAction.canceled += StopMovement;
        _jumpAction.performed += Jump;
        _crouchAction.started += TurnOnCrouch;
        _crouchAction.canceled += TurnOffCrouch;
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
            // based on the direction it will flip the character
            switch (_leftRightDirection)
            {
                case 0:
                    break;
                case -1:
                    _charSpriteRenderer.flipX = true;
                    _charAnimator.SetFloat("Speed", 1);
                    break;
                case 1:
                    _charSpriteRenderer.flipX = false;
                    _charAnimator.SetFloat("Speed", 1);
                    break;
            }
            if (_leftRightDirection == _oldDirection && Time.time < _previousMovementPressTime + .5f)
            {
                Debug.Log("Dash");
                _currentState = PlayerState.Dashing;
            }
            else
            {
                _oldDirection = _leftRightDirection;
            }
            _previousMovementPressTime = Time.time;
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
        float speed = _playerSpeed > _playerMaxSpeed ? _playerMaxSpeed : _playerSpeed;
        // moves player is not stunned
        switch (_currentState)
        {
            case PlayerState.Unstunned:
                // move character
                _rigidbody.linearVelocity = new Vector2(_leftRightDirection * (speed * Time.fixedDeltaTime), _rigidbody.linearVelocity.y);
                break;
            case PlayerState.Dashing:
                _rigidbody.linearVelocity = new Vector2(_leftRightDirection * (speed * Time.fixedDeltaTime) * -2.5f, _rigidbody.linearVelocity.y);
                _charAnimator.Play("NeroDash");
                _currentState = PlayerState.MovementLocked;
                StartCoroutine(DashDuration());
                break;
        }    
    }
    private IEnumerator DashDuration()
    {
        yield return new WaitForSeconds(.5f);
        _currentState = PlayerState.Unstunned;
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
                _currentState = PlayerState.MovementLocked;
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
    // play the prep jumping animation (enabling opponent a chance to attack)
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
    #region Crouch Related
    private void TurnOnCrouch(InputAction.CallbackContext context)
    {
        // [NOTE] play crouching anim
        //_currentState = PlayerState.MovementLocked;
    }
    private void TurnOffCrouch(InputAction.CallbackContext context)
    {
        // play uncrouching anim
        //_currentState = PlayerState.MovementLocked;
    }
    #endregion
    #region Disabling And Re-enabling Movement
    // used for stun
    private void OnEnable()
    {
        if (_setUpComplete)
        {
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
            _rigidbody.linearVelocity = Vector3.zero;
        }
    }
    #endregion
}