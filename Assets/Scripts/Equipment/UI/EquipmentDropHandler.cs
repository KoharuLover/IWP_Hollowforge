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
        if (_equipmentSlotUI == null)
        {
            return;
        }

        if (EquipmentManager.Instance == null)
        {
            Debug.LogWarning("EquipmentDropHandler could not find EquipmentManager.", gameObject);
            return;
        }

        EquipmentInstance draggedEquipment = InventoryDragHandler.DraggedEquipment;

        if (draggedEquipment == null || draggedEquipment.equipmentData == null)
        {
            Debug.Log("No valid inventory equipment was dropped.");
            return;
        }

        TryEquipDroppedItem(draggedEquipment);
    }

    private void TryEquipDroppedItem(EquipmentInstance equipment)
    {
        EquipmentUISlotType slotType = _equipmentSlotUI.SlotType;

        int slotIndex = _equipmentSlotUI.SlotIndex;

        if (slotType == EquipmentUISlotType.Weapon)
        {
            EquipmentManager.Instance.EquipWeaponToSlot(equipment, slotIndex);
            return;
        }

        if (slotType == EquipmentUISlotType.Helmet)
        {
            EquipmentManager.Instance.EquipArmourToSpecificSlot(equipment, ArmourSlotType.Helmet);
            return;
        }

        if (slotType == EquipmentUISlotType.Chestplate)
        {
            EquipmentManager.Instance.EquipArmourToSpecificSlot(equipment, ArmourSlotType.Chestplate);
            return;
        }

        if (slotType == EquipmentUISlotType.Pants)
        {
            EquipmentManager.Instance.EquipArmourToSpecificSlot(equipment, ArmourSlotType.Pants);
            return;
        }

        if (slotType == EquipmentUISlotType.Boots)
        {
            EquipmentManager.Instance.EquipArmourToSpecificSlot(equipment, ArmourSlotType.Boots);
            return;
        }

        if (slotType == EquipmentUISlotType.Artifact)
        {
            EquipmentManager.Instance.EquipArtifactToSlot(equipment, slotIndex);
        }
    }
}