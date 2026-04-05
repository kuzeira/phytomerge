using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxController : MonoBehaviour
{
    public List<IngredientData> ingredients = new List<IngredientData>();
    public Image[] slots;

    private void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < ingredients.Count && ingredients[i] != null && ingredients[i].icon != null)
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

    public bool CanAddIngredient()
    {
        return ingredients.Count < 3;
    }

    public IngredientData PeekTopIngredient()
    {
        if (ingredients.Count == 0) return null;
        return ingredients[ingredients.Count - 1];
    }

    public IngredientData RemoveTopIngredient()
    {
        if (ingredients.Count == 0) return null;

        IngredientData top = ingredients[ingredients.Count - 1];
        ingredients.RemoveAt(ingredients.Count - 1);
        RefreshUI();
        return top;
    }

    public void AddIngredient(IngredientData ingredient)
    {
        if (ingredient == null || !CanAddIngredient()) return;

        ingredients.Add(ingredient);
        RefreshUI();
    }
}