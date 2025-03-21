using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _survivalTimeText;
    [SerializeField] private TextMeshProUGUI _totalScoreText;
    private ScoreManagerScript _scoreScript;
    private bool _BTMMButtonPressed;
    private bool _retryPressed;
    void Start()
    {
        _scoreScript = FindAnyObjectByType<ScoreManagerScript>();
        _survivalTimeText.text = _scoreScript._timer.ToString() + " Seconds";
        _totalScoreText.text = _scoreScript._score.ToString();
        StartCoroutine(PreloadScene());
    }
    public void BackToMainMenu()
    {
        _BTMMButtonPressed = true;
    }
    public void Retry()
    {
        _retryPressed = true;
    }
    private IEnumerator PreloadScene()
    {
        AsyncOperation mainMenuScene = SceneManager.LoadSceneAsync(0);
        mainMenuScene.allowSceneActivation = false;
        AsyncOperation InfiniteModeScene = SceneManager.LoadSceneAsync(3);
        InfiniteModeScene.allowSceneActivation = false;
        while (true)
        {
            if(_BTMMButtonPressed)
            {
                mainMenuScene.allowSceneActivation = true;
                Time.timeScale = 1.0f;
            }
            else if (_retryPressed)
            {
                InfiniteModeScene.allowSceneActivation = true;
                Time.timeScale = 1.0f;
            }
            yield return null;
        }
    }
}
