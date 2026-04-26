using UnityEngine;
using UnityEngine.UI;

public class BoxController : MonoBehaviour
{
    [Header("Box Data")]
    public IngredientData[] ingredients = new IngredientData[3];

    [Header("UI")]
    public Image[] slots = new Image[3];

    [Header("NPC")]
    public NPCOrderUI npcOrderUI;

    private void Start()
    {
        PreventStartingMatch();
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (slots == null || slots.Length < 3)
        {
            Debug.LogError($"{gameObject.name}: slots не настроены");
            return;
        }

        if (ingredients == null || ingredients.Length < 3)
        {
            Debug.LogError($"{gameObject.name}: ingredients не настроены");
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            if (slots[i] == null)
            {
                Debug.LogError($"{gameObject.name}: Slot {i} не назначен");
                continue;
            }

            if (ingredients[i] != null && ingredients[i].icon != null)
            {
                slots[i].sprite = ingredients[i].icon;
                slots[i].enabled = true;
                slots[i].color = Color.white;
                slots[i].preserveAspect = true;
            }
            else
            {
                slots[i].sprite = null;
                slots[i].enabled = false;
            }
        }
    }

    public IngredientData GetIngredientAt(int index)
    {
        if (index < 0 || index >= ingredients.Length)
            return null;

        return ingredients[index];
    }

    public IngredientData RemoveIngredientAt(int index)
    {
        if (index < 0 || index >= ingredients.Length)
            return null;

        IngredientData ingredient = ingredients[index];
        ingredients[index] = null;
        RefreshUI();
        return ingredient;
    }

    public bool HasEmptySlot()
    {
        for (int i = 0; i < ingredients.Length; i++)
        {
            if (ingredients[i] == null)
                return true;
        }

        return false;
    }

    public bool AddIngredientToFirstEmpty(IngredientData ingredient)
    {
        if (ingredient == null)
            return false;

        for (int i = 0; i < ingredients.Length; i++)
        {
            if (ingredients[i] == null)
            {
                ingredients[i] = ingredient;
                RefreshUI();
                CheckMatch();
                return true;
            }
        }

        return false;
    }

    public bool CheckMatch()
    {
        if (ingredients[0] == null || ingredients[1] == null || ingredients[2] == null)
            return false;

        if (ingredients[0] == ingredients[1] && ingredients[1] == ingredients[2])
        {
            IngredientData matchedIngredient = ingredients[0];
            Debug.Log($"MATCH! В коробке {gameObject.name} собрано 3 x {matchedIngredient.ingredientName}");

            if (npcOrderUI != null)
            {
                npcOrderUI.OnIngredientMatched(matchedIngredient);
            }

            ClearBox();
            return true;
        }

        return false;
    }

    public void ClearBox()
    {
        for (int i = 0; i < ingredients.Length; i++)
        {
            ingredients[i] = null;
        }

        RefreshUI();
    }

    private void PreventStartingMatch()
    {
        if (ingredients[0] == null || ingredients[1] == null || ingredients[2] == null)
            return;

        if (ingredients[0] == ingredients[1] && ingredients[1] == ingredients[2])
        {
            Debug.LogWarning($"{gameObject.name}: стартовая тройка обнаружена. Третий слот очищен.");
            ingredients[2] = null;
        }
    }
}