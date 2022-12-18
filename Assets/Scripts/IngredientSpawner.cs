using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    [SerializeField] GameObject _ingredient;
    private bool _spawning = false;
    float _timer = -1f;
    int _ingredientCounter;

    [SerializeField] private List<IngredientScriptableObject> _ingredients = new List<IngredientScriptableObject>();
    [SerializeField] private List<IngredientScriptableObject> _badIngredients = new List<IngredientScriptableObject>();
    [SerializeField] private IngredientScriptableObject _topBread;

    [SerializeField] private GameObject _breadLauncher;

    void Start()
    {

    }

    public bool Toggle()
    {
        return _spawning = !_spawning;
    }

    private bool _waiting = false;
    void Update()
    {
        if(GameManager.Instance.InRound && _spawning) _timer += Time.deltaTime;
        if (_timer > 1f && _spawning)
        {
            _ingredientCounter++;
            _timer = 0;
            Ingredient newIngredient = Instantiate(_ingredient, new Vector3(Random.Range(-5.5f, 4f), transform.position.y), Quaternion.identity).GetComponent<Ingredient>();
            var ingredientValues = (Random.Range(0f, 1f) > 0.2f)
                ? _ingredients[Random.Range(0, _ingredients.Count)]
                : _badIngredients[Random.Range(0, _badIngredients.Count)];
            if (GameManager.Instance.SandwichSize > 5 && _ingredientCounter % 5 == 0)//this number will change
            {
                StartCoroutine(nameof(BreadLaunch));
                ingredientValues = _topBread;
                _timer = -2.5f;
            }
            newIngredient.AssignIngredientValues(ingredientValues);
        }
    }

    private IEnumerator BreadLaunch()
    {
        _breadLauncher.SetActive(_waiting = true);
        yield return new WaitForSeconds(2.2f);
        _breadLauncher.SetActive(_waiting = false);
    }
}
