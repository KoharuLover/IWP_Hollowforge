using UnityEngine;
using NaughtyAttributes;

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
    Dark
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
    // --------------------------------------------------
    // BASIC INFO
    // --------------------------------------------------

    [Header("Basic Info")]
    public string equipmentName;

    public EquipmentType equipmentType;

    [ShowIf(nameof(IsArmour))]
    public ArmourSlotType armourSlotType = ArmourSlotType.None;

    public Sprite icon;

    // --------------------------------------------------
    // COST AND RARITY
    // --------------------------------------------------

    [Header("Cost / Rarity")]
    [Min(1)]
    public int cost = 1;

    // --------------------------------------------------
    // TRAITS
    // --------------------------------------------------

    [Header("Traits")]
    public ElementType elementType;

    public ClassType classType;

    // --------------------------------------------------
    // WEAPON SETTINGS
    // --------------------------------------------------

    [Header("Weapon Settings")]
    [ShowIf(nameof(IsWeapon))]
    public WeaponAttackType weaponAttackType = WeaponAttackType.None;

    [ShowIf(nameof(IsAOE))]
    public AOECastType aoeCastType = AOECastType.None;

    [ShowIf(nameof(IsWeapon))]
    [Min(0f)]
    public float weaponDamage = 10f;

    [ShowIf(nameof(UsesAttackRange))]
    [Min(0f)]
    public float attackRange = 0.8f;

    [ShowIf(nameof(UsesAttackRadius))]
    [Min(0f)]
    public float attackRadius = 0.5f;

    [ShowIf(nameof(IsWeapon))]
    [Min(0.01f)]
    public float attackCooldown = 1f;

    [ShowIf(nameof(IsRanged))]
    [Min(0f)]
    public float projectileSpeed = 1f;

    // --------------------------------------------------
    // AOE SETTINGS
    // --------------------------------------------------

    [Header("AOE Aura Settings")]
    [ShowIf(nameof(IsAroundPlayerAOE))]
    [Min(0f)]
    public float aoeDuration = 3f;

    [ShowIf(nameof(IsAroundPlayerAOE))]
    [Min(0.01f)]
    public float aoeTickInterval = 0.5f;

    // --------------------------------------------------
    // VISUAL EFFECTS
    // --------------------------------------------------

    [Header("VFX")]
    [ShowIf(nameof(IsWeapon))]
    public GameObject attackVFXPrefab;

    [ShowIf(nameof(IsRanged))]
    public GameObject projectilePrefab;

    [ShowIf(nameof(IsRanged))]
    public GameObject impactVFXPrefab;

    // --------------------------------------------------
    // STAT BONUSES
    // --------------------------------------------------

    [Header("Stat Bonuses")]
    public float maxHealthBonus;

    public float maxManaBonus;

    public float attackBonus;

    public float attackSpeedBonus;

    public float magicPowerBonus;

    public float movementSpeedBonus;

    public float dashRangeBonus;

    // --------------------------------------------------
    // DESCRIPTION
    // --------------------------------------------------

    [Header("Description")]
    [TextArea(3, 6)]
    public string description;

    // --------------------------------------------------
    // INSPECTOR CONDITIONS
    // --------------------------------------------------

    private bool IsWeapon()
    {
        return equipmentType == EquipmentType.Weapon;
    }

    private bool IsArmour()
    {
        return equipmentType == EquipmentType.Armour;
    }

    private bool IsMelee()
    {
        return IsWeapon() &&
               weaponAttackType == WeaponAttackType.Melee;
    }

    private bool IsRanged()
    {
        return IsWeapon() &&
               weaponAttackType == WeaponAttackType.Ranged;
    }

    private bool IsAOE()
    {
        return IsWeapon() &&
               weaponAttackType == WeaponAttackType.AOE;
    }

    private bool IsAroundPlayerAOE()
    {
        return IsAOE() &&
               aoeCastType == AOECastType.AroundPlayer;
    }

    private bool IsMousePositionAOE()
    {
        return IsAOE() &&
               aoeCastType == AOECastType.AtMousePosition;
    }

    private bool UsesAttackRange()
    {
        return IsMelee() || IsMousePositionAOE();
    }

    private bool UsesAttackRadius()
    {
        return IsMelee() || IsAOE();
    }
}