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

public enum ElementalTrait
{
    None,
    Physical,
    Fire,
    Lightning
}

public enum ClassTrait
{
    None,
    Warrior,
    Mage,
    Assassin
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
    public ElementalTrait elementalTrait;
    public ClassTrait classTrait;

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
