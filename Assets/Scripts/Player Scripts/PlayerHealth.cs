using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerStats _playerStats;

    private float _currentHealth;
    private bool _isDead;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _playerStats.MaxHealth;
    public bool IsDead => _isDead;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        _currentHealth = MaxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        if (_isDead)
        {
            return;
        }

        _currentHealth -= damageAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, MaxHealth);

        if (_currentHealth <= 0f)
        {
            Die();
        }
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

    private void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;

        Debug.Log("Player died");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
