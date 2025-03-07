using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    private void Start()
    {
        InputSystem.actions.FindAction("Pause").started += PauseKeyPressed;
    }
    private void PauseKeyPressed(InputAction.CallbackContext context)
    {
        if (Time.timeScale > 0)
        {
            PauseGame();
        }
        else
        {
            Time.timeScale = 1.0f;
            SceneManager.UnloadSceneAsync(2);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }
}
