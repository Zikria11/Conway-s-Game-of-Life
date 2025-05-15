using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 50;
    public int height = 50; 
    public GameObject cellPrefab;
    public bool[,] grid; 
    private GameObject[,] cellObjects; 

    void Start()
    {
        grid = new bool[width, height];
        cellObjects = new GameObject[width, height];
        InitializeGrid();
    }

    void InitializeGrid()
    {
        float offsetX = width / 2f;
        float offsetY = height / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x - offsetX + 0.5f, y - offsetY + 0.5f, 0);
                cellObjects[x, y] = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                grid[x, y] = Random.value > 0.8f;
                UpdateCellVisual(x, y);
            }
        }
    }

    public void UpdateCellVisual(int x, int y)
    {
        if (cellObjects[x, y] != null)
        {
            cellObjects[x, y].GetComponent<SpriteRenderer>().enabled = grid[x, y];
        }
    }
}