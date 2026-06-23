using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryDragHandler :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    public static EquipmentInstance DraggedEquipment
    {
        get;
        private set;
    }

    public static int SourceInventorySlotIndex
    {
        get;
        private set;
    } = -1;

    private InventorySlotUI _inventorySlotUI;
    private RectTransform _slotRectTransform;
    private Canvas _rootCanvas;

    private GameObject _dragIconObject;
    private RectTransform _dragIconRectTransform;

    private void Awake()
    {
        _inventorySlotUI =
            GetComponent<InventorySlotUI>();

        _slotRectTransform =
            GetComponent<RectTransform>();

        Canvas canvas =
            GetComponentInParent<Canvas>();

        if (canvas != null)
        {
            _rootCanvas = canvas.rootCanvas;
        }
    }

    public void OnBeginDrag(
        PointerEventData eventData
    )
    {
        if (_inventorySlotUI == null ||
            InventoryManager.Instance == null ||
            _rootCanvas == null)
        {
            eventData.pointerDrag = null;
            return;
        }

        EquipmentInstance equipment =
            InventoryManager.Instance.GetItemAt(
                _inventorySlotUI.SlotIndex
            );

        if (equipment == null ||
            equipment.equipmentData == null ||
            equipment.equipmentData.icon == null)
        {
            eventData.pointerDrag = null;
            return;
        }

        DraggedEquipment = equipment;

        SourceInventorySlotIndex =
            _inventorySlotUI.SlotIndex;

        CreateDragIcon(
            equipment.equipmentData.icon
        );

        MoveDragIcon(eventData);

        Debug.Log(
            "Started dragging " +
            equipment.GetDisplayName() +
            " from inventory slot " +
            SourceInventorySlotIndex
        );
    }

    public void OnDrag(
        PointerEventData eventData
    )
    {
        MoveDragIcon(eventData);
    }

    public void OnEndDrag(
        PointerEventData eventData
    )
    {
        DestroyDragIcon();

        DraggedEquipment = null;
        SourceInventorySlotIndex = -1;

        Debug.Log(
            "Finished dragging inventory item."
        );
    }

    private void CreateDragIcon(
        Sprite equipmentIcon
    )
    {
        _dragIconObject = new GameObject(
            "Inventory Drag Icon",
            typeof(RectTransform),
            typeof(CanvasGroup),
            typeof(Image)
        );

        _dragIconObject.transform.SetParent(
            _rootCanvas.transform,
            false
        );

        _dragIconRectTransform =
            _dragIconObject.GetComponent<RectTransform>();

        float iconWidth = Mathf.Max(
            1f,
            _slotRectTransform.rect.width - 30f
        );

        float iconHeight = Mathf.Max(
            1f,
            _slotRectTransform.rect.height - 30f
        );

        _dragIconRectTransform.sizeDelta =
            new Vector2(
                iconWidth,
                iconHeight
            );

        Image dragImage =
            _dragIconObject.GetComponent<Image>();

        dragImage.sprite = equipmentIcon;
        dragImage.preserveAspect = true;
        dragImage.raycastTarget = false;

        CanvasGroup canvasGroup =
            _dragIconObject.GetComponent<CanvasGroup>();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        _dragIconObject.transform.SetAsLastSibling();
    }

    private void MoveDragIcon(
        PointerEventData eventData
    )
    {
        if (_dragIconRectTransform == null ||
            _rootCanvas == null)
        {
            return;
        }

        RectTransform canvasRectTransform =
            _rootCanvas.transform as RectTransform;

        bool foundPosition =
            RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    canvasRectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 localPosition
                );

        if (foundPosition)
        {
            _dragIconRectTransform.localPosition =
                localPosition;
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