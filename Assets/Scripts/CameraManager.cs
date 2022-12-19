using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static CameraManager Instance { get { return _instance; } }
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

    [SerializeField] private Vector3 _currentPosition;
    public Vector3 CurrentPosition { get { return _currentPosition; } }
    public Vector3 TargetPosition { get; set; }

    void Start()
    {
        _currentPosition = TargetPosition = transform.position;
    }

    void Update()
    {
        if (Vector2.Distance(_currentPosition, TargetPosition) < 0.01f)
        {
            _currentPosition = TargetPosition;
            return;
        }
        else _currentPosition = transform.position = Vector3.Lerp(CurrentPosition, TargetPosition, Time.deltaTime);
    }
}
