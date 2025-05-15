using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOfLife : MonoBehaviour
{
    public GridManager gridManager; // Reference to GridManager
    public Button playPauseButton; // Play/Pause button
    public Button stepButton; // Step button
    public Button clearButton; // Clear button
    public TextMeshProUGUI generationText; // Generation counter
    private bool[,] nextGrid; // Temporary grid for next generation
    private bool isPlaying = false; // Toggle for automatic updates
    public float updateInterval = 0.5f; // Time between generations (seconds)
    private float timer = 0;
    public Button gliderButton;
    private int generationCount = 0; // Track generations
    public Button pulsarButton;

    void Start()
    {
        // Check for missing references
        if (gridManager == null)
        {
            Debug.LogError("GridManager is not assigned in GameOfLife!");
            return;
        }
        if (playPauseButton == null)
        {
            Debug.LogError("PlayPauseButton is not assigned in GameOfLife!");
            return;
        }
        if (stepButton == null)
        {
            Debug.LogError("StepButton is not assigned in GameOfLife!");
            return;
        }
        if (clearButton == null)
        {
            Debug.LogError("ClearButton is not assigned in GameOfLife!");
            return;
        }
        if (generationText == null)
        {
            Debug.LogError("GenerationText is not assigned in GameOfLife!");
            return;
        }
        if (gliderButton == null)
        {
            Debug.LogError("GliderButton is not assigned in GameOfLife!");
            return;
        }
        if (pulsarButton == null)
        {
            Debug.LogError("PulsarButton is not assigned in GameOfLife!");
            return;
        }

        // Initialize nextGrid
        nextGrid = new bool[gridManager.width, gridManager.height];
        gliderButton.onClick.AddListener(() => LoadGlider(10, 10));
        pulsarButton.onClick.AddListener(() => LoadPulsar(18, 18));
        playPauseButton.onClick.AddListener(TogglePlayPause);
        stepButton.onClick.AddListener(Step);
        clearButton.onClick.AddListener(ClearGrid);
        // Initialize UI
        UpdateGenerationText();
        playPauseButton.GetComponentInChildren<Text>().text = "Play";
    }

    void Update()
    {
        // Mouse click to toggle cells
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.FloorToInt(mousePos.x + gridManager.width / 2f);
            int y = Mathf.FloorToInt(mousePos.y + gridManager.height / 2f);
            if (x >= 0 && x < gridManager.width && y >= 0 && y < gridManager.height)
            {
                gridManager.grid[x, y] = !gridManager.grid[x, y];
                gridManager.UpdateCellVisual(x, y);
            }
        }
        // Automatic updates when playing
        if (isPlaying)
        {
            timer += Time.deltaTime;
            if (timer >= updateInterval)
            {
                ComputeNextGeneration();
                UpdateGrid();
                generationCount++;
                UpdateGenerationText();
                timer = 0;
            }
        }
    }

    void TogglePlayPause()
    {
        isPlaying = !isPlaying;
        playPauseButton.GetComponentInChildren<Text>().text = isPlaying ? "Pause" : "Play";
    }

    void Step()
    {
        ComputeNextGeneration();
        UpdateGrid();
        generationCount++;
        UpdateGenerationText();
    }

    void ClearGrid()
    {
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                gridManager.grid[x, y] = false;
                gridManager.UpdateCellVisual(x, y);
            }
        }
        generationCount = 0;
        UpdateGenerationText();
    }

    void ComputeNextGeneration()
    {
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                int neighbors = CountNeighbors(x, y);
                bool isAlive = gridManager.grid[x, y];
                nextGrid[x, y] = isAlive ? (neighbors == 2 || neighbors == 3) : (neighbors == 3);
            }
        }
    }

    int CountNeighbors(int x, int y)
    {
        int count = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                // Wrap-around boundaries
                int nx = (x + dx + gridManager.width) % gridManager.width;
                int ny = (y + dy + gridManager.height) % gridManager.height;
                count += gridManager.grid[nx, ny] ? 1 : 0;
            }
        }
        return count;
    }

    void UpdateGrid()
    {
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                gridManager.grid[x, y] = nextGrid[x, y];
                gridManager.UpdateCellVisual(x, y);
            }
        }
    }

    void UpdateGenerationText()
    {
        generationText.text = $"Generation: {generationCount}";
    }

    // Load a preset pattern (e.g., Glider)
    public void LoadGlider(int offsetX, int offsetY)
    {
        ClearGrid();
        bool[,] glider = new bool[3, 3] {
            { false, true,  false },
            { false, false, true  },
            { true,  true,  true  }
        };
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                int gx = offsetX + x;
                int gy = offsetY + y;
                if (gx < gridManager.width && gy < gridManager.height)
                {
                    gridManager.grid[gx, gy] = glider[x, y];
                    gridManager.UpdateCellVisual(gx, gy);
                }
            }
        }
    }
    public void LoadPulsar(int offsetX, int offsetY)
    {
        ClearGrid();
        bool[,] pulsar = new bool[13, 13] {
        { false, false, true,  true,  true,  false, false, false, true,  true,  true,  false, false },
        { false, false, false, false, false, false, false, false, false, false, false, false, false },
        { true,  false, false, false, false, true,  false, true,  false, false, false, false, true  },
        { true,  false, false, false, false, true,  false, true,  false, false, false, false, true  },
        { true,  false, false, false, false, true,  false, true,  false, false, false, false, true  },
        { false, false, true,  true,  true,  false, false, false, true,  true,  true,  false, false },
        { false, false, false, false, false, false, false, false, false, false, false, false, false },
        { false, false, true,  true,  true,  false, false, false, true,  true,  true,  false, false },
        { true,  false, false, false, false, true,  false, true,  false, false, false, false, true  },
        { true,  false, false, false, false, true,  false, true,  false, false, false, false, true  },
        { true,  false, false, false, false, true,  false, true,  false, false, false, false, true  },
        { false, false, false, false, false, false, false, false, false, false, false, false, false },
        { false, false, true,  true,  true,  false, false, false, true,  true,  true,  false, false }
    };
        for (int x = 0; x < 13; x++)
        {
            for (int y = 0; y < 13; y++)
            {
                int gx = offsetX + x;
                int gy = offsetY + y;
                if (gx < gridManager.width && gy < gridManager.height)
                {
                    gridManager.grid[gx, gy] = pulsar[x, y];
                    gridManager.UpdateCellVisual(gx, gy);
                }
            }
        }
    }
}