using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackScript : MonoBehaviour
{
    
    private bool _moveAttacking;
    private void Start()
    {
        InputSystem.actions.FindAction("Attack").performed += DefaultAttack;
        InputSystem.actions.FindAction("MoveAttack").performed += MoveAttack;
    }
    private void DefaultAttack(InputAction.CallbackContext context)
    {
        StartCoroutine(PerformAttack());
    }
    private void MoveAttack(InputAction.CallbackContext context)
    {
        _moveAttacking = true;
    }
    private IEnumerator PerformAttack()
    {
        yield return new WaitForSeconds(.1f);
        if (_moveAttacking)
        {
            Debug.Log("Move Attack!");
        }
        else
        {
            Debug.Log("Attack!");
        }
        _moveAttacking = false;
    } 
}
