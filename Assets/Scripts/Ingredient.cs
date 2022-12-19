using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ingredient : MonoBehaviour
{
    [HideInInspector] public IngredientType IngredientType { get; private set; }
    private Sprite _landedSprite;
    private List<Sprite> _fallingSprites;
    [SerializeField] private GameObject _landedObject, _fallingObject;
    [SerializeField] private GameObject _foodParticles;
    [SerializeField] private Material _foodParticlesMaterial;
    bool _badItem;
    float _timer;
    int _counter;
    float _sinVal;

    private bool _landed;
    private GameObject _ingredientBelow;
    private Vector2 _currentPosition, _targetPosition;
    private float _currentRotation, _targetRotation; //z rotation value
    private int _height;
    public int Height { get { return _height; } }

    void Start()
    {
        if (IngredientType.Equals(IngredientType.Bread)) GameManager.Instance.StallBread(gameObject);
    }

    public void AssignIngredientValues(IngredientScriptableObject ingredientValues)
    {
        IngredientType = ingredientValues.IngredientType;
        _landedSprite = ingredientValues.LandedSprite;
        _fallingSprites = ingredientValues.FallingSprites;
        _badItem = ingredientValues.BadItem;
        gameObject.name = ingredientValues.IngredientType.ToString();
        _landedObject.GetComponent<SpriteRenderer>().sprite = _landedSprite;
        if (Random.Range(0f, 1f) > 0.5f) _fallingObject.GetComponent<SpriteRenderer>().flipX = _landedObject.GetComponent<SpriteRenderer>().flipX = true;
    }

    void Update()
    {
        if (!_landed)
        {
            _timer += Time.deltaTime;
            if (_timer > 0.1f)
            {
                if (_counter == _fallingSprites.Count) _counter = 0;
                _fallingObject.GetComponent<SpriteRenderer>().sprite = _fallingSprites[_counter];
                _timer = 0;
                _counter++;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_landed) return;
        if (collision.gameObject.CompareTag("Sandwich"))
        {
            if (_badItem)
            {
                gameObject.SetActive(false);
                GameManager.Instance.IckyItemCaught();
                return;
            }
            transform.DOPunchScale(new Vector2(0.5f, 0f), 0.25f);
            GameManager.Instance.SandwichSize++;
            IngredientManager.Instance.Sandwich.Add(this);
            transform.parent = IngredientManager.Instance.transform;

            _fallingObject.SetActive(false);
            _landedObject.SetActive(true);
            _landed = true;

            //collision.gameObject.GetComponent<Rigidbody2D>().enabled = false; //this doesnt work
            foreach (Transform child in collision.transform)
                child.GetComponent<Collider2D>().enabled = false;
            if(collision.gameObject.GetComponent<Collider2D>()) collision.gameObject.GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collision.gameObject.tag = "Untagged";
            gameObject.tag = "Sandwich";

            _height = collision.gameObject.GetComponent<Ingredient>() ? collision.gameObject.GetComponent<Ingredient>().Height + 1 : 1;

            transform.position = new Vector2(collision.transform.position.x, collision.transform.position.y + .2f);

            _ingredientBelow = collision.gameObject;
            
            //transform.localPosition = new Vector2(0, -1f + .2f * _height);
            //_currentRotation = 0;
            //_currentPosition = transform.position;

            if (_height > 7) CameraManager.Instance.TargetPosition += new Vector3(0, 0.2f, 0);

            if (IngredientType.Equals(IngredientType.Bread))
                GameManager.Instance.EndRound();
        }
        else if (collision.gameObject.CompareTag("Border"))
        {
            if (!_badItem) Explode();
            Destroy(gameObject);
        }
    }

    public void Explode()
    {
        var foodParticles = Instantiate(_foodParticles, transform.position, Quaternion.identity).GetComponent<ParticleSystemRenderer>();
        _foodParticlesMaterial.mainTexture = HelperClass.TextureFromSprite(_fallingSprites[0]);
        foodParticles.material = _foodParticlesMaterial;
        Destroy(foodParticles.gameObject, 2.5f);
    }
}

public enum IngredientType
{
    //normal ingredients
    Bacon,
    Bread,
    Cheese,
    Lettuce,
    Mayo,
    Meat,
    Mustard,
    Onion,
    Pepper,
    Pickles,
    Tomato,
    //icky ingredients
    Boot,
    Can,
    Goop,
    Eye,
    Fish,
    Mushroom,
}
