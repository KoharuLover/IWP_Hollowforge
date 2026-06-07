using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private EnemyStats _enemyStats;
    private EnemyHealth _enemyHealth;
    private EnemyAI _enemyAI;
    private Animator _animator;

    private Transform _player;
    private PlayerHealth _playerHealth;

    [Header("Attack Settings")]
    [SerializeField] private float _attackRange = 0.45f;

    private const string IsAttackingParameter = "IsAttacking";

    private float _nextAttackTime;
    private bool _isAttacking;

    public bool IsAttacking => _isAttacking;

    private void Awake()
    {
        _enemyStats = GetComponent<EnemyStats>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _enemyAI = GetComponent<EnemyAI>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            _player = playerObject.transform;
            _playerHealth = playerObject.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (_enemyHealth.IsDead)
        {
            return;
        }

        if (_player == null || _playerHealth == null)
        {
            return;
        }

        if (_playerHealth.IsDead)
        {
            return;
        }

        if (_enemyAI.CurrentState != EnemyState.Attack)
        {
            return;
        }

        TryStartAttack();
    }

    private void TryStartAttack()
    {
        if (_isAttacking)
        {
            return;
        }

        if (Time.time < _nextAttackTime)
        {
            return;
        }

        StartAttack();
    }

    private void StartAttack()
    {
        _isAttacking = true;

        if (_animator != null)
        {
            _animator.SetBool(IsAttackingParameter, true);
        }

        Debug.Log(gameObject.name + " started attack animation.");
    }

    // Animation Event Start
    public void DealAttackDamage()
    {
        if (_isAttacking == false)
        {
            return;
        }

        if (CanStillHitPlayer())
        {
            _playerHealth.TakeDamage(_enemyStats.Attack);
            Debug.Log(gameObject.name + " hit player for " + _enemyStats.Attack + " damage.");
        }
        else
        {
            Debug.Log(gameObject.name + " missed.");
        }
    }

    // Animation Event End
    public void FinishAttack()
    {
        _isAttacking = false;

        if (_animator != null)
        {
            _animator.SetBool(IsAttackingParameter, false);
        }

        _nextAttackTime = Time.time + 1f / _enemyStats.AttackSpeed;

        Debug.Log(gameObject.name + " finished attack.");
    }

    private bool CanStillHitPlayer()
    {
        if (_enemyHealth.IsDead)
        {
            return false;
        }

        if (_player == null || _playerHealth == null)
        {
            return false;
        }

        if (_playerHealth.IsDead)
        {
            return false;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, _player.position);

        return distanceToPlayer <= _attackRange;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
