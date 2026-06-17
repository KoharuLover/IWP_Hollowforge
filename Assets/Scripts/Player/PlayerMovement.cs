using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 _movement;

    private PlayerStats _playerStats;
    private PlayerHealth _playerHealth;
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private const string IsMovingParameter = "IsMoving";

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _playerStats = GetComponent<PlayerStats>();
        _playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (_playerHealth.IsDead)
        {
            _movement = Vector2.zero;
            _animator.SetBool(IsMovingParameter, false);
            return;
        }

        _movement = InputManager.Movement;

        if (_movement.sqrMagnitude > 1f)
        {
            _movement.Normalize();
        }

        UpdateAnimation();
        UpdateFacingDirection();
    }

    private void FixedUpdate()
    {
        if (_playerHealth.IsDead)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        _rb.linearVelocity = _movement * _playerStats.MovementSpeed;
    }

    private void UpdateAnimation()
    {
        bool isMoving = _movement.sqrMagnitude > 0.01f;

        _animator.SetBool(IsMovingParameter, isMoving);
    }

    private void UpdateFacingDirection()
    {
        if (_movement.x > 0.01f)
        {
            _spriteRenderer.flipX = false;
        }
        else if (_movement.x < -0.01f)
        {
            _spriteRenderer.flipX = true;
        }
    }
}
