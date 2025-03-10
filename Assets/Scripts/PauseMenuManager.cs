using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    // variables
    [Header("Canvas")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas controlCanvas;
    // when certain buttons are pressed
    #region Buttons function
    public void ResumeButton() 
    {
        Time.timeScale = 1.0f;
        SceneManager.UnloadSceneAsync(2);
    }
    public void SettingsButton()
    {
        //mainCanvas.enabled = false;
    }
    /*public void CloseSettingsButton()
    {
        //mainCanvas.enabled = true;
    }*/
    public void BackToMainMenuButton()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        // 0 should be main menu
        SceneManager.LoadSceneAsync(0);
    }
    public void QuitButton()
    {
        Application.Quit();
    }
    public void HelpButton()
    {
        mainCanvas.enabled = false;
        controlCanvas.enabled = true;
    }
    public void HelpExitButton()
    {
        controlCanvas.enabled = false;
        mainCanvas.enabled = true;
    }
    #endregion
}
