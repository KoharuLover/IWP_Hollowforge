using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EquipmentSlotUI))]
public class EquipmentDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static EquipmentInstance DraggedEquipment{ get; private set; }

    public static EquipmentUISlotType SourceSlotType{ get; private set; }

    public static int SourceSlotIndex{ get; private set; } = -1;

    private EquipmentSlotUI _equipmentSlotUI;
    private RectTransform _slotRectTransform;
    private Canvas _rootCanvas;

    private GameObject _dragIconObject;
    private RectTransform _dragIconRectTransform;

    private void Awake()
    {
        _equipmentSlotUI = GetComponent<EquipmentSlotUI>();
        _slotRectTransform = GetComponent<RectTransform>();
        Canvas canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
        {
            _rootCanvas = canvas.rootCanvas;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_equipmentSlotUI == null || EquipmentManager.Instance == null || _rootCanvas == null)
        {
            eventData.pointerDrag = null;
            return;
        }

        if (EquipmentManager.Instance.CanModifyEquipment == false)
        {
            Debug.Log("Equipment cannot be changed during combat.");

            eventData.pointerDrag = null;
            return;
        }

        EquipmentInstance equipment = _equipmentSlotUI.GetEquippedItem();

        if (equipment == null || equipment.equipmentData == null || equipment.equipmentData.icon == null)
        {
            eventData.pointerDrag = null;
            return;
        }

        DraggedEquipment = equipment;
        SourceSlotType = _equipmentSlotUI.SlotType;
        SourceSlotIndex = _equipmentSlotUI.SlotIndex;

        CreateDragIcon(equipment.equipmentData.icon);

        MoveDragIcon(eventData);

        Debug.Log("Started dragging equipped item: " + equipment.GetDisplayName());
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveDragIcon(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DestroyDragIcon();

        DraggedEquipment = null;
        SourceSlotIndex = -1;

        Debug.Log("Finished dragging equipped item.");
    }

    private void CreateDragIcon(Sprite equipmentIcon)
    {
        _dragIconObject = new GameObject("Equipment Drag Icon", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));

        _dragIconObject.transform.SetParent(_rootCanvas.transform, false);
        _dragIconRectTransform = _dragIconObject.GetComponent<RectTransform>();

        float iconWidth = Mathf.Max(1f, _slotRectTransform.rect.width - 30f);
        float iconHeight = Mathf.Max(1f, _slotRectTransform.rect.height - 30f);
        _dragIconRectTransform.sizeDelta = new Vector2(iconWidth, iconHeight);

        Image dragImage = _dragIconObject.GetComponent<Image>();
        dragImage.sprite = equipmentIcon;
        dragImage.preserveAspect = true;
        dragImage.raycastTarget = false;

        CanvasGroup canvasGroup = _dragIconObject.GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        _dragIconObject.transform.SetAsLastSibling();
    }

    private void MoveDragIcon(PointerEventData eventData)
    {
        if (_dragIconRectTransform == null || _rootCanvas == null)
        {
            return;
        }

        RectTransform canvasRectTransform = _rootCanvas.transform as RectTransform;

        bool foundPosition = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPosition);

        if (foundPosition)
        {
            _dragIconRectTransform.localPosition = localPosition;
        }
    }

    private void DestroyDragIcon()
    {
        if (_dragIconObject != null)
        {
            Destroy(_dragIconObject);
        }

        _dragIconObject = null;
        _dragIconRectTransform = null;
    }
}
