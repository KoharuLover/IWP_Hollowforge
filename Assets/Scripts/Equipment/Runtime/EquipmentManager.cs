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

    // ----------------------------------- EQUIPPING FUNCS ----------------------------------- //

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
    public bool UnequipWeaponSlot(int slotIndex)
    {
        if (CanModifyEquipment == false)
        {
            Debug.Log("Equipment cannot be changed during combat.");
            return false;
        }

        if (slotIndex < 0 || slotIndex > 1)
        {
            Debug.LogWarning("Invalid weapon slot index.");
            return false;
        }

        EquipmentInstance equippedWeapon;

        if (slotIndex == 0)
        {
            equippedWeapon = weaponSlot1;
        }
        else
        {
            equippedWeapon = weaponSlot2;
        }

        if (IsSlotEmpty(equippedWeapon))
        {
            Debug.Log("That weapon slot is already empty.");
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("EquipmentManager could not find InventoryManager.");
            return false;
        }

        bool addedToInventory = InventoryManager.Instance.AddToInventory(equippedWeapon);

        if (addedToInventory == false)
        {
            Debug.Log("The weapon could not be unequipped because the inventory is full.");
            return false;
        }

        if (slotIndex == 0)
        {
            weaponSlot1 = null;
        }
        else
        {
            weaponSlot2 = null;
        }

        RefreshPlayerStats();
        NotifyEquipmentChanged();

        Debug.Log("Unequipped " + equippedWeapon.GetDisplayName() +" from weapon slot " + (slotIndex + 1));
        return true;
    }
    public bool UnequipWeaponToInventorySlot(int weaponSlotIndex, int inventorySlotIndex)
    {
        if (CanModifyEquipment == false)
        {
            Debug.Log("Equipment cannot be changed during combat.");
            return false;
        }

        if (weaponSlotIndex < 0 || weaponSlotIndex > 1)
        {
            Debug.LogWarning("Invalid weapon slot index.");
            return false;
        }

        EquipmentInstance equippedWeapon;

        if (weaponSlotIndex == 0)
        {
            equippedWeapon = weaponSlot1;
        }
        else
        {
            equippedWeapon = weaponSlot2;
        }

        if (IsSlotEmpty(equippedWeapon))
        {
            Debug.Log("That weapon slot is already empty.");
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("EquipmentManager could not find InventoryManager.");
            return false;
        }

        bool addedToInventory = InventoryManager.Instance.AddToSpecificSlot(equippedWeapon, inventorySlotIndex);

        if (addedToInventory == false)
        {
            Debug.Log("The selected inventory slot is occupied or invalid.");
            return false;
        }

        if (weaponSlotIndex == 0)
        {
            weaponSlot1 = null;
        }
        else
        {
            weaponSlot2 = null;
        }

        RefreshPlayerStats();
        NotifyEquipmentChanged();

        Debug.Log("Moved " + equippedWeapon.GetDisplayName() + " from weapon slot " + (weaponSlotIndex + 1) + " into inventory slot " + (inventorySlotIndex + 1));
        return true;
    }
    public bool MoveOrSwapWeaponSlots(int sourceSlotIndex, int targetSlotIndex)
    {
        if (CanModifyEquipment == false)
        {
            Debug.Log("Equipment cannot be changed during combat.");
            return false;
        }

        if (sourceSlotIndex < 0 || sourceSlotIndex > 1 || targetSlotIndex < 0 || targetSlotIndex > 1)
        {
            Debug.LogWarning("Invalid weapon slot index.");
            return false;
        }

        if (sourceSlotIndex == targetSlotIndex)
        {
            return false;
        }

        EquipmentInstance sourceWeapon;

        if (sourceSlotIndex == 0)
        {
            sourceWeapon = weaponSlot1;
        }
        else
        {
            sourceWeapon = weaponSlot2;
        }

        if (IsSlotEmpty(sourceWeapon))
        {
            Debug.Log("The source weapon slot is empty.");
            return false;
        }

        EquipmentInstance temporaryWeapon = weaponSlot1;
        weaponSlot1 = weaponSlot2;
        weaponSlot2 = temporaryWeapon;

        RefreshPlayerStats();
        NotifyEquipmentChanged();

        Debug.Log("Moved or swapped weapon slot " + (sourceSlotIndex + 1) + " with weapon slot " + (targetSlotIndex + 1));
        return true;
    }
    public bool EquipOrSwapWeaponFromInventory(EquipmentInstance equipment, int sourceInventorySlotIndex, int targetWeaponSlotIndex)
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

        if (targetWeaponSlotIndex < 0 || targetWeaponSlotIndex > 1)
        {
            Debug.LogWarning("Invalid weapon slot index.");
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("EquipmentManager could not find InventoryManager.");
            return false;
        }

        EquipmentInstance sourceInventoryItem = InventoryManager.Instance.GetItemAt(sourceInventorySlotIndex);

        if (sourceInventoryItem != equipment)
        {
            Debug.Log("The dragged weapon was not found in its original inventory slot.");
            return false;
        }

        EquipmentInstance otherWeaponSlot;

        if (targetWeaponSlotIndex == 0)
        {
            otherWeaponSlot = weaponSlot2;
        }
        else
        {
            otherWeaponSlot = weaponSlot1;
        }

        if (IsSlotEmpty(otherWeaponSlot) == false && otherWeaponSlot.equipmentData == equipment.equipmentData)
        {
            Debug.Log("Cannot equip the same weapon twice: " + equipment.GetDisplayName());
            return false;
        }

        EquipmentInstance replacedWeapon;

        if (targetWeaponSlotIndex == 0)
        {
            replacedWeapon = weaponSlot1;
        }
        else
        {
            replacedWeapon = weaponSlot2;
        }

        bool removedFromInventory = InventoryManager.Instance.RemoveFromInventory(equipment);

        if (removedFromInventory == false)
        {
            return false;
        }

        if (IsSlotEmpty(replacedWeapon) == false)
        {
            bool returnedOldWeapon = InventoryManager.Instance.AddToSpecificSlot(replacedWeapon, sourceInventorySlotIndex);

            if (returnedOldWeapon == false)
            {
                InventoryManager.Instance.AddToSpecificSlot(equipment, sourceInventorySlotIndex);

                Debug.Log("Could not return the previous weapon to the inventory.");
                return false;
            }
        }

        if (targetWeaponSlotIndex == 0)
        {
            weaponSlot1 = equipment;
        }
        else
        {
            weaponSlot2 = equipment;
        }

        RefreshPlayerStats();
        NotifyEquipmentChanged();

        Debug.Log("Equipped " + equipment.GetDisplayName() + " into weapon slot " + (targetWeaponSlotIndex + 1));
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
    public bool UnequipArmourSlot(ArmourSlotType slotType)
    {
        if (CanModifyEquipment == false)
        {
            Debug.Log("Equipment cannot be changed during combat.");
            return false;
        }

        EquipmentInstance equippedArmour = null;

        if (slotType == ArmourSlotType.Helmet)
        {
            equippedArmour = helmetSlot;
        }
        else if (slotType == ArmourSlotType.Chestplate)
        {
            equippedArmour = chestplateSlot;
        }
        else if (slotType == ArmourSlotType.Pants)
        {
            equippedArmour = pantsSlot;
        }
        else if (slotType == ArmourSlotType.Boots)
        {
            equippedArmour = bootsSlot;
        }
        else
        {
            Debug.LogWarning("Invalid armour slot type.");
            return false;
        }

        if (IsSlotEmpty(equippedArmour))
        {
            Debug.Log("That armour slot is already empty.");
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("EquipmentManager could not find InventoryManager.");
            return false;
        }

        bool addedToInventory = InventoryManager.Instance.AddToInventory(equippedArmour);

        if (addedToInventory == false)
        {
            Debug.Log("The armour could not be unequipped because the inventory is full.");
            return false;
        }

        if (slotType == ArmourSlotType.Helmet)
        {
            helmetSlot = null;
        }
        else if (slotType == ArmourSlotType.Chestplate)
        {
            chestplateSlot = null;
        }
        else if (slotType == ArmourSlotType.Pants)
        {
            pantsSlot = null;
        }
        else if (slotType == ArmourSlotType.Boots)
        {
            bootsSlot = null;
        }

        RefreshPlayerStats();
        NotifyEquipmentChanged();

        Debug.Log("Unequipped armour: " + equippedArmour.GetDisplayName());
        return true;
    }
    public bool UnequipArmourToInventorySlot(ArmourSlotType armourSlotType, int inventorySlotIndex)
    {
        if (CanModifyEquipment == false)
        {
            Debug.Log("Equipment cannot be changed during combat.");
            return false;
        }

        EquipmentInstance equippedArmour;

        if (armourSlotType == ArmourSlotType.Helmet)
        {
            equippedArmour = helmetSlot;
        }
        else if (armourSlotType == ArmourSlotType.Chestplate)
        {
            equippedArmour = chestplateSlot;
        }
        else if (armourSlotType == ArmourSlotType.Pants)
        {
            equippedArmour = pantsSlot;
        }
        else if (armourSlotType == ArmourSlotType.Boots)
        {
            equippedArmour = bootsSlot;
        }
        else
        {
            Debug.LogWarning("Invalid armour slot type.");
            return false;
        }

        if (IsSlotEmpty(equippedArmour))
        {
            Debug.Log("That armour slot is already empty.");
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("EquipmentManager could not find InventoryManager.");
            return false;
        }

        bool addedToInventory = InventoryManager.Instance.AddToSpecificSlot(equippedArmour, inventorySlotIndex);

        if (addedToInventory == false)
        {
            Debug.Log("The selected inventory slot is occupied or invalid.");
            return false;
        }

        if (armourSlotType == ArmourSlotType.Helmet)
        {
            helmetSlot = null;
        }
        else if (armourSlotType == ArmourSlotType.Chestplate)
        {
            chestplateSlot = null;
        }
        else if (armourSlotType == ArmourSlotType.Pants)
        {
            pantsSlot = null;
        }
        else if (armourSlotType == ArmourSlotType.Boots)
        {
            bootsSlot = null;
        }

        RefreshPlayerStats();
        NotifyEquipmentChanged();

        Debug.Log("Moved " + equippedArmour.GetDisplayName() + " into inventory slot " + (inventorySlotIndex + 1));
        return true;
    }
    public bool EquipOrSwapArmourFromInventory(EquipmentInstance equipment, int sourceInventorySlotIndex, ArmourSlotType targetArmourSlotType)
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

        if (equipment.equipmentData.armourSlotType != targetArmourSlotType)
        {
            Debug.Log(equipment.GetDisplayName() + " cannot be placed in that armour slot.");
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("EquipmentManager could not find InventoryManager.");
            return false;
        }

        EquipmentInstance sourceInventoryItem = InventoryManager.Instance.GetItemAt(sourceInventorySlotIndex);

        if (sourceInventoryItem != equipment)
        {
            Debug.Log("The dragged armour was not found in its original inventory slot.");
            return false;
        }

        EquipmentInstance replacedArmour;

        if (targetArmourSlotType == ArmourSlotType.Helmet)
        {
            replacedArmour = helmetSlot;
        }
        else if (targetArmourSlotType == ArmourSlotType.Chestplate)
        {
            replacedArmour = chestplateSlot;
        }
        else if (targetArmourSlotType == ArmourSlotType.Pants)
        {
            replacedArmour = pantsSlot;
        }
        else if (targetArmourSlotType == ArmourSlotType.Boots)
        {
            replacedArmour = bootsSlot;
        }
        else
        {
            Debug.LogWarning("Invalid armour slot type.");
            return false;
        }

        bool removedFromInventory = InventoryManager.Instance.RemoveFromInventory(equipment);

        if (removedFromInventory == false)
        {
            return false;
        }

        if (IsSlotEmpty(replacedArmour) == false)
        {
            bool returnedOldArmour = InventoryManager.Instance.AddToSpecificSlot(replacedArmour, sourceInventorySlotIndex);

            if (returnedOldArmour == false)
            {
                InventoryManager.Instance.AddToSpecificSlot(equipment, sourceInventorySlotIndex);

                Debug.Log("Could not return the previous armour to the inventory.");
                return false;
            }
        }

        if (targetArmourSlotType == ArmourSlotType.Helmet)
        {
            helmetSlot = equipment;
        }
        else if (targetArmourSlotType == ArmourSlotType.Chestplate)
        {
            chestplateSlot = equipment;
        }
        else if (targetArmourSlotType == ArmourSlotType.Pants)
        {
            pantsSlot = equipment;
        }
        else if (targetArmourSlotType == ArmourSlotType.Boots)
        {
            bootsSlot = equipment;
        }

        RefreshPlayerStats();
        NotifyEquipmentChanged();

        Debug.Log("Equipped " + equipment.GetDisplayName() + " into its armour slot.");
        return true;
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
    public bool UnequipArtifactSlot(int slotIndex)
    {
        if (CanModifyEquipment == false)
        {
            Debug.Log("Equipment cannot be changed during combat.");
            return false;
        }

        if (slotIndex < 0 || slotIndex >= artifactSlots.Length)
        {
            Debug.LogWarning("Invalid artifact slot index.");
            return false;
        }

        EquipmentInstance equippedArtifact =
            artifactSlots[slotIndex];

        if (IsSlotEmpty(equippedArtifact))
        {
            Debug.Log("That artifact slot is already empty.");
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("EquipmentManager could not find InventoryManager.");
            return false;
        }

        bool addedToInventory = InventoryManager.Instance.AddToInventory(equippedArtifact);

        if (addedToInventory == false)
        {
            Debug.Log("The artifact could not be unequipped because the inventory is full.");
            return false;
        }

        artifactSlots[slotIndex] = null;

        RefreshPlayerStats();
        NotifyEquipmentChanged();

        Debug.Log("Unequipped " + equippedArtifact.GetDisplayName() + " from artifact slot " + (slotIndex + 1));
        return true;
    }
    public bool UnequipArtifactToInventorySlot(int artifactSlotIndex, int inventorySlotIndex)
    {
        if (CanModifyEquipment == false)
        {
            Debug.Log("Equipment cannot be changed during combat.");
            return false;
        }

        if (artifactSlotIndex < 0 || artifactSlotIndex >= artifactSlots.Length)
        {
            Debug.LogWarning("Invalid artifact slot index.");
            return false;
        }

        EquipmentInstance equippedArtifact = artifactSlots[artifactSlotIndex];

        if (IsSlotEmpty(equippedArtifact))
        {
            Debug.Log("That artifact slot is already empty.");
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("EquipmentManager could not find InventoryManager.");
            return false;
        }

        bool addedToInventory = InventoryManager.Instance.AddToSpecificSlot(equippedArtifact, inventorySlotIndex);

        if (addedToInventory == false)
        {
            Debug.Log("The selected inventory slot is occupied or invalid.");
            return false;
        }

        artifactSlots[artifactSlotIndex] = null;

        RefreshPlayerStats();
        NotifyEquipmentChanged();

        Debug.Log("Moved " + equippedArtifact.GetDisplayName() + " from artifact slot " + (artifactSlotIndex + 1) + " into inventory slot " + (inventorySlotIndex + 1));
        return true;
    }
    public bool MoveOrSwapArtifactSlots(int sourceSlotIndex, int targetSlotIndex)
    {
        if (CanModifyEquipment == false)
        {
            Debug.Log("Equipment cannot be changed during combat.");
            return false;
        }

        if (sourceSlotIndex < 0 || sourceSlotIndex >= artifactSlots.Length || targetSlotIndex < 0 || targetSlotIndex >= artifactSlots.Length)
        {
            Debug.LogWarning("Invalid artifact slot index.");
            return false;
        }

        if (sourceSlotIndex == targetSlotIndex)
        {
            return false;
        }

        if (IsSlotEmpty(artifactSlots[sourceSlotIndex]))
        {
            Debug.Log("The source artifact slot is empty.");
            return false;
        }

        EquipmentInstance temporaryArtifact = artifactSlots[targetSlotIndex];
        artifactSlots[targetSlotIndex] = artifactSlots[sourceSlotIndex];
        artifactSlots[sourceSlotIndex] = temporaryArtifact;

        RefreshPlayerStats();
        NotifyEquipmentChanged();

        Debug.Log("Moved or swapped artifact slot " + (sourceSlotIndex + 1) + " with artifact slot " + (targetSlotIndex + 1));
        return true;
    }
    public bool EquipOrSwapArtifactFromInventory(EquipmentInstance equipment, int sourceInventorySlotIndex, int targetArtifactSlotIndex)
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

        if (targetArtifactSlotIndex < 0 || targetArtifactSlotIndex >= artifactSlots.Length)
        {
            Debug.LogWarning("Invalid artifact slot index.");
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("EquipmentManager could not find InventoryManager.");
            return false;
        }

        EquipmentInstance sourceInventoryItem = InventoryManager.Instance.GetItemAt(sourceInventorySlotIndex);

        if (sourceInventoryItem != equipment)
        {
            Debug.Log("The dragged artifact was not found in its original inventory slot.");
            return false;
        }

        EquipmentInstance replacedArtifact = artifactSlots[targetArtifactSlotIndex];

        bool removedFromInventory = InventoryManager.Instance.RemoveFromInventory(equipment);

        if (removedFromInventory == false)
        {
            return false;
        }

        if (IsSlotEmpty(replacedArtifact) == false)
        {
            bool returnedOldArtifact = InventoryManager.Instance.AddToSpecificSlot(replacedArtifact, sourceInventorySlotIndex);

            if (returnedOldArtifact == false)
            {
                InventoryManager.Instance.AddToSpecificSlot(equipment, sourceInventorySlotIndex);

                Debug.Log("Could not return the previous artifact to the inventory.");
                return false;
            }
        }

        artifactSlots[targetArtifactSlotIndex] = equipment;

        RefreshPlayerStats();
        NotifyEquipmentChanged();

        Debug.Log("Equipped " + equipment.GetDisplayName() + " into artifact slot " + (targetArtifactSlotIndex + 1));
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