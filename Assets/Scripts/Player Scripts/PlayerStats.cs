using UnityEngine;
public enum StatType
{
    MaxHealth,
    MaxMana,
    Attack,
    AttackSpeed,
    MagicPower,
    MovementSpeed,
    DashRange
}
public class PlayerStats : MonoBehaviour
{
    [Header("Base Main Stats")]
    [SerializeField] private float _baseMaxHealth = 100f;
    [SerializeField] private float _baseMaxMana = 50f;

    [Header("Base Combat Stats")]
    [SerializeField] private float _baseAttack = 10f;
    [SerializeField] private float _baseAttackSpeed = 1f;
    [SerializeField] private float _baseMagicPower = 10f;

    [Header("Base Movement Stats")]
    [SerializeField] private float _baseMovementSpeed = 1f;
    [SerializeField] private float _baseDashRange = 2f;

    private float _bonusMaxHealth;
    private float _bonusMaxMana;
    private float _bonusAttack;
    private float _bonusAttackSpeed;
    private float _bonusMagicPower;
    private float _bonusMovementSpeed;
    private float _bonusDashRange;

    public float MaxHealth => _baseMaxHealth + _bonusMaxHealth;
    public float MaxMana => _baseMaxMana + _bonusMaxMana;
    public float Attack => _baseAttack + _bonusAttack;
    public float AttackSpeed => _baseAttackSpeed + _bonusAttackSpeed;
    public float MagicPower => _baseMagicPower + _bonusMagicPower;
    public float MovementSpeed => _baseMovementSpeed + _bonusMovementSpeed;
    public float DashRange => _baseDashRange + _bonusDashRange;

    public void AddModifier(StatType statType, float amount)
    {
        switch (statType)
        {
            case StatType.MaxHealth:
                _bonusMaxHealth += amount;
                break;

            case StatType.MaxMana:
                _bonusMaxMana += amount;
                break;

            case StatType.Attack:
                _bonusAttack += amount;
                break;

            case StatType.AttackSpeed:
                _bonusAttackSpeed += amount;
                break;

            case StatType.MagicPower:
                _bonusMagicPower += amount;
                break;

            case StatType.MovementSpeed:
                _bonusMovementSpeed += amount;
                break;

            case StatType.DashRange:
                _bonusDashRange += amount;
                break;
        }
    }
}
