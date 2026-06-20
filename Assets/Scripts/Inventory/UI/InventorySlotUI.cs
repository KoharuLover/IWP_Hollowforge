using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image _itemIcon;

    private int _slotIndex;

    public int SlotIndex => _slotIndex;

    public void Initialise(int slotIndex)
    {
        _slotIndex = slotIndex;
        Refresh();
    }

    public void Refresh()
    {
        if (InventoryManager.Instance == null)
        {
            ClearSlot();
            return;
        }

        EquipmentInstance equipment =
            InventoryManager.Instance.GetItemAt(_slotIndex);

        if (equipment == null ||
            equipment.equipmentData == null)
        {
            ClearSlot();
            return;
        }

        _itemIcon.sprite = equipment.equipmentData.icon;
        _itemIcon.enabled = equipment.equipmentData.icon != null;
    }

    private void ClearSlot()
    {
        _itemIcon.sprite = null;
        _itemIcon.enabled = false;
    }
}
