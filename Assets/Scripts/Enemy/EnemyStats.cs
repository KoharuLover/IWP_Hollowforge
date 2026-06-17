using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Main Stats")]
    [SerializeField] private float _maxHealth = 50f;

    [Header("Combat Stats")]
    [SerializeField] private float _attack = 10f;
    [SerializeField] private float _attackSpeed = 1f;

    [Header("Movement Stats")]
    [SerializeField] private float _movementSpeed = 2f;

    [Header("Element")]
    [SerializeField] private ElementType _elementalResistance = ElementType.Fire;
    [SerializeField] private ElementType _elementalWeakness = ElementType.Ice;

    public float MaxHealth => _maxHealth;
    public float Attack => _attack;
    public float AttackSpeed => _attackSpeed;
    public float MovementSpeed => _movementSpeed;
    public ElementType ElementalResistance => _elementalResistance;
    public ElementType ElementalWeakness => _elementalWeakness;
}
