using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    private static Player _instance;
    public static Player Instance { get { return _instance; } }

    [Header("Player Movement")]
    [Range(10, 20)]
    [Tooltip("How fast the player moves horizontally.")]
    [SerializeField] private float _movementSpeed = 10f;
    [Tooltip("The absolute value of the maximum x-position.")]
    [SerializeField] private float _maximumX = 6.43f;
    [SerializeField] private float _minimumX = -5.11f;

    [Header("Player Visuals")]
    [Tooltip("Frames of the player's walk animation")]
    [SerializeField] private List<Sprite> _walkCycle = new();
    [SerializeField] private List<Sprite> _pausedCycle = new();
    int _counter;
    float _timer,_timer2;

    private GameObject _visual;
    private SpriteRenderer _renderer;

    [HideInInspector] public float MomentumMultiplier, Momentum;

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

    void Start()
    {
        _visual = transform.GetChild(0).gameObject;
        _renderer = _visual.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!GameManager.Instance.InRound) return;
        if (MomentumMultiplier > 0) MomentumMultiplier -= Time.deltaTime;
        else if (MomentumMultiplier < 0) MomentumMultiplier += Time.deltaTime;
        bool paused = GameManager.Instance.Paused;
        List<Sprite> currentCycle = !paused ? _walkCycle : _pausedCycle;
        if (_timer > 0.1f)
        {
            _timer = 0;
            _counter++;
            if (_counter == currentCycle.Count) _counter = 0;
            _renderer.sprite = currentCycle[_counter];
        }
        Vector2 movementOffset = new(Input.GetAxisRaw(axisName: "Horizontal"), 0);
        movementOffset *= Time.deltaTime * _movementSpeed;
        bool direction = movementOffset.x > 0; //right = true, left = false
        _timer2 += Time.deltaTime;
        if (movementOffset.magnitude > 0)
        {
            MomentumMultiplier = direction ? 1 : -1;
        }

        if ((transform.position.x > _maximumX && direction) || (transform.position.x < _minimumX && !direction))
        {
            _renderer.sprite = currentCycle[_counter = 0];
            return;
        }
        transform.Translate(movementOffset);

        if (movementOffset.magnitude > 0 && !paused)
        {
            _timer += Time.deltaTime;
        }
        else if (paused) _timer += 0.001f;
        else _renderer.sprite = currentCycle[_counter = 0];
    }

    public void ResetPlayer()
    {
        var plate = transform.GetChild(1).gameObject;
        plate.GetComponent<Collider2D>().enabled = true;
        plate.tag = "Sandwich";
    }
}
