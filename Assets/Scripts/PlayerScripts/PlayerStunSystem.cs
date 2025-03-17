using System;
using System.Collections;
using UnityEngine;

public class PlayerStunSystem : MonoBehaviour
{
    // variables
    public event Action<float> StunEvent;
    private MovementScript _movementScript;
    private PlayerAttackScript _playerAttackScript;
    private float _stunTime;
    private Coroutine _stunCoroutine;
    void Start()
    {
        _movementScript = GetComponent<MovementScript>();
        _playerAttackScript = GetComponent<PlayerAttackScript>();
        StunEvent += StartStun;
    }
    private void StartStun(float time) 
    {
        // if there is alr a stun in place then stop it
        if (_stunCoroutine != null)
        {
            StopCoroutine(_stunCoroutine);
        }
        // disable player movement and attack controls
        _movementScript.enabled = false;
        _playerAttackScript.enabled = false;
        _stunTime = time;
        // start new countdown till the stun is removed
        _stunCoroutine = StartCoroutine(Stun());
    }
    private IEnumerator Stun()
    {
        yield return new WaitForSeconds(_stunTime);
        // allow player to move and attack again
        _movementScript.enabled = true;
        _playerAttackScript.enabled = true;
    }
}
