using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    public BoxController[] boxes;

    [Header("Level Goals")]
    public IngredientData[] requiredIngredients;

    [Header("Generation Settings")]
    public int triplesPerIngredient = 2;

    private List<IngredientData[]> generatedSets = new List<IngredientData[]>();
    private int nextSetIndex = 0;

    private void Start()
    {
        GenerateLevelSets();
        FillBoxes();
    }

    private void GenerateLevelSets()
    {
        generatedSets.Clear();

        if (requiredIngredients == null || requiredIngredients.Length == 0)
        {
            Debug.LogError("Required Ingredients пустой!");
            return;
        }

        List<IngredientData> allPieces = new List<IngredientData>();

        foreach (IngredientData ingredient in requiredIngredients)
        {
            if (ingredient == null) continue;

            for (int t = 0; t < triplesPerIngredient; t++)
            {
                allPieces.Add(ingredient);
                allPieces.Add(ingredient);
                allPieces.Add(ingredient);
            }
        }

        Shuffle(allPieces);

        int totalSetsNeeded = boxes.Length * 2;
        int pieceIndex = 0;

        for (int s = 0; s < totalSetsNeeded; s++)
        {
            IngredientData[] boxSet = new IngredientData[3];

            int count = Random.Range(0, 4);

            for (int i = 0; i < count && pieceIndex < allPieces.Count; i++)
            {
                boxSet[i] = allPieces[pieceIndex];
                pieceIndex++;
            }

            if (IsTripleSame(boxSet))
            {
                allPieces.Add(boxSet[2]);
                boxSet[2] = null;
            }

            generatedSets.Add(boxSet);
        }

        while (pieceIndex < allPieces.Count)
        {
            IngredientData[] boxSet = new IngredientData[3];

            int count = Random.Range(1, 4);

            for (int i = 0; i < count && pieceIndex < allPieces.Count; i++)
            {
                boxSet[i] = allPieces[pieceIndex];
                pieceIndex++;
            }

            if (IsTripleSame(boxSet))
            {
                allPieces.Add(boxSet[2]);
                boxSet[2] = null;
            }

            generatedSets.Add(boxSet);
        }

        Shuffle(generatedSets);
    }

    private void FillBoxes()
    {
        nextSetIndex = 0;

        foreach (BoxController box in boxes)
        {
            if (box == null)
                continue;

            box.SetIngredients(GetNextSet());
            box.SetPreview(GetNextSet());
        }
    }

    public void GiveNextPreviewToBox(BoxController box)
    {
        if (box == null)
            return;

        box.SetPreview(GetNextSet());
    }

    private IngredientData[] GetNextSet()
    {
        if (nextSetIndex >= generatedSets.Count)
            return new IngredientData[3];

        IngredientData[] set = generatedSets[nextSetIndex];
        nextSetIndex++;
        return set;
    }

    private bool IsTripleSame(IngredientData[] set)
    {
        if (set == null || set.Length < 3)
            return false;

        if (set[0] == null || set[1] == null || set[2] == null)
            return false;

        return set[0] == set[1] && set[1] == set[2];
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}