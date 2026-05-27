using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    [Header("Weapon Slots")]
    [SerializeField] private EquipmentInstance weaponSlot1;
    [SerializeField] private EquipmentInstance weaponSlot2;

    [Header("Armour Slots")]
    [SerializeField] private EquipmentInstance helmetSlot;
    [SerializeField] private EquipmentInstance chestplateSlot;
    [SerializeField] private EquipmentInstance pantsSlot;
    [SerializeField] private EquipmentInstance bootsSlot;

    [Header("Artifact Slots")]
    [SerializeField] private EquipmentInstance[] artifactSlots = new EquipmentInstance[6];

    public EquipmentInstance WeaponSlot1 => weaponSlot1;
    public EquipmentInstance WeaponSlot2 => weaponSlot2;

    public EquipmentInstance HelmetSlot => helmetSlot;
    public EquipmentInstance ChestplateSlot => chestplateSlot;
    public EquipmentInstance PantsSlot => pantsSlot;
    public EquipmentInstance BootsSlot => bootsSlot;

    public EquipmentInstance[] ArtifactSlots => artifactSlots;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool EquipItem(EquipmentInstance equipment)
    {
        if (equipment == null || equipment.equipmentData == null)
        {
            Debug.LogWarning("Tried to equip null equipment.");
            return false;
        }

        EquipmentType equipmentType = equipment.equipmentData.equipmentType;

        if (equipmentType == EquipmentType.Weapon)
        {
            return EquipWeapon(equipment);
        }
        else if (equipmentType == EquipmentType.Armour)
        {
            return EquipArmour(equipment);
        }
        else if (equipmentType == EquipmentType.Artifact)
        {
            return EquipArtifact(equipment);
        }

        return false;
    }

    private bool EquipWeapon(EquipmentInstance equipment)
    {
        if (IsSameWeaponAlreadyEquipped(equipment))
        {
            Debug.Log("Cannot equip the same weapon twice: " + equipment.GetDisplayName());
            return false;
        }

        if (weaponSlot1 == null)
        {
            weaponSlot1 = equipment;
            InventoryManager.Instance.RemoveFromInventory(equipment);
            Debug.Log("Equipped weapon in slot 1: " + equipment.GetDisplayName());
            return true;
        }

        if (weaponSlot2 == null)
        {
            weaponSlot2 = equipment;
            InventoryManager.Instance.RemoveFromInventory(equipment);
            Debug.Log("Equipped weapon in slot 2: " + equipment.GetDisplayName());
            return true;
        }

        Debug.Log("No empty weapon slot.");
        return false;
    }

    private bool EquipArmour(EquipmentInstance equipment)
    {
        ArmourSlotType armourSlotType = equipment.equipmentData.armourSlotType;

        if (armourSlotType == ArmourSlotType.Helmet)
        {
            return EquipArmourToSlot(ref helmetSlot, equipment);
        }
        else if (armourSlotType == ArmourSlotType.Chestplate)
        {
            return EquipArmourToSlot(ref chestplateSlot, equipment);
        }
        else if (armourSlotType == ArmourSlotType.Pants)
        {
            return EquipArmourToSlot(ref pantsSlot, equipment);
        }
        else if (armourSlotType == ArmourSlotType.Boots)
        {
            return EquipArmourToSlot(ref bootsSlot, equipment);
        }

        Debug.Log("Invalid armour slot type.");
        return false;
    }

    private bool EquipArmourToSlot(ref EquipmentInstance armourSlot, EquipmentInstance equipment)
    {
        if (armourSlot != null)
        {
            Debug.Log("That armour slot is already occupied.");
            return false;
        }

        armourSlot = equipment;
        InventoryManager.Instance.RemoveFromInventory(equipment);
        Debug.Log("Equipped armour: " + equipment.GetDisplayName());
        return true;
    }

    private bool EquipArtifact(EquipmentInstance equipment)
    {
        for (int i = 0; i < artifactSlots.Length; i++)
        {
            if (artifactSlots[i] == null)
            {
                artifactSlots[i] = equipment;
                InventoryManager.Instance.RemoveFromInventory(equipment);
                Debug.Log("Equipped artifact in slot " + (i + 1) + ": " + equipment.GetDisplayName());
                return true;
            }
        }

        Debug.Log("No empty artifact slot.");
        return false;
    }

    private bool IsSameWeaponAlreadyEquipped(EquipmentInstance equipment)
    {
        if (weaponSlot1 != null && weaponSlot1.equipmentData == equipment.equipmentData)
        {
            return true;
        }

        if (weaponSlot2 != null && weaponSlot2.equipmentData == equipment.equipmentData)
        {
            return true;
        }

        return false;
    }
}
