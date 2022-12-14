using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientManager : MonoBehaviour
{
    private static IngredientManager _instance;

    public static IngredientManager Instance { get { return _instance; } }
    private float _timer = 0;
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
        _timer += Time.deltaTime;
        var i = 0;
        Vector2 ingredientBeforePos = Vector2.zero;
        foreach (var ingredient in _sandwich)
        {
            //ingredient.transform.position = new Vector2(pos.x + Player.Instance.Momentum * i / 3f, ingredient.Height * 0.2f - 2.55f);

            //ingredient.TargetPosition = new Vector2(pos.x + Player.Instance.MomentumMultiplier * i / 3f, ingredient.Height * 0.2f - 2.55f);

            if (i == 0) ingredientBeforePos = ingredient.TargetPosition = new Vector2(pos.x, ingredient.Height * 0.2f - 2.55f);
            else ingredientBeforePos = ingredient.TargetPosition = new Vector2(Vector2.Lerp(ingredient.CurrentPosition, ingredientBeforePos, 0.90f).x, ingredient.Height * 0.2f - 2.55f);

            ingredient.TargetPosition += Vector2.right * 2f* Player.Instance.Momentum / (50f - Mathf.Clamp(i, 0, 49));
            if (i == 0) ingredient.CurrentPosition = ingredient.transform.position = Vector2.Lerp(ingredient.CurrentPosition, new Vector2(pos.x, ingredient.Height * 0.2f - 2.55f), Time.deltaTime * 40f);
            float lerpMultiplier = 0;
            if (ingredient.Height > 30)
                lerpMultiplier = 10f - .5f * (Mathf.Abs(30 - ingredient.Height));
            else lerpMultiplier = Mathf.Clamp(40f - ingredient.Height, 10, 40);
            if (i != 0) ingredient.CurrentPosition = ingredient.transform.position = Vector2.Lerp(ingredient.CurrentPosition, ingredient.TargetPosition, Time.deltaTime * lerpMultiplier);

            i++;
        }
    }
}
