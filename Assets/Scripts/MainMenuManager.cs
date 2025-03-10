using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // variables
    [Header("Canvas")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas modeSelectCanvas;
    [SerializeField] private Canvas settingCanvas;
    [SerializeField] private Canvas controlCanvas;
    // when certain buttons are pressed
    #region Buttons function
    public void StartButton()
    {
        mainCanvas.enabled = false;
        modeSelectCanvas.enabled = true;
    }
    public void CloseModeSelection()
    {
        modeSelectCanvas.enabled = false;
        mainCanvas.enabled = true;
    }
    public void WhichModeSelected(int modeID)
    {
        // 1 should be the loading scene
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        // the id 1to1 is in the built profile
        //if Scenes are reorganised, may have to change value buttons passed to this function
        SceneManager.LoadSceneAsync(modeID);
    }
    public void SettingsButton()
    {
        mainCanvas.enabled = false;
        settingCanvas.enabled = true;
    }
    public void SettingsExitButton()
    {
        settingCanvas.enabled = false;
        mainCanvas.enabled = true;
    }
    public void ExitButton()
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
