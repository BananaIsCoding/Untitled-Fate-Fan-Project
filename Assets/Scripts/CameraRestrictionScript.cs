using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRestrictionScript : MonoBehaviour
{
    // variables
    [SerializeField] Vector3 _presetCamPos;
    [SerializeField] Vector3 _presetLocalPos;
    private GameObject _camareObject;
    private GameObject _player;
    private Vector3 _cameraPos;
    private MovementScript _playerMovementScript;
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
            _camareObject.transform.parent = transform.parent;
            _camareObject.transform.position = _presetCamPos;
            // set up procedure to be called to relink the camera when player jumps
            if (_player.transform.position.y > 2.904933 + 0.01f)
            {
                _cameraPos = _presetCamPos;
                StartCoroutine(MoveCameraUp());
                // function which set camera back onto the ground
                _playerMovementScript.JumpEndedEvent += UnBindCamera;
            }
            else
            {
                _playerMovementScript.JumpEvent += BindCameraWhenJump;
            }
        }
    }
    // when player leaves the edge of the map then rebind camera
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // and remove function link as it no longer needs to rebind each jump
            _playerMovementScript.JumpEvent -= BindCameraWhenJump;
            _playerMovementScript.JumpEndedEvent -= UnBindCamera;
            StopAllCoroutines();
            // set the camera back to follow player position
            _camareObject.transform.parent = _player.transform;
            _camareObject.transform.localPosition = _presetLocalPos;
        }
    }
    private void BindCameraWhenJump()
    {
        // So this can't be called muiltiple times
        _playerMovementScript.JumpEvent -= BindCameraWhenJump;
        _cameraPos = _presetCamPos;
        StartCoroutine(MoveCameraUp());
        // function which set camera back onto the ground
        _playerMovementScript.JumpEndedEvent += UnBindCamera;
    }
    IEnumerator MoveCameraUp()
    {
        // moves cam up as based on how high player is in the air
        while (true)
        {
            yield return new WaitForFixedUpdate();
            _cameraPos.y = _player.transform.position.y + 0.6f;
            _camareObject.transform.position = _cameraPos;
        }
    }
    private void UnBindCamera()
    {
        _playerMovementScript.JumpEndedEvent -= UnBindCamera;
        StopAllCoroutines();
        // set camera back to ground level
        _camareObject.transform.position = _presetCamPos;
        _playerMovementScript.JumpEvent += BindCameraWhenJump;
    }
}