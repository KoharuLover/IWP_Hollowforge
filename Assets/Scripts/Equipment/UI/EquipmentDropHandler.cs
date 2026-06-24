using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EquipmentSlotUI))]
public class EquipmentDropHandler : MonoBehaviour, IDropHandler
{
    private EquipmentSlotUI _equipmentSlotUI;

    private void Awake()
    {
        _equipmentSlotUI = GetComponent<EquipmentSlotUI>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_equipmentSlotUI == null || EquipmentManager.Instance == null)
        {
            return;
        }

        EquipmentInstance inventoryEquipment = InventoryDragHandler.DraggedEquipment;

        if (inventoryEquipment != null && inventoryEquipment.equipmentData != null)
        {
            TryEquipDroppedItem(inventoryEquipment);
            return;
        }

        EquipmentInstance equippedEquipment = EquipmentDragHandler.DraggedEquipment;

        if (equippedEquipment != null && equippedEquipment.equipmentData != null)
        {
            TryMoveEquippedItem();
        }
    }

    private void TryEquipDroppedItem(EquipmentInstance equipment)
    {
        EquipmentUISlotType slotType = _equipmentSlotUI.SlotType;

        int slotIndex = _equipmentSlotUI.SlotIndex;

        int sourceInventorySlotIndex = InventoryDragHandler.SourceInventorySlotIndex;

        if (slotType == EquipmentUISlotType.Weapon)
        {
            EquipmentManager.Instance.EquipOrSwapWeaponFromInventory(equipment, sourceInventorySlotIndex, slotIndex);
            return;
        }

        if (slotType == EquipmentUISlotType.Helmet)
        {
            EquipmentManager.Instance.EquipOrSwapArmourFromInventory(equipment, sourceInventorySlotIndex, ArmourSlotType.Helmet);
            return;
        }

        if (slotType == EquipmentUISlotType.Chestplate)
        {
            EquipmentManager.Instance.EquipOrSwapArmourFromInventory(equipment, sourceInventorySlotIndex, ArmourSlotType.Chestplate);
            return;
        }

        if (slotType == EquipmentUISlotType.Pants)
        {
            EquipmentManager.Instance.EquipOrSwapArmourFromInventory(equipment, sourceInventorySlotIndex, ArmourSlotType.Pants);
            return;
        }

        if (slotType == EquipmentUISlotType.Boots)
        {
            EquipmentManager.Instance.EquipOrSwapArmourFromInventory(equipment, sourceInventorySlotIndex, ArmourSlotType.Boots);
            return;
        }

        if (slotType == EquipmentUISlotType.Artifact)
        {
            EquipmentManager.Instance.EquipOrSwapArtifactFromInventory(equipment, sourceInventorySlotIndex, slotIndex);
            return;
        }
    }

    private void TryMoveEquippedItem()
    {
        EquipmentUISlotType sourceSlotType = EquipmentDragHandler.SourceSlotType;

        int sourceSlotIndex = EquipmentDragHandler.SourceSlotIndex;

        EquipmentUISlotType targetSlotType = _equipmentSlotUI.SlotType;

        int targetSlotIndex = _equipmentSlotUI.SlotIndex;

        if (sourceSlotType == EquipmentUISlotType.Weapon && targetSlotType == EquipmentUISlotType.Weapon)
        {
            EquipmentManager.Instance.MoveOrSwapWeaponSlots(sourceSlotIndex, targetSlotIndex);
            return;
        }

        if (sourceSlotType == EquipmentUISlotType.Artifact && targetSlotType == EquipmentUISlotType.Artifact)
        {
            EquipmentManager.Instance.MoveOrSwapArtifactSlots(sourceSlotIndex, targetSlotIndex);
            return;
        }

        Debug.Log("Moving between these equipment slot types is not implemented yet.");
    }
}