using System;
using System.Collections;
using UnityEngine;

public class PlayerStunSystem : MonoBehaviour
{
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
        _movementScript.enabled = false;
        _playerAttackScript.enabled = false;
        _stunTime = time;
        if (_stunCoroutine != null)
        {
            StopCoroutine(_stunCoroutine);
        }
        _stunCoroutine = StartCoroutine(Stun());
    }
    private IEnumerator Stun()
    {
        yield return new WaitForSeconds(_stunTime);
        _movementScript.enabled = false;
        _playerAttackScript.enabled = false;
    }
}
