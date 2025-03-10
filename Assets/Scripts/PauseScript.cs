using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    private void Start()
    {
        // linking function to the pause button
        InputSystem.actions.FindAction("Pause").started += PauseKeyPressed;
    }
    // when pause button is pressed
    private void PauseKeyPressed(InputAction.CallbackContext context)
    {
        // check if time is not stopped
        if (Time.timeScale > 0)
        {
            // if so then pause time
            PauseGame();
        }
        else
        {
            // else resume time and get rid of pause menu
            Time.timeScale = 1.0f;
            SceneManager.UnloadSceneAsync(2);
        }
    }
    /// <summary>
    /// This procedures pauses the time
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0;
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }
}
