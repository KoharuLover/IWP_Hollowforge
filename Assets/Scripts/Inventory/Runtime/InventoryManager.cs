using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public event Action InventoryChanged;

    [Header("Inventory Settings")]
    [SerializeField] private int maxInventorySlots = 9;

    [Header("Stored Equipment")]
    [SerializeField] private List<EquipmentInstance> inventorySlots = new List<EquipmentInstance>();

    public List<EquipmentInstance> InventorySlots => inventorySlots;
    public int MaxInventorySlots => maxInventorySlots;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitialiseInventorySlots();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // ----------------------------------- INITIALISATION ----------------------------------- //

    private void InitialiseInventorySlots()
    {
        while (inventorySlots.Count < maxInventorySlots)
        {
            inventorySlots.Add(null);
        }

        if (inventorySlots.Count > maxInventorySlots)
        {
            inventorySlots.RemoveRange(maxInventorySlots, inventorySlots.Count - maxInventorySlots);
        }

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (IsSlotEmpty(inventorySlots[i]))
            {
                inventorySlots[i] = null;
            }
        }
    }

    // ----------------------------------- GET ITEM ----------------------------------- //

    public EquipmentInstance GetItemAt(int slotIndex)
    {
        if (IsValidSlotIndex(slotIndex) == false)
        {
            Debug.LogWarning("Invalid inventory slot index.");
            return null;
        }

        return inventorySlots[slotIndex];
    }

    // ----------------------------------- ADD ITEM ----------------------------------- //

    public bool AddToInventory(EquipmentInstance equipment)
    {
        if (equipment == null || equipment.equipmentData == null)
        {
            Debug.LogWarning("Tried to add null equipment to inventory.");
            return false;
        }

        int emptySlotIndex = FindFirstEmptySlot();

        if (emptySlotIndex == -1)
        {
            Debug.Log("Inventory is full. Cannot add " + equipment.GetDisplayName());
            return false;
        }

        inventorySlots[emptySlotIndex] = equipment;

        NotifyInventoryChanged();

        Debug.Log("Added to inventory slot " + (emptySlotIndex + 1) + ": " + equipment.GetDisplayName());
        return true;
    }

    public bool AddToInventory(EquipmentData equipmentData)
    {
        if (equipmentData == null)
        {
            Debug.LogWarning("Tried to add null EquipmentData to inventory.");
            return false;
        }

        EquipmentInstance newEquipment = new EquipmentInstance(equipmentData, 1);

        return AddToInventory(newEquipment);
    }

    public bool AddToSpecificSlot(EquipmentInstance equipment, int slotIndex)
    {
        if (equipment == null || equipment.equipmentData == null)
        {
            Debug.LogWarning("Tried to add null equipment to an inventory slot.");
            return false;
        }

        if (IsValidSlotIndex(slotIndex) == false)
        {
            Debug.LogWarning("Invalid inventory slot index.");
            return false;
        }

        if (IsSlotEmpty(inventorySlots[slotIndex]) == false)
        {
            Debug.Log("Inventory slot " + (slotIndex + 1) + " is already occupied.");
            return false;
        }

        inventorySlots[slotIndex] = equipment;

        NotifyInventoryChanged();

        Debug.Log("Added " + equipment.GetDisplayName() + " to inventory slot " + (slotIndex + 1));
        return true;
    }

    // ----------------------------------- REMOVE ITEM ----------------------------------- //

    public bool RemoveFromInventory(EquipmentInstance equipment)
    {
        if (equipment == null)
        {
            return false;
        }

        int slotIndex = inventorySlots.IndexOf(equipment);

        if (slotIndex == -1)
        {
            Debug.Log("Inventory does not contain: " + equipment.GetDisplayName());
            return false;
        }

        inventorySlots[slotIndex] = null;

        NotifyInventoryChanged();

        Debug.Log("Removed from inventory slot " + (slotIndex + 1) + ": " + equipment.GetDisplayName());
        return true;
    }

    public bool RemoveFromSlot(int slotIndex)
    {
        if (IsValidSlotIndex(slotIndex) == false)
        {
            Debug.LogWarning("Invalid inventory slot index.");
            return false;
        }

        if (IsSlotEmpty(inventorySlots[slotIndex]))
        {
            Debug.Log("That inventory slot is already empty.");
            return false;
        }

        EquipmentInstance removedEquipment = inventorySlots[slotIndex];

        inventorySlots[slotIndex] = null;

        NotifyInventoryChanged();

        Debug.Log("Removed " + removedEquipment.GetDisplayName() + " from inventory slot " + (slotIndex + 1));
        return true;
    }

    // ----------------------------------- SWAP ITEMS ----------------------------------- //

    public bool SwapSlots(int firstSlotIndex, int secondSlotIndex)
    {
        if (IsValidSlotIndex(firstSlotIndex) == false || IsValidSlotIndex(secondSlotIndex) == false)
        {
            Debug.LogWarning("Invalid inventory slot index.");
            return false;
        }

        if (firstSlotIndex == secondSlotIndex)
        {
            return false;
        }

        EquipmentInstance temporaryEquipment = inventorySlots[firstSlotIndex];

        inventorySlots[firstSlotIndex] = inventorySlots[secondSlotIndex];

        inventorySlots[secondSlotIndex] = temporaryEquipment;

        NotifyInventoryChanged();

        Debug.Log("Swapped inventory slots " + (firstSlotIndex + 1) + " and " + (secondSlotIndex + 1));
        return true;
    }

    // ----------------------------------- INVENTORY INFO ----------------------------------- //

    public bool IsInventoryFull()
    {
        return FindFirstEmptySlot() == -1;
    }

    public int GetEmptySlotCount()
    {
        int emptySlotCount = 0;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (IsSlotEmpty(inventorySlots[i]))
            {
                emptySlotCount++;
            }
        }

        return emptySlotCount;
    }

    public int GetEquipmentCount(EquipmentData equipmentData, int starLevel)
    {
        int count = 0;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            EquipmentInstance currentEquipment = inventorySlots[i];

            if (IsSlotEmpty(currentEquipment))
            {
                continue;
            }

            if (currentEquipment.equipmentData == equipmentData && currentEquipment.starLevel == starLevel)
            {
                count++;
            }
        }

        return count;
    }

    public bool RemoveMultipleFromInventory(EquipmentData equipmentData, int starLevel, int amount)
    {
        if (equipmentData == null || amount <= 0)
        {
            return false;
        }

        if (GetEquipmentCount(equipmentData, starLevel) < amount)
        {
            Debug.Log("Not enough copies of " + equipmentData.equipmentName);
            return false;
        }

        int removedCount = 0;

        for (int i = inventorySlots.Count - 1; i >= 0; i--)
        {
            EquipmentInstance currentEquipment = inventorySlots[i];

            if (IsSlotEmpty(currentEquipment))
            {
                continue;
            }

            if (currentEquipment.equipmentData == equipmentData && currentEquipment.starLevel == starLevel)
            {
                inventorySlots[i] = null;
                removedCount++;

                if (removedCount >= amount)
                {
                    break;
                }
            }
        }

        NotifyInventoryChanged();

        Debug.Log("Removed " + amount + " copies of " + equipmentData.equipmentName + " *" + starLevel);
        return true;
    }

    // ----------------------------------- SLOT HELPERS ----------------------------------- //

    private int FindFirstEmptySlot()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (IsSlotEmpty(inventorySlots[i]))
            {
                return i;
            }
        }

        return -1;
    }

    private bool IsSlotEmpty(EquipmentInstance equipment)
    {
        return equipment == null || equipment.equipmentData == null;
    }

    private bool IsValidSlotIndex(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < inventorySlots.Count;
    }

    private void NotifyInventoryChanged()
    {
        InventoryChanged?.Invoke();
    }
}