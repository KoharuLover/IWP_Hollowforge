using UnityEngine;
using UnityEngine.UI;

public enum EquipmentUISlotType
{
    Weapon,
    Helmet,
    Chestplate,
    Pants,
    Boots,
    Artifact
}

public class EquipmentSlotUI : MonoBehaviour
{
    [Header("Slot Settings")]
    [SerializeField] private EquipmentUISlotType _slotType;
    [SerializeField] private int _slotIndex;

    [Header("UI References")]
    [SerializeField] private Image _itemIcon;

    private void Start()
    {
        if (EquipmentManager.Instance != null)
        {
            EquipmentManager.Instance.EquipmentChanged += Refresh;
        }
        else
        {
            Debug.LogWarning("EquipmentSlotUI could not find the EquipmentManager.", gameObject);
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (EquipmentManager.Instance != null)
        {
            EquipmentManager.Instance.EquipmentChanged -= Refresh;
        }
    }

    public void Refresh()
    {
        EquipmentInstance equipment = GetEquippedItem();

        if (equipment == null || equipment.equipmentData == null)
        {
            ClearSlot();
            return;
        }

        if (_itemIcon != null)
        {
            _itemIcon.sprite = equipment.equipmentData.icon;
            _itemIcon.enabled = equipment.equipmentData.icon != null;
        }
    }

    private EquipmentInstance GetEquippedItem()
    {
        if (EquipmentManager.Instance == null)
        {
            return null;
        }

        if (_slotType == EquipmentUISlotType.Weapon)
        {
            if (_slotIndex == 0)
            {
                return EquipmentManager.Instance.WeaponSlot1;
            }

            if (_slotIndex == 1)
            {
                return EquipmentManager.Instance.WeaponSlot2;
            }
        }
        else if (_slotType == EquipmentUISlotType.Helmet)
        {
            return EquipmentManager.Instance.HelmetSlot;
        }
        else if (_slotType == EquipmentUISlotType.Chestplate)
        {
            return EquipmentManager.Instance.ChestplateSlot;
        }
        else if (_slotType == EquipmentUISlotType.Pants)
        {
            return EquipmentManager.Instance.PantsSlot;
        }
        else if (_slotType == EquipmentUISlotType.Boots)
        {
            return EquipmentManager.Instance.BootsSlot;
        }
        else if (_slotType == EquipmentUISlotType.Artifact)
        {
            EquipmentInstance[] artifactSlots = EquipmentManager.Instance.ArtifactSlots;

            if (_slotIndex >= 0 && _slotIndex < artifactSlots.Length)
            {
                return artifactSlots[_slotIndex];
            }
        }

        return null;
    }

    private void ClearSlot()
    {
        if (_itemIcon == null)
        {
            return;
        }

        _itemIcon.sprite = null;
        _itemIcon.enabled = false;
    }
}