using System.Collections;
using UnityEngine;

public class AOEDamageZone : MonoBehaviour
{
    private Transform _followTarget;

    private float _damage;
    private float _radius;
    private float _duration;
    private float _tickInterval;

    private ElementType _elementType;
    private LayerMask _enemyLayer;

    private bool _followTargetPosition;

    public void Setup(
        Transform followTarget,
        float damage,
        float radius,
        float duration,
        float tickInterval,
        ElementType elementType,
        LayerMask enemyLayer,
        bool followTargetPosition
    )
    {
        _followTarget = followTarget;
        _damage = damage;
        _radius = radius;
        _duration = duration;
        _tickInterval = tickInterval;
        _elementType = elementType;
        _enemyLayer = enemyLayer;
        _followTargetPosition = followTargetPosition;

        StartCoroutine(DamageOverTimeRoutine());
    }

    private void Update()
    {
        if (_followTargetPosition && _followTarget != null)
        {
            transform.position = _followTarget.position;
        }
    }

    private IEnumerator DamageOverTimeRoutine()
    {
        float timer = 0f;

        while (timer < _duration)
        {
            DamageEnemiesInArea();

            yield return new WaitForSeconds(_tickInterval);

            timer += _tickInterval;
        }

        Destroy(gameObject);
    }

    private void DamageEnemiesInArea()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            transform.position,
            _radius,
            _enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(_damage, _elementType);
            }
        }
    }
}
