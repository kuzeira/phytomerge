using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IngredientSlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public BoxController parentBox;
    public int slotIndex;
    public Image dragIcon;

    private IngredientData draggedIngredient;
    private bool isDragging;
    private Image slotImage;

    private void Start()
    {
        slotImage = GetComponent<Image>();

        if (dragIcon == null)
        {
            GameObject obj = GameObject.Find("DragIcon");
            if (obj != null)
                dragIcon = obj.GetComponent<Image>();
        }

        if (dragIcon != null)
        {
            dragIcon.enabled = false;
            dragIcon.raycastTarget = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (parentBox == null) return;

        draggedIngredient = parentBox.GetIngredientAt(slotIndex);

        if (draggedIngredient == null || dragIcon == null || draggedIngredient.icon == null)
            return;

        isDragging = true;

        dragIcon.sprite = draggedIngredient.icon;
        dragIcon.enabled = true;
        dragIcon.preserveAspect = true;
        dragIcon.color = new Color(1f, 1f, 1f, 0.95f);
        dragIcon.transform.position = eventData.position;
        dragIcon.transform.localScale = Vector3.one * 1.35f;
        dragIcon.transform.SetAsLastSibling();

        if (slotImage != null)
            slotImage.color = new Color(1f, 1f, 1f, 0.25f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || dragIcon == null) return;

        dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        IngredientSlotDragHandler targetSlot = GetTargetSlot(eventData);

        if (targetSlot != null && targetSlot.parentBox != null)
        {
            parentBox.SwapOrMoveIngredient(
                targetSlot.parentBox,
                slotIndex,
                targetSlot.slotIndex
            );
        }

        if (dragIcon != null)
        {
            dragIcon.enabled = false;
            dragIcon.sprite = null;
            dragIcon.transform.localScale = Vector3.one;
            dragIcon.color = Color.white;
        }

        parentBox.RefreshUI();

        if (targetSlot != null && targetSlot.parentBox != null)
            targetSlot.parentBox.RefreshUI();

        draggedIngredient = null;
        isDragging = false;
    }

    private IngredientSlotDragHandler GetTargetSlot(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            IngredientSlotDragHandler slot = result.gameObject.GetComponent<IngredientSlotDragHandler>();

            if (slot != null)
                return slot;

            slot = result.gameObject.GetComponentInParent<IngredientSlotDragHandler>();

            if (slot != null)
                return slot;
        }

        return null;
    }
}