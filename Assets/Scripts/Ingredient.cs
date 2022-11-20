using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        _timer += Time.deltaTime;
        if (_timer > 0.1f)
        {
            if (_counter == _fallingSprites.Count) _counter = 0;
            _fallingObject.GetComponent<SpriteRenderer>().sprite = _fallingSprites[_counter];
            _timer = 0;
            _counter++;
        }
        if (_landed)
        {
            /* wobbling states depending on player movement:
             * player is not moving - ingredients wobble back and forth based on the sine wave 
             * player is moving - the ingredients wobble towards a fixed rotation, facing towards movement
             * igredients above will lerp to the lower ingredient's rotation and position
             */



            _sinVal += Time.deltaTime;
            //transform.localRotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Sin(_sinVal) * Mathf.Clamp(_height / 2f, 1, 7.5f)));

            //_targetPosition = new Vector2(_ingredientBelow.transform.position.x, -3f + (.2f *(_height + 1)));
            //transform.position = _currentPosition = Vector2.Lerp(_currentPosition, _targetPosition, Time.deltaTime * (30f - _height));


            //bool direction = (_targetPosition.x - _currentPosition.x) > 0;
            //if (Vector2.Distance(_targetPosition, _currentPosition) > 0.1f)
            //    _targetRotation = 3f * (_height / 8f) * (direction ? -1 : 1);
            //else _targetRotation = 0;

            if (_ingredientBelow == null)
            {
                _targetRotation = Mathf.Sin(_sinVal) * 25f;
                _targetPosition = Vector3.zero + (25f * Mathf.Sin(_sinVal) * Vector3.right);
            }
            else
            {
                _targetRotation = _ingredientBelow.transform.localRotation.z;
                _targetPosition = _ingredientBelow.transform.localPosition;
            }

            transform.localPosition = _currentPosition = Vector2.Lerp(_currentPosition, _targetPosition, Time.deltaTime * 30f);
            _currentRotation = Mathf.Lerp(_currentRotation, _targetRotation, Time.deltaTime * 30f);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, _currentRotation));
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_landed) return;
        if (collision.gameObject.tag.Equals("Sandwich"))
        {
            if (_badItem)
            {
                gameObject.SetActive(false);
                GameManager.Instance.IckyItemCaught();
                return;
            }
            GameManager.Instance.SandwichSize++;

            _fallingObject.SetActive(false);
            _landedObject.SetActive(true);

            //transform.localPosition -= new Vector3(0, .5f);
            _landed = true;

            Destroy(collision.gameObject.GetComponent<Rigidbody2D>());
            foreach (Transform child in collision.transform)
                Destroy(child.GetComponent<Collider2D>());
            Destroy(collision.gameObject.GetComponent<Collider2D>());
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collision.gameObject.tag = "Untagged";
            gameObject.tag = "Sandwich";

            _height = collision.gameObject.GetComponent<Ingredient>() ? collision.gameObject.GetComponent<Ingredient>().Height + 1 : 1;

            _ingredientBelow = collision.gameObject;
            transform.parent = _height < 2 ? collision.transform.parent : collision.transform;
            transform.position = new Vector2(_ingredientBelow.transform.position.x, -3f + (.2f * (_height + 2)));
            //_currentRotation = 0;
            //_currentPosition = transform.position;

            if (_height > 7) CameraManager.Instance._targetPosition += new Vector3(0, 0.2f, 0);

            if (IngredientType.Equals(IngredientType.Bread))
                GameManager.Instance.EndRound();
        }
        else if (collision.gameObject.tag.Equals("Border"))
        {
            //if (!_badItem) Explode();

            Destroy(this.gameObject);
        }
    }

    public void Explode()
    {
        ParticleSystemRenderer foodParticles = Instantiate(_foodParticles, transform.position, Quaternion.identity).GetComponent<ParticleSystemRenderer>();
        _foodParticlesMaterial.mainTexture = HelperClass.TextureFromSprite(_fallingSprites[0]);
        foodParticles.material = _foodParticlesMaterial;
        Destroy(foodParticles.gameObject, 2.5f);
    }
}

public enum IngredientType
{
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

    Boot,
    Can,
    Goop,
    Eye,
    Fish,
    Mushroom,
}
