using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private EnemyStats _enemyStats;

    private float _currentHealth;
    private bool _isDead;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _enemyStats.MaxHealth;
    public bool IsDead => _isDead;

    private void Awake()
    {
        _enemyStats = GetComponent<EnemyStats>();
    }

    private void Start()
    {
        _currentHealth = MaxHealth;
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

        Debug.Log(gameObject.name + " took " + finalDamage + " damage. HP: " + _currentHealth);

        if (_currentHealth <= 0f)
        {
            Die();
        }
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

    private void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;

        Debug.Log(gameObject.name + " died");

        Destroy(gameObject);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10f, ElementType.Physical);
        }
    }

}
