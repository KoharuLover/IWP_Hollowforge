using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

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
    }

    public bool AddToInventory(EquipmentInstance equipment)
    {
        if (equipment == null || equipment.equipmentData == null)
        {
            Debug.LogWarning("Tried to add null equipment to inventory.");
            return false;
        }

        if (IsInventoryFull())
        {
            Debug.Log("Inventory is full. Cannot add " + equipment.GetDisplayName());
            return false;
        }

        inventorySlots.Add(equipment);
        Debug.Log("Added to inventory: " + equipment.GetDisplayName());
        return true;
    }

    public bool AddToInventory(EquipmentData equipmentData)
    {
        EquipmentInstance newEquipment = new EquipmentInstance(equipmentData, 1);
        return AddToInventory(newEquipment);
    }

    public bool RemoveFromInventory(EquipmentInstance equipment)
    {
        if (equipment == null)
        {
            return false;
        }

        if (inventorySlots.Contains(equipment) == false)
        {
            Debug.Log("Inventory does not contain: " + equipment.GetDisplayName());
            return false;
        }

        inventorySlots.Remove(equipment);
        Debug.Log("Removed from inventory: " + equipment.GetDisplayName());
        return true;
    }

    public bool IsInventoryFull()
    {
        return inventorySlots.Count >= maxInventorySlots;
    }

    public int GetEmptySlotCount()
    {
        return maxInventorySlots - inventorySlots.Count;
    }

    public int GetEquipmentCount(EquipmentData equipmentData, int starLevel)
    {
        int count = 0;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            EquipmentInstance currentEquipment = inventorySlots[i];

            if (currentEquipment.equipmentData == equipmentData && currentEquipment.starLevel == starLevel)
            {
                count++;
            }
        }

        return count;
    }

    public bool RemoveMultipleFromInventory(EquipmentData equipmentData, int starLevel, int amount)
    {
        if (equipmentData == null)
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

            if (currentEquipment.equipmentData == equipmentData && currentEquipment.starLevel == starLevel)
            {
                inventorySlots.RemoveAt(i);
                removedCount++;

                if (removedCount >= amount)
                {
                    break;
                }
            }
        }

        Debug.Log("Removed " + amount + " copies of " + equipmentData.equipmentName + " *" + starLevel);
        return true;
    }
}
