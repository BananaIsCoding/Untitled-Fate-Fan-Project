using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAttackScript : MonoBehaviour
{
    #region Variables
    public event Action<float, float, float, float> OnPlayerAttack;
    private bool _jumping;
    private bool _moveAttacking;
    private bool _crouching;
    private bool _chargeDownComplete;
    private bool _attackSetUpStage;
    private bool _cooldownOn;
    private bool _arenaActive;
    private bool _arenaCooldown;
    private Coroutine _attackCoroutine;
    private Coroutine _rushCoroutine;
    private Coroutine _chargeArenaSkillCoroutine;
    private MovementScript _movementScript;
    private float _direction = 1.0f;
    private float _knockbackStrength = 5.0f;
    private InputAction _moveAction;
    private InputAction _crouchAction;
    private PlayerHitBoxScript _hitboxScript;
    private PlayerRangedAttackV2 _rangedSliceScriptV2;
    private RushHitDetectScript _rushHitDetectScript;
    private Animator _playerAnimator;
    private Rigidbody2D _playerRigidBody;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _playerCollider;
    private string _attackCombo;
    [SerializeField] private float _originalYAxis = 2.904933f; 
    [SerializeField] private GameObject _hitbox;
    [SerializeField] private GameObject _rangeSlice;
    [SerializeField] private GameObject _rushAttackRangeHitBox;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackCooldown = 0.8f;
    [SerializeField] private Canvas _chargeDownSpecialCanvas;
    [SerializeField] private Slider _ArenaChargeUpSlider;
    [SerializeField] private Image _arenaHintImage;
    [SerializeField] private Image _whiteScreen;
    [SerializeField] private Grid _arenaBackground;
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
        _playerCollider = GetComponent<Collider2D>();
        _hitboxScript = _hitbox.GetComponent<PlayerHitBoxScript>();
        _rangedSliceScriptV2 = _rangeSlice.GetComponent<PlayerRangedAttackV2>();
        _rushHitDetectScript = _rushAttackRangeHitBox.GetComponent<RushHitDetectScript>();
        _hitboxScript.EndAttackEvent += EndTheAttack;
        _rushHitDetectScript.EnemyDetectedEvent += RushDetectEnemy;
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
        if (!_arenaCooldown)
        {
            _chargeArenaSkillCoroutine = StartCoroutine(ChargeSpecialArenaSkill());
        }
    }
    private IEnumerator ChargeSpecialArenaSkill()
    {
        yield return new WaitForSeconds(.5f);
        _ArenaChargeUpSlider.value = 0;
        float addAmount = 1.0f/15.0f;
        _chargeDownSpecialCanvas.enabled = true;
        for (int i = 0; i < 15; i++)
        {
            yield return new WaitForSeconds(.25f);
            _ArenaChargeUpSlider.value += addAmount;
        }
        _arenaHintImage.enabled = true;
        _chargeDownComplete = true;
    }
    private void TurnOffCrouch(InputAction.CallbackContext context)
    {
        // play uncrouching anim
        _crouching = false;
        _chargeDownComplete = false;
        _chargeDownSpecialCanvas.enabled = false;
        _arenaHintImage.enabled = false;
        StopCoroutine(_chargeArenaSkillCoroutine);
    }
    #endregion
    #region Main Attack Code
    private IEnumerator PerformAttack()
    {
        float attackduration = 0.2f;
        _movementScript.enabled = false;
        _knockbackStrength = 1.0f;
        if (_chargeDownComplete)
        {
            Debug.Log("Aestus Domus Aurea");
            _chargeDownComplete = false;
            _chargeDownSpecialCanvas.enabled = false;
            _arenaHintImage.enabled = false;
            tag = "Untagged";
            _playerAnimator.Play("NeroSummonArena");
            yield return new WaitForSeconds(.1f);
            _whiteScreen.enabled = true;
            for (float i = 0.9f; i >= 0; i -= 0.1f)
            {
                yield return new WaitForSeconds(.2f);
                _whiteScreen.color = new Color (1,1,1,i);
            }
            _playerAnimator.Play("NeroSummonArenaEnd");
            _arenaBackground.enabled = true;
            _whiteScreen.enabled = false;
            _arenaActive = true;
            EndTheAttack();
            StartCoroutine(AttackCooldown());
            tag = "Player";
            StartCoroutine(EndArenaField());
            yield break;
        }
        else if (gameObject.transform.position.y > _originalYAxis + 0.01f)
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
            if (_attackCombo == "Z426")
            {
                yield return new WaitForSeconds(0.25f);
            }
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
                    _rangedSliceScriptV2.StartAttack?.Invoke(2.5f, 1.0f, 1.0f);
                    break;
                case "Z248":
                    Debug.Log("Left Ranged Attack!");
                    _spriteRenderer.flipX = true;
                    _playerAnimator.Play("360TurnSlash");
                    yield return new WaitForSeconds(.38f);
                    _hitbox.transform.localPosition = new Vector3(-1.0f, 0.0f, 0.0f);
                    _hitbox.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
                    _rangeSlice.transform.localPosition = new Vector3(-1.0f, 0.0f, 0.0f);
                    _rangedSliceScriptV2.StartAttack?.Invoke(2.5f, 1.0f, -1.0f);
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
                // removed (dont have the sprite)
                /*case "Z22":
                    Debug.Log("Downslam Attack!");
                    // check in air
                    break;*/
                // Call a procedure to handle the rest of rush attack
                case "Z66":
                    Debug.Log("Right Dash Attack!");
                    _spriteRenderer.flipX = false;
                    _rushCoroutine = StartCoroutine(RushAttackProcedure());
                    yield break;
                case "Z44":
                    Debug.Log("Left Dash Attack!");
                    _spriteRenderer.flipX = true;
                    _rushCoroutine = StartCoroutine(RushAttackProcedure());
                    yield break;
                case "Z4268":
                    if (_arenaActive)
                    {
                        Debug.Log("Ultimate!");
                        _playerAnimator.Play("NeroUltimate");
                        yield return new WaitForSeconds(.55f);
                        _hitbox.transform.localPosition = new Vector3(0.0f, 3.0f, 0.0f);
                        _hitbox.transform.localScale = new Vector3(25.0f, 10.0f, 1.0f);
                    }
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
                        if (_moveAction.ReadValue<Vector2>().x == 1)
                        {
                            _spriteRenderer.flipX = false;
                        }
                        else
                        {
                            _spriteRenderer.flipX = true;
                        }
                        // Play Animation
                        _playerAnimator.Play("MiddleJabAttack");
                        /*yield return new WaitForSeconds(.4f);
                        // Place hitbox a bit infront of the player 
                        _hitbox.transform.localPosition = new Vector3(1.55f * _direction, 0.45f, 0.0f);
                        _hitbox.transform.localScale = new Vector3(3.0f, 1.0f, 1.0f);*/
                        StartCoroutine(DashAttackProcedure());
                        yield break;
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
        OnPlayerAttack.Invoke(attackduration, _direction, _knockbackStrength);
        Debug.Log(_attackCombo);
        StartCoroutine(AttackCooldown());
    } 
    private void EndTheAttack()
    {
        // tells code attack is over
        _attackCoroutine = null;
        _movementScript.enabled = true;
        // mainly for the jump attack but it sets the gravity back to normal/initial value
        _playerRigidBody.gravityScale = 2;
    }
    // So player does not spam the attacks
    private IEnumerator AttackCooldown()
    {
        _cooldownOn = true;
        yield return new WaitForSeconds(_attackCooldown);
        _cooldownOn = false;
    }
    #endregion
    #region Special Attack Procedure
    // Code for the dash attack
    private IEnumerator DashAttackProcedure()
    {
        yield return new WaitForSeconds(.4f);
        gameObject.layer = 8;
        _hitbox.transform.localPosition = new Vector3(1.55f * _direction, 0.45f, 0.0f);
        _hitbox.transform.localScale = new Vector3(3.0f, 1.0f, 1.0f);
        _playerRigidBody.AddForceX(_direction * 2000);
        OnPlayerAttack.Invoke(1.0f, _direction, _knockbackStrength);
        yield return new WaitForSeconds(.1f);
        gameObject.layer = 6;
        EndTheAttack();
        StartCoroutine(AttackCooldown());
    }
    // Code for the rush attack
    private IEnumerator RushAttackProcedure()
    {
        _playerAnimator.Play("NeroRushAttack");
        yield return new WaitForSeconds(0.8f);
        gameObject.layer = 8;
        _playerRigidBody.AddForceX(_direction * 1500);
        _rushAttackRangeHitBox.transform.localPosition = new Vector3(_direction * 0.5f, 0.0f, 0.0f);
        _rushAttackRangeHitBox.transform.localScale = new Vector3(1.0f, 1.75f, 1.0f);
        OnPlayerAttack.Invoke(0.6f, _direction, _knockbackStrength);
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(RushAttackEnd());
    }
    private void RushDetectEnemy()
    {
        _playerAnimator.Play("NeroRushAttackEnd");
        StartCoroutine(RushAttackEnd());
    }
    private IEnumerator RushAttackEnd()
    {
        StopCoroutine(_rushCoroutine);
        _rushAttackRangeHitBox.transform.localPosition = new Vector3(0.0f, -10.0f, 0.0f);
        OnPlayerAttack.Invoke(1.0f, _direction, _knockbackStrength);
        _playerRigidBody.linearVelocity = Vector2.zero;
        _hitbox.transform.localPosition = new Vector3(_direction * 1.0f, 0.0f, 0.0f);
        _hitbox.transform.localScale = new Vector3(1.75f, 1.75f, 1.0f);
        gameObject.layer = 6;
        yield return new WaitForSeconds(1.0f);
        EndTheAttack();
        StartCoroutine(AttackCooldown());
    }
    private IEnumerator EndArenaField()
    {
        yield return new WaitForSeconds(45);
        _arenaActive = false;
        _arenaBackground.enabled = false;
        _arenaCooldown = true;
        yield return new WaitForSeconds(120);
        _arenaCooldown = false;
    }
    #endregion
}
