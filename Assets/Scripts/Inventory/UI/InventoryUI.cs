using System;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private InventorySlotUI[] _inventorySlots;

    private void Awake()
    {
        FindInventorySlots();
    }

    private void Start()
    {
        InitialiseSlots();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.InventoryChanged += Refresh;
        }
        else
        {
            Debug.LogWarning("InventoryUI could not find the InventoryManager.", gameObject);
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.InventoryChanged -= Refresh;
        }
    }

    private void FindInventorySlots()
    {
        _inventorySlots = GetComponentsInChildren<InventorySlotUI>(true);

        Array.Sort(_inventorySlots, CompareSlotHierarchyOrder);
    }

    private int CompareSlotHierarchyOrder(InventorySlotUI firstSlot, InventorySlotUI secondSlot)
    {
        return firstSlot.transform.GetSiblingIndex().CompareTo(secondSlot.transform.GetSiblingIndex());
    }

    private void InitialiseSlots()
    {
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            _inventorySlots[i].Initialise(i);
        }
    }

    public void Refresh()
    {
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            _inventorySlots[i].Refresh();
        }
    }
}