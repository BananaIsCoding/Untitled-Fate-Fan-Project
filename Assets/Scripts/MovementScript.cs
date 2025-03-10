using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{
    public event Action JumpEndedEvent;
    // variables
    private InputAction _moveAction;
    private float _leftRightDirection;
    private Rigidbody2D _rigidbody;
    private bool _jumped;
    private bool _doubleJumped;
    [SerializeField] private float _jumpPower = 10;
    [Header("Movement parameters")]
    [SerializeField] private float _playerSpeed = 200f;
    [SerializeField] private float _playerMaxSpeed = 1000f;
    void Start()
    {
        // set up by getting required things and linking procedures/functions to certain input
        InputSystem.actions.FindAction("Jump").performed += Jump;
        _moveAction = InputSystem.actions.FindAction("Move");
        _rigidbody = GetComponent<Rigidbody2D>();
        _moveAction.performed += Move;
        _moveAction.canceled += StopMovement;
    }
    // gets the which direction player wants to move in
    private void Move(InputAction.CallbackContext context)
    {
        _leftRightDirection = _moveAction.ReadValue<Vector2>().x;
    }
    private void StopMovement(InputAction.CallbackContext context)
    {
        _leftRightDirection = 0;
    }
    private void FixedUpdate()
    {
        // move character
        float speed = _playerSpeed > _playerMaxSpeed ? _playerMaxSpeed : _playerSpeed;
        _rigidbody.linearVelocity = new Vector2(_leftRightDirection * (speed * Time.fixedDeltaTime), _rigidbody.linearVelocity.y);
    }
    private void Jump(InputAction.CallbackContext context)
    {
        // check if jumped alr or double jumped yet and if not done yet then make character jump
        if (!_jumped)
        {
            _jumped = true;
            _rigidbody.linearVelocityY = _jumpPower;
        }
        else if (!_doubleJumped) 
        {
            _doubleJumped = true;
            _rigidbody.linearVelocityY = _jumpPower;
        }
    }
    // used to check if player hits the ground (meaning their jump has ended)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            _jumped = false;
            _doubleJumped = false;
            JumpEndedEvent?.Invoke();
        }
    }
}
