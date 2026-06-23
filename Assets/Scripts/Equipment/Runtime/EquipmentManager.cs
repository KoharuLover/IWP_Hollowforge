using System;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    public event Action EquipmentChanged;

    [Header("Equipment Editing")]
    public bool CanModifyEquipment { get; private set; } = true;

    [Header("Weapon Slots")]
    [SerializeField] private EquipmentInstance weaponSlot1;
    [SerializeField] private EquipmentInstance weaponSlot2;
    [SerializeField] private int activeWeaponSlotIndex = 0;

    public int ActiveWeaponSlotIndex => activeWeaponSlotIndex;

    [Header("Armour Slots")]
    [SerializeField] private EquipmentInstance helmetSlot;
    [SerializeField] private EquipmentInstance chestplateSlot;
    [SerializeField] private EquipmentInstance pantsSlot;
    [SerializeField] private EquipmentInstance bootsSlot;

    [Header("Artifact Slots")]
    [SerializeField]
    private EquipmentInstance[] artifactSlots = new EquipmentInstance[6];

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

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // ----------------------------------- EDITING LOCK ----------------------------------- //

    public void SetEquipmentEditingAllowed(bool isAllowed)
    {
        CanModifyEquipment = isAllowed;

        if (CanModifyEquipment)
        {
            Debug.Log("Equipment editing unlocked.");
        }
        else
        {
            Debug.Log("Equipment editing locked.");
        }
    }

    // ----------------------------------- EQUIPPING FUNC ----------------------------------- //

    public bool EquipItem(EquipmentInstance equipment)
    {
        if (CanModifyEquipment == false)
        {
            Debug.Log("Equipment cannot be changed during combat.");
            return false;
        }

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

        Debug.LogWarning("Invalid equipment type.");
        return false;
    }

    public bool EquipWeaponToSlot(EquipmentInstance equipment, int slotIndex)
    {
        if (CanModifyEquipment == false)
        {
            Debug.Log("Equipment cannot be changed during combat.");
            return false;
        }

        if (equipment == null || equipment.equipmentData == null)
        {
            Debug.LogWarning("Tried to equip null equipment.");
            return false;
        }

        if (equipment.equipmentData.equipmentType != EquipmentType.Weapon)
        {
            Debug.Log("Only weapons can be placed in weapon slots.");
            return false;
        }

        if (slotIndex < 0 || slotIndex > 1)
        {
            Debug.LogWarning("Invalid weapon slot index.");
            return false;
        }

        if (IsSameWeaponAlreadyEquipped(equipment))
        {
            Debug.Log("Cannot equip the same weapon twice: " + equipment.GetDisplayName());
            return false;
        }

        EquipmentInstance targetSlot = slotIndex == 0 ? weaponSlot1 : weaponSlot2;

        if (IsSlotEmpty(targetSlot) == false)
        {
            Debug.Log("Weapon slot " + (slotIndex + 1) + " is already occupied.");
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("EquipmentManager could not find InventoryManager.");
            return false;
        }

        if (InventoryManager.Instance.RemoveFromInventory(equipment) == false)
        {
            Debug.Log("The weapon was not found in the inventory.");
            return false;
        }

        if (slotIndex == 0)
        {
            weaponSlot1 = equipment;
        }
        else
        {
            weaponSlot2 = equipment;
        }

        RefreshPlayerStats();
        NotifyEquipmentChanged();

        Debug.Log("Equipped " + equipment.GetDisplayName() + " into weapon slot " + (slotIndex + 1));
        return true;
    }
    public bool EquipArmourToSpecificSlot(EquipmentInstance equipment, ArmourSlotType targetSlotType)
    {
        if (CanModifyEquipment == false)
        {
            Debug.Log("Equipment cannot be changed during combat.");
            return false;
        }

        if (equipment == null || equipment.equipmentData == null)
        {
            Debug.LogWarning("Tried to equip null equipment.");
            return false;
        }

        if (equipment.equipmentData.equipmentType != EquipmentType.Armour)
        {
            Debug.Log("Only armour can be placed in armour slots.");
            return false;
        }

        if (equipment.equipmentData.armourSlotType != targetSlotType)
        {
            Debug.Log(equipment.GetDisplayName() + " cannot be placed in that armour slot.");
            return false;
        }

        if (targetSlotType == ArmourSlotType.Helmet)
        {
            return EquipArmourToSlot(ref helmetSlot, equipment);
        }

        if (targetSlotType == ArmourSlotType.Chestplate)
        {
            return EquipArmourToSlot(ref chestplateSlot, equipment);
        }

        if (targetSlotType == ArmourSlotType.Pants)
        {
            return EquipArmourToSlot(ref pantsSlot, equipment);
        }

        if (targetSlotType == ArmourSlotType.Boots)
        {
            return EquipArmourToSlot(ref bootsSlot, equipment);
        }

        Debug.LogWarning("Invalid armour slot type.");
        return false;
    }
    public bool EquipArtifactToSlot(EquipmentInstance equipment, int slotIndex)
    {
        if (CanModifyEquipment == false)
        {
            Debug.Log("Equipment cannot be changed during combat.");
            return false;
        }

        if (equipment == null || equipment.equipmentData == null)
        {
            Debug.LogWarning("Tried to equip null equipment.");
            return false;
        }

        if (equipment.equipmentData.equipmentType != EquipmentType.Artifact)
        {
            Debug.Log("Only artifacts can be placed in artifact slots.");
            return false;
        }

        if (slotIndex < 0 || slotIndex >= artifactSlots.Length)
        {
            Debug.LogWarning("Invalid artifact slot index.");
            return false;
        }

        if (IsSlotEmpty(artifactSlots[slotIndex]) == false)
        {
            Debug.Log("Artifact slot " + (slotIndex + 1) + " is already occupied.");
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("EquipmentManager could not find InventoryManager.");
            return false;
        }

        bool removedFromInventory = InventoryManager.Instance.RemoveFromInventory(equipment);

        if (removedFromInventory == false)
        {
            Debug.Log("The artifact was not found in the inventory.");
            return false;
        }

        artifactSlots[slotIndex] = equipment;

        RefreshPlayerStats();
        NotifyEquipmentChanged();

        Debug.Log("Equipped " + equipment.GetDisplayName() + " into artifact slot " + (slotIndex + 1));
        return true;
    }

    private bool EquipWeapon(EquipmentInstance equipment)
    {
        if (IsSameWeaponAlreadyEquipped(equipment))
        {
            Debug.Log("Cannot equip the same weapon twice: " + equipment.GetDisplayName());
            return false;
        }

        if (IsSlotEmpty(weaponSlot1))
        {
            weaponSlot1 = equipment;

            InventoryManager.Instance.RemoveFromInventory(equipment);

            RefreshPlayerStats();
            NotifyEquipmentChanged();

            Debug.Log("Equipped weapon in slot 1: " + equipment.GetDisplayName());
            return true;
        }

        if (IsSlotEmpty(weaponSlot2))
        {
            weaponSlot2 = equipment;

            InventoryManager.Instance.RemoveFromInventory(equipment);

            RefreshPlayerStats();
            NotifyEquipmentChanged();

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
        if (IsSlotEmpty(armourSlot) == false)
        {
            Debug.Log("That armour slot is already occupied.");
            return false;
        }

        armourSlot = equipment;

        InventoryManager.Instance.RemoveFromInventory(equipment);

        RefreshPlayerStats();
        NotifyEquipmentChanged();

        Debug.Log("Equipped armour: " + equipment.GetDisplayName());
        return true;
    }
    private bool EquipArtifact(EquipmentInstance equipment)
    {
        for (int i = 0; i < artifactSlots.Length; i++)
        {
            if (IsSlotEmpty(artifactSlots[i]))
            {
                artifactSlots[i] = equipment;

                InventoryManager.Instance.RemoveFromInventory(equipment);

                RefreshPlayerStats();
                NotifyEquipmentChanged();

                Debug.Log("Equipped artifact in slot " + (i + 1) + ": " + equipment.GetDisplayName());
                return true;
            }
        }

        Debug.Log("No empty artifact slot.");
        return false;
    }

    private bool IsSameWeaponAlreadyEquipped(EquipmentInstance equipment)
    {
        if (IsSlotEmpty(weaponSlot1) == false && weaponSlot1.equipmentData == equipment.equipmentData)
        {
            return true;
        }

        if (IsSlotEmpty(weaponSlot2) == false && weaponSlot2.equipmentData == equipment.equipmentData)
        {
            return true;
        }

        return false;
    }
    private bool IsSlotEmpty(EquipmentInstance slot)
    {
        return slot == null || slot.equipmentData == null;
    }

    private void RefreshPlayerStats()
    {
        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.RecalculateEquipmentStats();
        }

        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.RefreshMaxHealth();
        }
    }

    private void NotifyEquipmentChanged()
    {
        EquipmentChanged?.Invoke();
    }

    // ----------------------------------- WEAPON FUNC ----------------------------------- //

    public EquipmentInstance ActiveWeapon
    {
        get
        {
            if (activeWeaponSlotIndex == 0)
            {
                return weaponSlot1;
            }
            return weaponSlot2;
        }
    }

    public EquipmentData ActiveWeaponData
    {
        get
        {
            if (ActiveWeapon == null || ActiveWeapon.equipmentData == null)
            {
                return null;
            }
            return ActiveWeapon.equipmentData;
        }
    }

    public void SetActiveWeaponSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > 1)
        {
            Debug.LogWarning("Invalid weapon slot index.");
            return;
        }

        activeWeaponSlotIndex = slotIndex;

        if (ActiveWeaponData != null)
        {
            Debug.Log("Active weapon changed to: " + ActiveWeaponData.equipmentName);
        }
        else
        {
            Debug.Log("Active weapon slot is empty.");
        }
    }
}