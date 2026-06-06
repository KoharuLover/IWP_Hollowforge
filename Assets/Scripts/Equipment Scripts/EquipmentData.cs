using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armour,
    Artifact
}

public enum ArmourSlotType
{
    None,
    Helmet,
    Chestplate,
    Pants,
    Boots
}

public enum ElementType
{
    None,
    Physical,
    Fire,
    Ice,
    Lightning,
    Dark,
}

public enum ClassType
{
    None,
    Warrior,
    Mage,
    Assassin
}

public enum WeaponAttackType
{
    None,
    Melee,
    Ranged,
    AOE
}

public enum AOECastType
{
    None,
    AroundPlayer,
    AtMousePosition
}

[CreateAssetMenu(fileName = "New Equipment", menuName = "Hollowforge/Equipment")]
public class EquipmentData : ScriptableObject
{
    [Header("Basic Info")]
    public string equipmentName;
    public EquipmentType equipmentType;
    public ArmourSlotType armourSlotType;
    public Sprite icon;

    [Header("Cost / Rarity")]
    public int cost = 1;

    [Header("Traits")]
    public ElementType elementType;
    public ClassType classType;

    [Header("Weapon Settings")]
    public WeaponAttackType weaponAttackType;
    public AOECastType aoeCastType = AOECastType.None;

    public float weaponDamage = 10f;
    public float attackRange = 0.8f;
    public float attackRadius = 0.5f;
    public float attackCooldown = 1f;
    public float projectileSpeed = 1f;

    [Header("AOE Aura Settings")]
    public float aoeDuration = 3f;
    public float aoeTickInterval = 0.5f;

    [Header("VFX")]
    public GameObject attackVFXPrefab;
    public GameObject projectilePrefab;
    public GameObject impactVFXPrefab;

    [Header("Stat Bonuses")]
    public float maxHealthBonus;
    public float maxManaBonus;
    public float attackBonus;
    public float attackSpeedBonus;
    public float magicPowerBonus;
    public float movementSpeedBonus;
    public float dashRangeBonus;

    [Header("Description")]
    [TextArea]
    public string description;
}
