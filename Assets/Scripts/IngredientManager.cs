using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientManager : MonoBehaviour
{
    private static IngredientManager _instance;

    public static IngredientManager Instance { get { return _instance; } }
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

    List<Ingredient> _sandwich = new();
    public List<Ingredient> Sandwich { get { return _sandwich; } }
    Plate _plate;

    void Start()
    {
        _plate = Plate.Instance;
    }

    void Update()
    {
        if (!GameManager.Instance.InRound) return;
        Vector2 pos = _plate.PlatePosition;
        foreach (var ingredient in _sandwich)
        {
            ingredient.transform.position = new Vector2(pos.x, ingredient.Height * 0.2f - 2.55f);
        }
    }
}
