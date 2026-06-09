using UnityEngine;

public enum EnemyState
{
    Idle,
    Chase,
    Attack,
    Dead
}

public class EnemyAI : MonoBehaviour
{
    private EnemyHealth _enemyHealth;

    private Transform _player;
    private PlayerHealth _playerHealth;

    [Header("State Settings (Range to change states)")]
    [SerializeField] private float _chaseRange = 5f;
    [SerializeField] private float _attackRange = 0.6f;

    public EnemyState CurrentState { get; private set; }

    public float AttackRange => _attackRange;

    private void Awake()
    {
        _enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Start()
    {
        CurrentState = EnemyState.Idle;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            _player = playerObject.transform;
            _playerHealth = playerObject.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        if (_enemyHealth.IsDead)
        {
            CurrentState = EnemyState.Dead;
            return;
        }

        if (_player == null || _playerHealth == null)
        {
            CurrentState = EnemyState.Idle;
            return;
        }

        if (_playerHealth.IsDead)
        {
            CurrentState = EnemyState.Idle;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, _player.position);

        if (distanceToPlayer <= _attackRange)
        {
            CurrentState = EnemyState.Attack;
            return;
        }

        if (distanceToPlayer <= _chaseRange)
        {
            CurrentState = EnemyState.Chase;
            return;
        }

        CurrentState = EnemyState.Idle;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
