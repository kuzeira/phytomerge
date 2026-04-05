using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(BoxController))]
public class BoxDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private BoxController boxController;

    [Header("Drag UI")]
    public Canvas canvas;
    public Image dragIcon;

    private IngredientData draggedIngredient;

    private void Awake()
    {
        boxController = GetComponent<BoxController>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedIngredient = boxController.PeekTopIngredient();

        if (draggedIngredient == null || dragIcon == null)
            return;

        dragIcon.sprite = draggedIngredient.icon;
        dragIcon.enabled = true;
        dragIcon.preserveAspect = true;
        dragIcon.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedIngredient == null || dragIcon == null)
            return;

        dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedIngredient == null)
            return;

        BoxController targetBox = GetTargetBox(eventData);

        if (targetBox != null && targetBox != boxController && targetBox.CanAddIngredient())
        {
            IngredientData moved = boxController.RemoveTopIngredient();
            targetBox.AddIngredient(moved);
            Debug.Log($"Перенесён {moved.ingredientName} из {boxController.name} в {targetBox.name}");
        }
        else
        {
            Debug.Log("Целевая коробка не найдена или заполнена");
        }

        if (dragIcon != null)
        {
            dragIcon.enabled = false;
            dragIcon.sprite = null;
        }

        draggedIngredient = null;
    }

    private BoxController GetTargetBox(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            BoxController foundBox = result.gameObject.GetComponent<BoxController>();
            if (foundBox != null)
                return foundBox;

            foundBox = result.gameObject.GetComponentInParent<BoxController>();
            if (foundBox != null)
                return foundBox;
        }

        return null;
    }
}