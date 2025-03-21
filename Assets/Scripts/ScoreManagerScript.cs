using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManagerScript : MonoBehaviour
{
    public UnityEvent<int> AddScoreEvent;
    public int _score;
    public int _timer;
    [SerializeField] private TextMeshProUGUI _scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddScoreEvent.AddListener(AddScore);
        StartCoroutine(StartPassiveScoreGain());
    }
    private IEnumerator StartPassiveScoreGain()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            _timer += 1;
            _score += 1;
            _scoreText.text = _score.ToString();
        }
    }
    private void AddScore(int scoreToAdd)
    {
        _score += scoreToAdd;
        _scoreText.text = _score.ToString();
    }
}