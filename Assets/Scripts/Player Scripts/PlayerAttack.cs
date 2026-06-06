using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _attackPoint;

    private PlayerStats _playerStats;
    private PlayerHealth _playerHealth;
    private Camera _mainCamera;

    private float _nextAttackTime;

    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
        _playerHealth = GetComponent<PlayerHealth>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (_playerHealth.IsDead)
        {
            return;
        }

        UpdateAttackPointDirection();

        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }
    }

    private void UpdateAttackPointDirection()
    {
        if (_attackPoint == null || _mainCamera == null)
        {
            return;
        }

        EquipmentData weapon = EquipmentManager.Instance.ActiveWeaponData;

        float attackRange = 0.8f;

        if (weapon != null)
        {
            attackRange = weapon.attackRange;
        }

        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        Vector2 attackDirection = mouseWorldPosition - transform.position;
        attackDirection.Normalize();

        _attackPoint.localPosition = attackDirection * attackRange;

        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
        _attackPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void TryAttack()
    {
        EquipmentData weapon = EquipmentManager.Instance.ActiveWeaponData;

        if (weapon == null)
        {
            Debug.Log("No active weapon equipped.");
            return;
        }

        if (weapon.equipmentType != EquipmentType.Weapon)
        {
            Debug.Log("Active equipment is not a weapon.");
            return;
        }

        if (weapon.weaponAttackType != WeaponAttackType.Melee)
        {
            Debug.Log("This weapon is not melee yet.");
            return;
        }

        if (Time.time < _nextAttackTime)
        {
            return;
        }

        MeleeAttack(weapon);

        _nextAttackTime = Time.time + 1f / _playerStats.AttackSpeed;
    }

    private void MeleeAttack(EquipmentData weapon)
    {
        SpawnAttackVFX(weapon);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            _attackPoint.position,
            weapon.attackRadius,
            LayerMask.GetMask("Enemy")
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                float totalDamage = weapon.weaponDamage + _playerStats.Attack;
                enemyHealth.TakeDamage(totalDamage, weapon.elementType);
            }
        }

        Debug.Log("Attacked with " + weapon.equipmentName);
    }

    private void SpawnAttackVFX(EquipmentData weapon)
    {
        if (weapon.attackVFXPrefab == null || _attackPoint == null)
        {
            return;
        }

        Instantiate(
            weapon.attackVFXPrefab,
            _attackPoint.position,
            _attackPoint.rotation
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (_attackPoint == null)
        {
            return;
        }

        EquipmentData weapon = null;

        if (EquipmentManager.Instance != null)
        {
            weapon = EquipmentManager.Instance.ActiveWeaponData;
        }

        float radius = 0.5f;

        if (weapon != null)
        {
            radius = weapon.attackRadius;
        }

        Gizmos.DrawWireSphere(_attackPoint.position, radius);
    }
}
