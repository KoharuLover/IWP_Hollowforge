using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private EnemyStats _enemyStats;
    private EnemyHealth _enemyHealth;
    private Rigidbody2D _rb;

    private Transform _player;
    private PlayerHealth _playerHealth;

    [Header("Movement Settings")]
    [SerializeField] private float _stopDistance = 1.2f;
    [SerializeField] private float _directionSmoothSpeed = 8f;

    [Header("Soft Surround Settings")]
    [SerializeField] private float _surroundStartDistance = 2f;
    [SerializeField] private float _surroundRadius = 0.8f;
    [SerializeField] private int _spotCheckCount = 12;
    [SerializeField] private float _spotOccupiedRadius = 0.35f;
    [SerializeField] private float _spotRefreshInterval = 1f;

    [Header("Separation Settings")]
    [SerializeField] private float _separationRadius = 0.5f;
    [SerializeField] private float _separationWeight = 0.25f;
    [SerializeField] private LayerMask _enemyLayer;

    private Vector2 _currentMoveDirection;
    private Vector2 _currentTargetPosition;
    private float _nextSpotRefreshTime;

    private void Awake()
    {
        _enemyStats = GetComponent<EnemyStats>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            _player = playerObject.transform;
            _playerHealth = playerObject.GetComponent<PlayerHealth>();
        }

        _currentTargetPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (_enemyHealth.IsDead)
        {
            StopMoving();
            return;
        }

        if (_player == null || _playerHealth == null)
        {
            StopMoving();
            return;
        }

        if (_playerHealth.IsDead)
        {
            StopMoving();
            return;
        }

        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        Vector2 playerPosition = _player.position;
        Vector2 enemyPosition = transform.position;

        float distanceToPlayer = Vector2.Distance(enemyPosition, playerPosition);

        if (distanceToPlayer <= _stopDistance)
        {
            StopMoving();
            return;
        }

        if (distanceToPlayer > _surroundStartDistance)
        {
            _currentTargetPosition = playerPosition;
        }
        else if (Time.time >= _nextSpotRefreshTime)
        {
            _currentTargetPosition = GetBestVacantSpotNearPlayer();
            _nextSpotRefreshTime = Time.time + _spotRefreshInterval;
        }

        Vector2 directionToTarget = _currentTargetPosition - enemyPosition;

        if (directionToTarget == Vector2.zero)
        {
            StopMoving();
            return;
        }

        directionToTarget.Normalize();

        Vector2 separationDirection = GetSeparationDirection();

        Vector2 finalDirection =
            directionToTarget +
            separationDirection * _separationWeight;

        if (finalDirection == Vector2.zero)
        {
            StopMoving();
            return;
        }

        finalDirection.Normalize();

        _currentMoveDirection = Vector2.Lerp(
            _currentMoveDirection,
            finalDirection,
            _directionSmoothSpeed * Time.fixedDeltaTime
        );

        _rb.linearVelocity = _currentMoveDirection * _enemyStats.MovementSpeed;
    }

    private Vector2 GetBestVacantSpotNearPlayer()
    {
        Vector2 playerPosition = _player.position;
        Vector2 enemyPosition = transform.position;

        Vector2 bestSpot = playerPosition;
        float bestScore = Mathf.Infinity;

        for (int i = 0; i < _spotCheckCount; i++)
        {
            float angle = 360f / _spotCheckCount * i;
            float angleRad = angle * Mathf.Deg2Rad;

            Vector2 spotOffset = new Vector2(
                Mathf.Cos(angleRad),
                Mathf.Sin(angleRad)
            ) * _surroundRadius;

            Vector2 spotPosition = playerPosition + spotOffset;

            if (IsSpotOccupied(spotPosition))
            {
                continue;
            }

            float distanceFromEnemy = Vector2.Distance(enemyPosition, spotPosition);

            if (distanceFromEnemy < bestScore)
            {
                bestScore = distanceFromEnemy;
                bestSpot = spotPosition;
            }
        }

        return bestSpot;
    }

    private bool IsSpotOccupied(Vector2 spotPosition)
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(
            spotPosition,
            _spotOccupiedRadius,
            _enemyLayer
        );

        foreach (Collider2D enemy in nearbyEnemies)
        {
            if (enemy.gameObject == gameObject)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private Vector2 GetSeparationDirection()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(
            transform.position,
            _separationRadius,
            _enemyLayer
        );

        Vector2 separationDirection = Vector2.zero;

        foreach (Collider2D enemy in nearbyEnemies)
        {
            if (enemy.gameObject == gameObject)
            {
                continue;
            }

            Vector2 awayFromEnemy = transform.position - enemy.transform.position;

            if (awayFromEnemy == Vector2.zero)
            {
                continue;
            }

            float distance = awayFromEnemy.magnitude;

            separationDirection += awayFromEnemy.normalized / distance;
        }

        return separationDirection;
    }

    private void StopMoving()
    {
        _rb.linearVelocity = Vector2.zero;
        _currentMoveDirection = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _separationRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _stopDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _surroundStartDistance);

        if (Application.isPlaying && _player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_currentTargetPosition, 0.1f);
        }

        if (Application.isPlaying && _player != null)
        {
            Gizmos.color = Color.magenta;

            Vector2 playerPosition = _player.position;

            for (int i = 0; i < _spotCheckCount; i++)
            {
                float angle = 360f / _spotCheckCount * i;
                float angleRad = angle * Mathf.Deg2Rad;

                Vector2 spotOffset = new Vector2(
                    Mathf.Cos(angleRad),
                    Mathf.Sin(angleRad)
                ) * _surroundRadius;

                Vector2 spotPosition = playerPosition + spotOffset;

                Gizmos.DrawWireSphere(spotPosition, 0.06f);
            }
        }
    }
}
