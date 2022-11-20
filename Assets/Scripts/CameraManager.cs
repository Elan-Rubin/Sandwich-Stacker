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

    public Vector3 _currentPosition, _targetPosition;

    void Start()
    {
        _currentPosition = _targetPosition = transform.position;
    }

    void Update()
    {
        _currentPosition = transform.position = Vector3.Lerp(_currentPosition, _targetPosition, Time.deltaTime);
    }
}
