using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRestrictionScript : MonoBehaviour
{
    // variables
    private GameObject _camareObject;
    private GameObject _player;
    private Vector3 _cameraPos;
    private float _initialY;
    private MovementScript _playerMovementScript;
    private InputAction _jumpAction;
    private Coroutine _jumpCoroutine;
    private void Start()
    {
        _jumpAction = InputSystem.actions.FindAction("Jump");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // getting the required stuff when it is first encounter the player
            if (_camareObject == null)
            {
                _player = collision.gameObject;
                _camareObject = _player.GetComponentInChildren<Camera>().gameObject;
                _playerMovementScript = _player.GetComponent<MovementScript>();
            }
            // unlinks the camera from char
            _camareObject.transform.parent = transform;
            // set up procedure to be called to relink the camera when player jumps
            _jumpAction.performed += BindCameraWhenJump;
            // set up procedure to be called to detach the camera when player lands
            _playerMovementScript.JumpEndedEvent += UnBindCamera;
        }
    }
    // when player leaves the edge of the map then rebind camera
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _camareObject.transform.parent = _player.transform;
            // and remove function link as it no longer needs to rebind each jump
            _jumpAction.performed -= BindCameraWhenJump;
            _playerMovementScript.JumpEndedEvent -= UnBindCamera;
            StopAllCoroutines();
        }
    }
    private void BindCameraWhenJump(InputAction.CallbackContext context)
    {
        _initialY = _camareObject.transform.position.y;
        // caluate where char is from the camera so that when it is relinked to char, the camera does re-position itself based on where player is
        _cameraPos = _camareObject.transform.position;
        StartCoroutine(MoveCameraUp());
    }
    IEnumerator MoveCameraUp()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            _cameraPos.y = _player.transform.position.y + 0.6f;
            _camareObject.transform.position = _cameraPos;

        }
    }
    private void UnBindCamera()
    {
        StopCoroutine(MoveCameraUp());
        _cameraPos.y = _initialY;
        _camareObject.transform.position = _cameraPos;
    }
}
