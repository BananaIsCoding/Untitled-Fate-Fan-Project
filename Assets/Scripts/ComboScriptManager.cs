using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ComboScriptManager : MonoBehaviour
{
    public UnityEvent HitComboCounterEvent;
    private int _currentHitCombo;
    private Coroutine _counterCoroutine;
    [SerializeField] private Canvas _hitComboCanvas;
    [SerializeField] private TextMeshProUGUI _hitComboText;
    [SerializeField] private ScoreManagerScript _scoreManagerScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HitComboCounterEvent.AddListener(AddToCounter);
    }
    private void AddToCounter()
    {
        _currentHitCombo += 1;
        _hitComboText.text = _currentHitCombo.ToString();
        _hitComboCanvas.enabled = true;
        if (_currentHitCombo%5 == 0)
        {
            _scoreManagerScript.AddScoreEvent?.Invoke(100);
        }
        if (_counterCoroutine != null)
        {
            StopCoroutine(_counterCoroutine);
        }
        _counterCoroutine = StartCoroutine(ResetCounter());
    }
    IEnumerator ResetCounter()
    {
        yield return new WaitForSeconds(3.0f);
        _hitComboCanvas.enabled = false;
        _currentHitCombo = 0;
    }
}
