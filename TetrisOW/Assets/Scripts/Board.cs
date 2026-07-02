using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI; 
using TMPro;

public enum GameState { Playing, Paused, GameOver } 

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominoes;
    public Vector3Int spawnPosition;
    public Vector2Int boardsize = new Vector2Int(10, 20);

    public List<TetrominoData> nextPiecesQueue = new List<TetrominoData>(); 
    
    public PieceSpawner pieceSpawner; 

    // --- Scoring and Game State variables ---
    private int score = 0;
    private int level = 1; 
    private int linesClearedTotal = 0;
    public GameState currentGameState = GameState.Playing; 
    public GameMenuManager.Difficulty currentDifficulty = GameMenuManager.Difficulty.Medium; 
    
    // UI Elements (Placeholder - you'd link these in the Inspector)
    [Header("UI Elements (Assign in Inspector)")]
    public TextMeshProUGUI scoreText; 
    public TMP_InputField inputField; 
    public Button submitButton; 
    public Button restartButton;

    public TextMeshProUGUI GameOverText;

    // --- Gameplay Constants ---
    private static readonly int[] lineScoreMultipliers = { 0, 100, 300, 500, 800 }; 
    private const int linesPerLevel = 5; 
    
    // Difficulty base values for step delay (gravity)
    private const float easyStepDelay = 1.0f;
    private const float mediumStepDelay = 0.8f;
    private const float hardStepDelay = 0.6f;
    
    // Difficulty base values for lock delay
    private const float easyLockDelay = 0.9f; 
    private const float mediumLockDelay = 0.7f;
    private const float hardLockDelay = 0.5f; 
    
    // Difficulty multipliers
    private float DifficultyMultiplier
    {
        get
        {
            switch (currentDifficulty)
            {
                case GameMenuManager.Difficulty.Easy:
                    return 0.8f;
                case GameMenuManager.Difficulty.Medium:
                    return 1.0f;
                case GameMenuManager.Difficulty.Hard:
                    return 1.5f;
                default:
                    return 1.0f;
            }
        }
    }
    
    private float GravityDelay
    {
        get
        {
            float baseDelay = currentDifficulty switch
            {
                GameMenuManager.Difficulty.Easy => easyStepDelay,
                GameMenuManager.Difficulty.Hard => hardStepDelay,
                _ => mediumStepDelay,
            };
            
            return Mathf.Max(0.1f, baseDelay * Mathf.Pow(0.9f, level - 1));
        }
    }

    private float LockDelay
    {
        get
        {
            return currentDifficulty switch
            {
                GameMenuManager.Difficulty.Easy => easyLockDelay,
                GameMenuManager.Difficulty.Hard => hardLockDelay,
                _ => mediumLockDelay,
            };
        }
    }

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardsize.x / 2, -this.boardsize.y / 2);
            return new RectInt(position, this.boardsize);
        }
    }

    // New function to check the next piece's data without modifying activePiece
    private bool IsNextPieceValid(TetrominoData data, Vector3Int position)
    {
        RectInt bounds = this.Bounds;
        for (int i = 0; i < data.cells.Length; i++)
        {
            Vector3Int tilepositon = (Vector3Int)data.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilepositon))
            {
                return false;
            }

            if (this.tilemap.HasTile(tilepositon))
            {
                return false;
            }
        }
        return true;
    }
    
    private void Awake()
    {
        if (GameMenuManager.selectedDifficulty != GameMenuManager.Difficulty.Medium) 
        {
            this.currentDifficulty = GameMenuManager.selectedDifficulty;
            Debug.Log("Board initialized with difficulty: " + this.currentDifficulty);
        }
        
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        if (this.pieceSpawner == null)
        {
            this.pieceSpawner = FindFirstObjectByType<PieceSpawner>();
        }
        
        for (int i = 0; i < this.tetrominoes.Length; i++)
        {
            this.tetrominoes[i].Initialize();
        }
        
        if (inputField != null) inputField.interactable = false;
        if (submitButton != null) submitButton.interactable = false;
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
            restartButton.gameObject.SetActive(false); 
            GameOverText.gameObject.SetActive(false);
        }
        UpdateScoreDisplay();
    }
    
    void Start()
    {
        StartNewGame();
    }
    
    private void StartNewGame()
    {
        score = 0;
        level = 1;
        linesClearedTotal = 0;
        currentGameState = GameState.Playing;
        
        this.tilemap.ClearAllTiles();
        nextPiecesQueue.Clear();
        
        for (int i = 0; i < 5; i++)
        { 
            GenerateNextPiece(); 
        }
        
        if (pieceSpawner != null)
        {
            pieceSpawner.UpdateDisplay(nextPiecesQueue);
        }
        
        if (inputField != null) inputField.interactable = false;
        if (submitButton != null) submitButton.interactable = false;
        if (restartButton != null) restartButton.gameObject.SetActive(false);
        if (GameOverText != null) GameOverText.gameObject.SetActive(false);
        
        UpdateScoreDisplay();
        Spawnpiece();
    }

    private void GenerateNextPiece()
    {
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = this.tetrominoes[random];
        nextPiecesQueue.Add(data);
    }

    public void Spawnpiece()
    {
        if (currentGameState != GameState.Playing)
        {
            return;
        }
        
        if (nextPiecesQueue.Count == 0)
        {
            GenerateNextPiece(); 
        }
        
        TetrominoData data = nextPiecesQueue[0];
        nextPiecesQueue.RemoveAt(0); 
        
        GenerateNextPiece();

        // 1. Check for Game Over using the new piece's data BEFORE initialization.
        if (!IsNextPieceValid(data, this.spawnPosition))
        {
            // If Game Over, just set the state and return. DO NOT initialize or render the piece.
            GameOver();
            return; 
        }

        // 2. If valid, initialize and render the piece.
        this.activePiece.stepDelay = GravityDelay;
        this.activePiece.lockDelay = LockDelay;
        this.activePiece.Initialize(this, spawnPosition, data);

        Set(this.activePiece);
        
        if (pieceSpawner != null)
        {
            pieceSpawner.UpdateDisplay(nextPiecesQueue);
        }
    }
    
    private void AddScore(int lines)
    {
        if (lines > 0 && lines <= 4)
        {
            int baseScore = lineScoreMultipliers[lines];
            
            int points = Mathf.RoundToInt(baseScore * level * DifficultyMultiplier); 
            
            score += points;
            linesClearedTotal += lines;
            
            int newLevel = (linesClearedTotal / linesPerLevel) + 1;
            if (newLevel > level) 
            {
                level = newLevel;
                this.activePiece.stepDelay = GravityDelay;
                Debug.Log("Leveled up to: " + level + ". New Step Delay: " + GravityDelay);
            }
            
            UpdateScoreDisplay();
        }
    }
    
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString(); 
        }
    }

    private void GameOver()
    {
        currentGameState = GameState.GameOver;
        Debug.Log("Game Over! Final Score: " + score);

        if (LeaderBoard.Instance != null)
        {
            LeaderBoard.Instance.SetScore(score);
        }

        if (inputField != null) inputField.interactable = true;
        if (submitButton != null) submitButton.interactable = true;
        if (restartButton != null) restartButton.gameObject.SetActive(true); 
        if (GameOverText != null) GameOverText.gameObject.SetActive(true);
}
    
    public void RestartGame()
    {
        StartNewGame();
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds;
        
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilepositon = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilepositon))
            {
                return false;
            }

            if (this.tilemap.HasTile(tilepositon))
            {
                return false;
            }
        }
        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;
        int linesClearedInStep = 0; 

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                linesClearedInStep++; 
            }
            else
            {
                row++;
            }
        }
        
        if (linesClearedInStep > 0)
        {
            AddScore(linesClearedInStep);
        }
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!this.tilemap.HasTile(position))
            {
                return false;
            }
        }
        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(position, null);
        }
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = this.tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(position, above);
            }
            row++;
        }
    }
}