using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public List<GameObject> ingredients = new List<GameObject>();

    // Вытаскиваем ингредиент
    public void ExtractIngredient()
    {
        if (ingredients.Count == 0)
        {
            Debug.Log("Коробка пуста!");
            return;
        }

        // Берём последний ингредиент
        GameObject ingredient = ingredients[ingredients.Count - 1];
        ingredients.RemoveAt(ingredients.Count - 1);

        // Можно делать анимацию или просто скрыть
        ingredient.SetActive(false);

        Debug.Log("Ингредиент извлечён: " + ingredient.name);
    }
}