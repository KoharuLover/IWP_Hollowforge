using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InventorySlotUI))]
public class InventoryDropHandler : MonoBehaviour, IDropHandler
{
    private InventorySlotUI _inventorySlotUI;

    private void Awake()
    {
        _inventorySlotUI = GetComponent<InventorySlotUI>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_inventorySlotUI == null)
        {
            return;
        }

        // First check whether the item came from another inventory slot.
        if (TryMoveInventoryItem())
        {
            return;
        }

        // Otherwise, check whether it came from an equipment slot.
        TryMoveEquipmentToInventory();
    }

    private bool TryMoveInventoryItem()
    {
        if (InventoryManager.Instance == null)
        {
            return false;
        }

        EquipmentInstance draggedEquipment = InventoryDragHandler.DraggedEquipment;

        int sourceInventorySlotIndex = InventoryDragHandler.SourceInventorySlotIndex;

        if (draggedEquipment == null || draggedEquipment.equipmentData == null || sourceInventorySlotIndex < 0)
        {
            return false;
        }

        int targetInventorySlotIndex = _inventorySlotUI.SlotIndex;

        if (sourceInventorySlotIndex == targetInventorySlotIndex)
        {
            return true;
        }

        InventoryManager.Instance.SwapSlots(sourceInventorySlotIndex, targetInventorySlotIndex);

        return true;
    }

    private void TryMoveEquipmentToInventory()
    {
        if (EquipmentManager.Instance == null)
        {
            return;
        }

        EquipmentInstance draggedEquipment = EquipmentDragHandler.DraggedEquipment;

        if (draggedEquipment == null || draggedEquipment.equipmentData == null)
        {
            return;
        }

        EquipmentUISlotType sourceSlotType = EquipmentDragHandler.SourceSlotType;

        int sourceSlotIndex = EquipmentDragHandler.SourceSlotIndex;

        int targetInventorySlotIndex = _inventorySlotUI.SlotIndex;

        if (sourceSlotType == EquipmentUISlotType.Weapon)
        {
            EquipmentManager.Instance.UnequipWeaponToInventorySlot(sourceSlotIndex, targetInventorySlotIndex);
            return;
        }

        if (sourceSlotType == EquipmentUISlotType.Helmet)
        {
            EquipmentManager.Instance.UnequipArmourToInventorySlot(ArmourSlotType.Helmet, targetInventorySlotIndex);
            return;
        }

        if (sourceSlotType == EquipmentUISlotType.Chestplate)
        {
            EquipmentManager.Instance.UnequipArmourToInventorySlot(ArmourSlotType.Chestplate, targetInventorySlotIndex);
            return;
        }

        if (sourceSlotType == EquipmentUISlotType.Pants)
        {
            EquipmentManager.Instance.UnequipArmourToInventorySlot(ArmourSlotType.Pants, targetInventorySlotIndex);
            return;
        }

        if (sourceSlotType == EquipmentUISlotType.Boots)
        {
            EquipmentManager.Instance.UnequipArmourToInventorySlot(ArmourSlotType.Boots, targetInventorySlotIndex);
            return;
        }

        if (sourceSlotType == EquipmentUISlotType.Artifact)
        {
            EquipmentManager.Instance.UnequipArtifactToInventorySlot(sourceSlotIndex, targetInventorySlotIndex);
        }
    }
}