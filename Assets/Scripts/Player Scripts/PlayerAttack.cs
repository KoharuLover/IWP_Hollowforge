using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack VFX")]
    [SerializeField] private GameObject _attackVFXPrefab;
    [SerializeField] private Transform _attackPoint;

    [Header("Attack Settings")]
    [SerializeField] private float _attackDistance = 0.6f;

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
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        Vector2 attackDirection = mouseWorldPosition - transform.position;
        attackDirection.Normalize();

        _attackPoint.localPosition = attackDirection * _attackDistance;

        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
        _attackPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void TryAttack()
    {
        if (Time.time < _nextAttackTime)
        {
            return;
        }

        Attack();

        _nextAttackTime = Time.time + 1f / _playerStats.AttackSpeed;
    }

    private void Attack()
    {
        Debug.Log("Player attacked");

        if (_attackVFXPrefab != null && _attackPoint != null)
        {
            Instantiate(_attackVFXPrefab, _attackPoint.position, _attackPoint.rotation);
        }
    }
}
