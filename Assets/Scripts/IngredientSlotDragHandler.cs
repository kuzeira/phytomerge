using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IngredientSlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
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
            GameObject dragIconObject = GameObject.Find("DragIcon");
            if (dragIconObject != null)
            {
                dragIcon = dragIconObject.GetComponent<Image>();
            }
        }

        if (dragIcon != null)
        {
            dragIcon.enabled = false;
            dragIcon.raycastTarget = false;
            dragIcon.color = new Color(1f, 1f, 1f, 1f);
            dragIcon.transform.localScale = Vector3.one;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (parentBox == null) return;

        draggedIngredient = parentBox.GetIngredientAt(slotIndex);

        if (draggedIngredient == null || dragIcon == null || draggedIngredient.icon == null)
        {
            isDragging = false;
            return;
        }

        isDragging = true;

        dragIcon.sprite = draggedIngredient.icon;
        dragIcon.enabled = true;
        dragIcon.preserveAspect = true;
        dragIcon.transform.position = eventData.position;
        dragIcon.transform.SetAsLastSibling();
        dragIcon.transform.localScale = Vector3.one * 1.35f;
        dragIcon.color = new Color(1f, 1f, 1f, 0.95f);

        if (slotImage != null)
        {
            slotImage.color = new Color(1f, 1f, 1f, 0.2f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || dragIcon == null) return;

        dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        BoxController targetBox = GetTargetBox(eventData);

        if (targetBox != null && targetBox != parentBox && targetBox.HasEmptySlot())
        {
            IngredientData moved = parentBox.RemoveIngredientAt(slotIndex);

            if (moved != null)
            {
                bool added = targetBox.AddIngredientToFirstEmpty(moved);

                if (!added)
                {
                    parentBox.ingredients[slotIndex] = moved;
                    parentBox.RefreshUI();
                }
            }
        }

        if (dragIcon != null)
        {
            dragIcon.enabled = false;
            dragIcon.sprite = null;
            dragIcon.color = new Color(1f, 1f, 1f, 1f);
            dragIcon.transform.localScale = Vector3.one;
        }

        if (slotImage != null)
        {
            slotImage.color = new Color(1f, 1f, 1f, 1f);
        }

        draggedIngredient = null;
        isDragging = false;
    }

    private BoxController GetTargetBox(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            BoxController box = result.gameObject.GetComponent<BoxController>();
            if (box != null)
                return box;

            box = result.gameObject.GetComponentInParent<BoxController>();
            if (box != null)
                return box;
        }

        return null;
    }
}