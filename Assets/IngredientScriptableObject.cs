using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ingredient Values")]
public class IngredientScriptableObject : ScriptableObject
{
    public bool BadItem;
    public IngredientType IngredientType;
    public Sprite LandedSprite;
    public List<Sprite> FallingSprites;
}
