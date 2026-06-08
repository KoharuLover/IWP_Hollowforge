using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;

    [Header("Hit Flash")]
    [SerializeField] private Color _flashColor = Color.red;
    [SerializeField] private float _flashDuration = 0.1f;

    private PlayerStats _playerStats;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    private float _currentHealth;
    private bool _isDead;

    private Color _originalColor;
    private Coroutine _flashCoroutine;

    private const string IsDeadParameter = "IsDead";

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _playerStats.MaxHealth;
    public bool IsDead => _isDead;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        _currentHealth = MaxHealth;

        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (_isDead)
        {
            return;
        }

        _currentHealth -= damageAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, MaxHealth);

        Debug.Log("Player HP: " + _currentHealth);

        if (_currentHealth <= 0f)
        {
            Die();
            return;
        }

        PlayHitFlash();
    }

    public void Heal(float healAmount)
    {
        if (_isDead)
        {
            return;
        }

        _currentHealth += healAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, MaxHealth);
    }

    public void RefreshMaxHealth()
    {
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, MaxHealth);
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

        if (_animator != null)
        {
            _animator.SetBool(IsDeadParameter, true);
        }

        Debug.Log("Player died");
    }
}
