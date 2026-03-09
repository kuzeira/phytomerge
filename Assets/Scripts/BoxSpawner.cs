using UnityEngine;

public class BoxSpawnerGrid : MonoBehaviour
{
    public GameObject boxPrefab;
    public int rows = 2;
    public int columns = 3;
    public Vector3 startPosition;
    public float spacingX = 2f;
    public float spacingY = 2f;

    void Start()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector3 pos = startPosition + new Vector3(c * spacingX, -r * spacingY, 0);
                Instantiate(boxPrefab, pos, Quaternion.identity);
            }
        }
    }
}
