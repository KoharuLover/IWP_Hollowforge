using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float _lifeTime = 3f;

    private float _speed;
    private float _damage;
    private ElementType _elementType;
    private Vector2 _direction;
    private GameObject _impactVFXPrefab;

    private bool _hasHit;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)(_direction * _speed * Time.deltaTime);
    }

    public void Setup(float damage, ElementType elementType, Vector2 direction, float speed, GameObject impactVFXPrefab)
    {
        _damage = damage;
        _elementType = elementType;
        _direction = direction.normalized;
        _speed = speed;
        _impactVFXPrefab = impactVFXPrefab;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasHit)
        {
            return;
        }

        EnemyHealth enemyHealth = collision.GetComponentInParent<EnemyHealth>();

        if (enemyHealth == null)
        {
            return;
        }

        _hasHit = true;

        enemyHealth.TakeDamage(_damage, _elementType);
        SpawnImpactVFX();

        Destroy(gameObject);
    }

    private void SpawnImpactVFX()
    {
        if (_impactVFXPrefab == null)
        {
            return;
        }

        Instantiate(_impactVFXPrefab, transform.position, Quaternion.identity);
    }
}
