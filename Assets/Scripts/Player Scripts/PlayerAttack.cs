using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _attackPoint;

    [Header("Layers")]
    [SerializeField] private LayerMask _enemyLayer;

    private PlayerStats _playerStats;
    private PlayerHealth _playerHealth;
    private Camera _mainCamera;

    private float[] _nextAttackTimes = new float[2];

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
        HandleWeaponSwitchInput();

        if (InputManager.AttackPressed)
        {
            TryAttack();
        }
    }

    private void HandleWeaponSwitchInput()
    {
        if (EquipmentManager.Instance == null)
        {
            return;
        }

        if (InputManager.WeaponSlot1Pressed)
        {
            EquipmentManager.Instance.SetActiveWeaponSlot(0);
        }

        if (InputManager.WeaponSlot2Pressed)
        {
            EquipmentManager.Instance.SetActiveWeaponSlot(1);
        }
    }

    private void UpdateAttackPointDirection()
    {
        if (_attackPoint == null || _mainCamera == null)
        {
            return;
        }

        if (EquipmentManager.Instance == null)
        {
            return;
        }

        EquipmentData weapon = EquipmentManager.Instance.ActiveWeaponData;

        float attackRange = 0.8f;

        if (weapon != null)
        {
            attackRange = weapon.attackRange;
        }

        Vector3 mouseScreenPosition = InputManager.MousePosition;
        Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0f;

        Vector2 attackDirection = mouseWorldPosition - transform.position;

        if (attackDirection == Vector2.zero)
        {
            return;
        }

        attackDirection.Normalize();

        _attackPoint.localPosition = attackDirection * attackRange;

        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
        _attackPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void TryAttack()
    {
        if (EquipmentManager.Instance == null)
        {
            Debug.LogWarning("No EquipmentManager found.");
            return;
        }

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

        int activeWeaponSlotIndex = EquipmentManager.Instance.ActiveWeaponSlotIndex;

        if (activeWeaponSlotIndex < 0 || activeWeaponSlotIndex >= _nextAttackTimes.Length)
        {
            Debug.LogWarning("Invalid active weapon slot index.");
            return;
        }

        if (Time.time < _nextAttackTimes[activeWeaponSlotIndex])
        {
            return;
        }

        Attack(weapon);

        _nextAttackTimes[activeWeaponSlotIndex] = Time.time + weapon.attackCooldown / _playerStats.AttackSpeed;
    }

    private void Attack(EquipmentData weapon)
    {
        if (weapon.weaponAttackType == WeaponAttackType.Melee)
        {
            MeleeAttack(weapon);
        }
        else if (weapon.weaponAttackType == WeaponAttackType.Ranged)
        {
            RangedAttack(weapon);
        }
        else if (weapon.weaponAttackType == WeaponAttackType.AOE)
        {
            AOEAttack(weapon);
        }
        else
        {
            Debug.Log("This weapon attack type is not supported yet.");
        }
    }

    private void MeleeAttack(EquipmentData weapon)
    {
        SpawnAttackVFX(weapon);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            _attackPoint.position,
            weapon.attackRadius,
            _enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                float totalDamage = weapon.weaponDamage + _playerStats.Attack;
                enemyHealth.TakeDamage(totalDamage, weapon.elementType);
            }
        }

        Debug.Log("Melee attack with " + weapon.equipmentName);
    }

    private void RangedAttack(EquipmentData weapon)
    {
        SpawnAttackVFX(weapon);

        if (weapon.projectilePrefab == null)
        {
            Debug.LogWarning(weapon.equipmentName + " has no projectile prefab.");
            return;
        }

        GameObject projectileObject = Instantiate(
            weapon.projectilePrefab,
            _attackPoint.position,
            _attackPoint.rotation
        );

        PlayerProjectile projectile = projectileObject.GetComponent<PlayerProjectile>();

        if (projectile != null)
        {
            float totalDamage = weapon.weaponDamage + _playerStats.Attack;
            Vector2 direction = _attackPoint.right;

            projectile.Setup(
                totalDamage,
                weapon.elementType,
                direction,
                weapon.projectileSpeed,
                weapon.impactVFXPrefab
            );
        }

        Debug.Log("Ranged attack with " + weapon.equipmentName);
    }

    private void AOEAttack(EquipmentData weapon)
    {
        if (weapon.aoeCastType == AOECastType.AroundPlayer)
        {
            AroundPlayerAOEAttack(weapon);
        }
        else
        {
            Debug.Log("This AOE cast type is not supported yet.");
        }
    }

    private void AroundPlayerAOEAttack(EquipmentData weapon)
    {
        if (weapon.attackVFXPrefab == null)
        {
            Debug.LogWarning(weapon.equipmentName + " has no AOE VFX prefab.");
            return;
        }

        GameObject aoeObject = Instantiate(
            weapon.attackVFXPrefab,
            transform.position,
            Quaternion.identity
        );

        AOEDamageZone aoeDamageZone = aoeObject.GetComponent<AOEDamageZone>();

        if (aoeDamageZone == null)
        {
            Debug.LogWarning("AOE prefab does not have AOEDamageZone script.");
            return;
        }

        float totalDamage = weapon.weaponDamage + _playerStats.MagicPower;

        aoeDamageZone.Setup(
            transform,
            totalDamage,
            weapon.attackRadius,
            weapon.aoeDuration,
            weapon.aoeTickInterval,
            weapon.elementType,
            _enemyLayer,
            true
        );

        Debug.Log("Around player AOE attack with " + weapon.equipmentName);
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

    private void OnDrawGizmos()
    {
        if (_attackPoint == null)
        {
            return;
        }

        float attackRadius = 0.5f;

        if (Application.isPlaying && EquipmentManager.Instance != null)
        {
            EquipmentData weapon = EquipmentManager.Instance.ActiveWeaponData;

            if (weapon != null)
            {
                attackRadius = weapon.attackRadius;
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_attackPoint.position, attackRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, _attackPoint.position);
    }
}
