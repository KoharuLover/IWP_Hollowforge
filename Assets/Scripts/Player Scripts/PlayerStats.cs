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

    private float _equipmentMaxHealth;
    private float _equipmentMaxMana;
    private float _equipmentAttack;
    private float _equipmentAttackSpeed;
    private float _equipmentMagicPower;
    private float _equipmentMovementSpeed;
    private float _equipmentDashRange;

    public float MaxHealth => _baseMaxHealth + _bonusMaxHealth + _equipmentMaxHealth;
    public float MaxMana => _baseMaxMana + _bonusMaxMana + _equipmentMaxMana;
    public float Attack => _baseAttack + _bonusAttack + _equipmentAttack;
    public float AttackSpeed => _baseAttackSpeed + _bonusAttackSpeed + _equipmentAttackSpeed;
    public float MagicPower => _baseMagicPower + _bonusMagicPower + _equipmentMagicPower;
    public float MovementSpeed => _baseMovementSpeed + _bonusMovementSpeed + _equipmentMovementSpeed;
    public float DashRange => _baseDashRange + _bonusDashRange + _equipmentDashRange;

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

    public void RecalculateEquipmentStats()
    {
        ClearEquipmentStats();

        if (EquipmentManager.Instance == null)
        {
            return;
        }

        AddEquipmentStats(EquipmentManager.Instance.WeaponSlot1);
        AddEquipmentStats(EquipmentManager.Instance.WeaponSlot2);

        AddEquipmentStats(EquipmentManager.Instance.HelmetSlot);
        AddEquipmentStats(EquipmentManager.Instance.ChestplateSlot);
        AddEquipmentStats(EquipmentManager.Instance.PantsSlot);
        AddEquipmentStats(EquipmentManager.Instance.BootsSlot);

        EquipmentInstance[] artifactSlots = EquipmentManager.Instance.ArtifactSlots;

        for (int i = 0; i < artifactSlots.Length; i++)
        {
            AddEquipmentStats(artifactSlots[i]);
        }

        Debug.Log("Player equipment stats recalculated.");
    }

    private void ClearEquipmentStats()
    {
        _equipmentMaxHealth = 0f;
        _equipmentMaxMana = 0f;
        _equipmentAttack = 0f;
        _equipmentAttackSpeed = 0f;
        _equipmentMagicPower = 0f;
        _equipmentMovementSpeed = 0f;
        _equipmentDashRange = 0f;
    }

    private void AddEquipmentStats(EquipmentInstance equipment)
    {
        if (equipment == null || equipment.equipmentData == null)
        {
            return;
        }

        EquipmentData data = equipment.equipmentData;

        _equipmentMaxHealth += data.maxHealthBonus;
        _equipmentMaxMana += data.maxManaBonus;
        _equipmentAttack += data.attackBonus;
        _equipmentAttackSpeed += data.attackSpeedBonus;
        _equipmentMagicPower += data.magicPowerBonus;
        _equipmentMovementSpeed += data.movementSpeedBonus;
        _equipmentDashRange += data.dashRangeBonus;
    }
}
