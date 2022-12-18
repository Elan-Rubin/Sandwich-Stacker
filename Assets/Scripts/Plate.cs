using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    private static Plate _instance;
    public static Plate Instance { get { return _instance; } }
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
    Vector2 _platePosition = new();
    public Vector2 PlatePosition { get { return _platePosition; } }
    void Start()
    {
        _platePosition = transform.position;
    }

    void Update()
    {
        _platePosition = transform.position;
    }
}
