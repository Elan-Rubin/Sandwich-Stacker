using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    [Header("Build details")]
    [Space(5)]
    [SerializeField] private bool _NGBuild = false;
    public bool NGBuild { get { return _NGBuild; } }
    [Space(5)]

    [Header("Game State Details")]
    [Space(5)]
    [SerializeField] private int _round = 1;
    public int Round { get { return _round; } }
    [SerializeField] private int _score = 0;
    public int Score { get { return _score; } }

    public int SandwichSize;
    [Space(5)]

    [Header("Game Panels")]
    [Space(5)]
    [SerializeField] private Button _playButton;
    [SerializeField] private Sprite _play, _pause;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [Space(5)]

    [Header("Icky UI")]
    [Space(5)]
    [SerializeField] private Slider _ickMeter;
    [SerializeField] private GameObject _ickyOverlay;
    private int _ickCounter = 0;
    [Space(5)]

    [Header("Ingredient Spawner")]
    [Space(5)]
    [SerializeField] private IngredientSpawner _ingredientSpawner;
    [Space(5)]

    [Header("Start Round")]
    [Space(5)]
    [SerializeField] private TextMeshProUGUI _levelText;
    [Space(5)]

    [Header("End Round")]
    [Space(5)]
    [SerializeField] private GameObject _pointsAdded;
    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _sceneTransition;
    [SerializeField] private TextMeshProUGUI _flavorText1, _flavorText2, _levelText2, _scoreText2;
    [SerializeField] private List<string> _flavorTexts1 = new List<string>();
    [SerializeField] private List<string> _flavorTexts2 = new List<string>();
    private int[] _scores = new int[] { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 75, 100, 150, 200, 250, 300, 350, 400, 500 };

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    bool _paused, _pausing;
    public bool Paused { get { return _paused; } }
    void Start()
    {
        StartRound();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) PauseGame();
        if (Input.GetKeyDown(KeyCode.W)) EndRound();
    }

    public int IckyItemCaught()
    {
        StartCoroutine(nameof(IckyOverlay));

        _ickCounter++;
        _ickMeter.value = _ickCounter;
        Image ickMeterImage = _ickMeter.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        Color previousColor = ickMeterImage.color;
        ickMeterImage.DOColor(Color.white, 0.1f).SetEase(Ease.InExpo).OnComplete(() =>
        {
            ickMeterImage.DOColor(previousColor, 0.1f).SetEase(Ease.OutExpo);
        });
        CameraManager.Instance.transform.DOShakePosition(0.2f, 0.5f, 25);
        return _ickCounter;
    }
    private IEnumerator IckyOverlay()
    {
        _ickyOverlay.SetActive(true);
        yield return new WaitForSeconds(1f);
        _ickyOverlay.SetActive(false);
    }

    public void StartRound()
    {
        _levelText.text = "Level " + _round;
        StartCoroutine(nameof(StartRoundCoroutine));
    }

    private IEnumerator StartRoundCoroutine()
    {
        yield return null;
        _levelText.gameObject.SetActive(true);
        _levelText.transform.localScale = Vector3.zero;
        var previousColor = _levelText.color;
        var levelTextSequence = DOTween.Sequence()
            .Append(_levelText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InExpo))
            .AppendInterval(1f)
            .Append(_levelText.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutExpo))
            .Join(_levelText.transform.DORotate(Vector3.forward * 200 * (Random.value > 0.5f ? 1 : -1), 0.75f).SetEase(Ease.InExpo))
            .OnComplete(() =>
            {
                _levelText.color = previousColor;
                _levelText.transform.rotation = Quaternion.Euler(Vector3.zero);
                _levelText.gameObject.SetActive(false);
            });
    }

    public void EndRound()
    {
        _round++;
        _ingredientSpawner.Toggle();
        StartCoroutine(nameof(EndRoundCoroutine));
    }

    private IEnumerator EndRoundCoroutine()
    {
        Transform ingredient = Player.Instance.transform;
        while (ingredient.childCount > 2)
            ingredient = ingredient.GetChild(2);

        yield return new WaitForSeconds(0.5f);

        int ingredientCounter = 0;
        do
        {
            ingredientCounter++;
            //int scoreAddition = (int)(_round * Mathf.Pow(2, ingredientCounter));
            //int scoreAddition = (int)(.5f * Mathf.Pow(2, ingredientCounter) + _round * ingredientCounter);
            //scoreAddition = (int)Mathf.Round(scoreAddition / 5.0f) * 5;
            int scoreAddition = _scores[Mathf.Clamp(ingredientCounter, 0, _scores.Length - 1)] * (_round - 1);
            _scoreText.text = (_score += scoreAddition).ToString();
            _scoreText.transform.DOPunchScale(Vector3.one / 10f, 0.1f);

            var pointsAdded = Instantiate(_pointsAdded, ingredient.transform.position + Vector3.left * (ingredientCounter % 2 != 0 ? -1 : 1), Quaternion.identity);
            pointsAdded.transform.Rotate(Vector3.forward * Random.Range(-25, 25));
            var pointsAddedText = pointsAdded.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            pointsAddedText.color = Color.clear;
            pointsAddedText.text = "+" + scoreAddition;
            var sequence = DOTween.Sequence()
                .Append(pointsAddedText.DOColor(Color.white, 0.5f))
                .AppendInterval(0.75f)
                .Append(pointsAddedText.DOColor(Color.clear, 0.5f).OnComplete(() =>
                {
                    Destroy(pointsAddedText.transform.parent.gameObject);
                }));

            ingredient.GetComponent<Ingredient>().Explode();
            var temp = ingredient.parent;
            Destroy(ingredient.gameObject);
            ingredient = temp;
            yield return new WaitForSeconds(0.25f);
        } while (ingredient.parent != null);

        yield return new WaitForSeconds(0.25f);
        _sceneTransition.SetActive(true);
        yield return new WaitForSeconds(1f);
        _winScreen.SetActive(true);
        _flavorText1.text = "";
        _flavorText2.text = "";
        _scoreText2.text = "Score: " + _score;
        _levelText2.text = "Level " + (_round - 1);
        _flavorText1.text = _flavorTexts1[Random.Range(0, _flavorTexts1.Count)];
        _flavorText2.text = _flavorTexts2[Random.Range(0, _flavorTexts2.Count)];

        yield return new WaitForSeconds(1f);
        _sceneTransition.SetActive(false);
    }

    public void PauseGame()
    {
        if (_pausing) return;
        _paused = !_paused;
        _playButton.image.sprite = _paused ? _play : _pause;
        StartCoroutine(nameof(ChangeTimeScale), _paused ? 0 : 1);
    }

    private IEnumerator ChangeTimeScale(float value)
    {
        _pausing = true;
        while (Mathf.Abs(value - Time.timeScale) > 0.01f)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, value, 0.05f);
            yield return null;
        }
        Time.timeScale = value;
        _pausing = false;
    }
}
