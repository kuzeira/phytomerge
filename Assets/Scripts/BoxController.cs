using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BoxController : MonoBehaviour
{
    public IngredientData[] ingredients = new IngredientData[3];
    public IngredientData[] previewIngredients = new IngredientData[3];

    public Image[] slots = new Image[3];
    public Image[] previewSlots = new Image[3];

    public NPCOrderUI npcOrderUI;

    private bool isMatching;

    private void Start()
    {
        RefreshUI();
        RefreshPreviewUI();
    }

    public IngredientData GetIngredientAt(int index)
    {
        if (index < 0 || index >= 3) return null;
        return ingredients[index];
    }

    public void SwapOrMoveIngredient(BoxController targetBox, int fromIndex, int targetIndex)
    {
        if (targetBox == null || isMatching || targetBox.isMatching) return;

        IngredientData fromIngredient = ingredients[fromIndex];
        IngredientData targetIngredient = targetBox.ingredients[targetIndex];

        if (fromIngredient == null) return;

        targetBox.ingredients[targetIndex] = fromIngredient;
        ingredients[fromIndex] = targetIngredient;

        RefreshUI();
        targetBox.RefreshUI();

        CheckMatch();
        targetBox.CheckMatch();

        RefillFromPreviewIfEmpty();
        targetBox.RefillFromPreviewIfEmpty();
    }

    public void SetIngredients(IngredientData[] newIngredients)
    {
        for (int i = 0; i < 3; i++)
            ingredients[i] = newIngredients != null && i < newIngredients.Length ? newIngredients[i] : null;

        RefreshUI();
    }

    public void SetPreview(IngredientData[] newPreview)
    {
        for (int i = 0; i < 3; i++)
            previewIngredients[i] = newPreview != null && i < newPreview.Length ? newPreview[i] : null;

        RefreshPreviewUI();
    }

    public void UsePreviewAsCurrent()
    {
        SetIngredients(previewIngredients);
        SetPreview(null);
    }

    public void RefreshUI()
    {
        for (int i = 0; i < 3; i++)
        {
            if (slots[i] == null) continue;

            slots[i].enabled = true;
            slots[i].raycastTarget = true;
            slots[i].transform.localScale = Vector3.one;

            if (ingredients[i] != null && ingredients[i].icon != null)
            {
                slots[i].sprite = ingredients[i].icon;
                slots[i].color = Color.white;
                slots[i].preserveAspect = true;
            }
            else
            {
                slots[i].sprite = null;
                slots[i].color = new Color(1, 1, 1, 0);
            }
        }
    }

    public void RefreshPreviewUI()
    {
        for (int i = 0; i < 3; i++)
        {
            if (previewSlots[i] == null) continue;

            if (previewIngredients[i] != null && previewIngredients[i].icon != null)
            {
                previewSlots[i].sprite = previewIngredients[i].icon;
                previewSlots[i].enabled = true;
                previewSlots[i].color = Color.white;
                previewSlots[i].preserveAspect = true;
            }
            else
            {
                previewSlots[i].sprite = null;
                previewSlots[i].enabled = false;
            }
        }
    }

    public bool CheckMatch()
    {
        if (isMatching) return false;

        if (ingredients[0] == null || ingredients[1] == null || ingredients[2] == null)
            return false;

        if (ingredients[0] == ingredients[1] && ingredients[1] == ingredients[2])
        {
            IngredientData matched = ingredients[0];

            if (npcOrderUI != null)
                npcOrderUI.OnIngredientMatched(matched);

            StartCoroutine(PlayMatchEffect());
            return true;
        }

        return false;
    }

    private IEnumerator PlayMatchEffect()
    {
        isMatching = true;

        foreach (Image slot in slots)
        {
            if (slot != null)
            {
                slot.color = Color.yellow;
                slot.transform.localScale = Vector3.one * 1.35f;
            }
        }

        yield return new WaitForSeconds(0.3f);

        ClearBox();
        RefillFromPreviewIfEmpty();

        isMatching = false;
    }

    public void ClearBox()
    {
        for (int i = 0; i < 3; i++)
            ingredients[i] = null;

        RefreshUI();
    }

    public bool IsEmpty()
    {
        for (int i = 0; i < 3; i++)
        {
            if (ingredients[i] != null)
                return false;
        }

        return true;
    }

    public bool HasPreview()
    {
        for (int i = 0; i < 3; i++)
        {
            if (previewIngredients[i] != null)
                return true;
        }

        return false;
    }

    public void RefillFromPreviewIfEmpty()
    {
        if (!IsEmpty()) return;

        if (HasPreview())
        {
            UsePreviewAsCurrent();

            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            if (levelManager != null)
                levelManager.GiveNextPreviewToBox(this);
        }
    }
}