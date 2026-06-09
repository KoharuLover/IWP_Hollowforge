using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private EnemyStats _enemyStats;
    private Animator _animator;
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private float _currentHealth;
    private bool _isDead;

    private Color _originalColor;
    private Coroutine _flashCoroutine;

    private const string IsDeadParameter = "IsDead";
    private const string IsMovingParameter = "IsMoving";
    private const string IsAttackingParameter = "IsAttacking";

    [Header("Hit Flash")]
    [SerializeField] private Color _flashColor = Color.red;
    [SerializeField] private float _flashDuration = 0.1f;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _enemyStats.MaxHealth;
    public bool IsDead => _isDead;

    private void Awake()
    {
        _enemyStats = GetComponent<EnemyStats>();
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _currentHealth = MaxHealth;

        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
        }
    }

    public void TakeDamage(float damageAmount, ElementType damageElement)
    {
        if (_isDead)
        {
            return;
        }

        float finalDamage = CalculateElementalDamage(damageAmount, damageElement);

        _currentHealth -= finalDamage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, MaxHealth);

        Debug.Log(gameObject.name + " took " + finalDamage + " " + damageElement + " damage. HP: " + _currentHealth);

        if (_currentHealth <= 0f)
        {
            Die();
            return;
        }

        PlayHitFlash();
    }

    private float CalculateElementalDamage(float damageAmount, ElementType damageElement)
    {
        if (damageElement == _enemyStats.ElementalWeakness)
        {
            return damageAmount * 1.5f;
        }

        if (damageElement == _enemyStats.ElementalResistance)
        {
            return damageAmount * 0.5f;
        }

        return damageAmount;
    }

    private void PlayHitFlash()
    {
        if (_spriteRenderer == null)
        {
            return;
        }

        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
        }

        _flashCoroutine = StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        _spriteRenderer.color = _flashColor;

        yield return new WaitForSeconds(_flashDuration);

        if (_spriteRenderer != null && _isDead == false)
        {
            _spriteRenderer.color = _originalColor;
        }

        _flashCoroutine = null;
    }

    private void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;

        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
        }

        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _originalColor;
        }

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
        }

        if (_collider != null)
        {
            _collider.enabled = false;
        }

        if (_animator != null)
        {
            _animator.SetBool(IsMovingParameter, false);
            _animator.SetBool(IsAttackingParameter, false);
            _animator.SetBool(IsDeadParameter, true);
        }

        Debug.Log(gameObject.name + " died");
    }

    // Animation Event (Death)
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
