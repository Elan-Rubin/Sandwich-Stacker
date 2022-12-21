using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class LoadingScreen : MonoBehaviour
{
    int _loadingProgress;
    [SerializeField] private RectTransform _loadingStack;
    private float _currentY, _targetY;
    [SerializeField] private TextMeshProUGUI _loadingText, _loadingPercentage;
    void Start()
    {
        _currentY = _targetY = -160;
        StartCoroutine(nameof(Loading));
        StartCoroutine(nameof(DotDotDot));
    }
    private void Update()
    {
        _targetY = (_loadingProgress / 100f) * 160 - 160f;
        _currentY = Mathf.Lerp(_currentY, _targetY, Time.deltaTime * 10f);
        _loadingStack.anchoredPosition = new Vector2(0, _currentY);
    }
    private IEnumerator Loading()
    {
        while (_loadingProgress < 100)
        {
            yield return new WaitForSeconds(Random.Range(0.15f, 0.35f));
            _loadingProgress += Random.Range(0, 10);
            if (_loadingProgress > 100) _loadingProgress = 100;
            _loadingPercentage.text = $"{_loadingProgress} / 100";
        }
        yield return new WaitForSeconds(1f);
        GetComponent<CanvasGroup>().DOFade(0, 1f).OnComplete(() => gameObject.SetActive(false));
    }
    private IEnumerator DotDotDot()
    {
        _loadingText.text = "Loading";
        yield return new WaitForSeconds(0.15f);
        _loadingText.text = "Loading.";
        yield return new WaitForSeconds(0.15f);
        _loadingText.text = "Loading..";
        yield return new WaitForSeconds(0.15f);
        _loadingText.text = "Loading...";
        yield return new WaitForSeconds(0.15f);
        if(_loadingProgress < 100) StartCoroutine(nameof(DotDotDot));
    }
}
