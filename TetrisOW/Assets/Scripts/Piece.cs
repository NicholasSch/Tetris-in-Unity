using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Piece : MonoBehaviour
{

    public Board board { get; private set; }
    public TetrominoData data { get; private set; }

    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    public int rotationIndex { get; private set; }

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;

    private float stepTime;

    private float lockTime;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;
        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;

        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }
    void Start()
    {

    }

    void Update()
    {
        if (board != null && board.currentGameState == GameState.Playing)
        {
            this.lockTime += Time.deltaTime;

            if (Time.time >= this.stepTime)
            {
                Step();
            }
        }
    }

    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay;

        Move(Vector2Int.down);

        if (this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }

    private void Lock()
    {
        this.board.Set(this);
        this.board.ClearLines();
        this.board.Spawnpiece();
    }

    public void OnLeft(InputAction.CallbackContext context)
    {
        if (context.performed && board.currentGameState==GameState.Playing)
        {
            Move(Vector2Int.left);
        }

    }
    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.performed && board.currentGameState==GameState.Playing)
        {
            Move(Vector2Int.right);
        }

    }
    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.performed && board.currentGameState==GameState.Playing)
        {
            Move(Vector2Int.down);
        }
    }

    public void OnHardDrop(InputAction.CallbackContext context)
    {
        if (context.performed && board.currentGameState==GameState.Playing)
        {
            while (Move(Vector2Int.down))
            {
                continue;
            }
            Lock();
        }
    }
    public void OnRotateRight(InputAction.CallbackContext context)
    {
        if (context.performed && board.currentGameState==GameState.Playing)
        {
            Rotate(1);
        }
    }
    public void OnRotateLeft(InputAction.CallbackContext context)
    {
        if (context.performed && board.currentGameState==GameState.Playing)
        {
            Rotate(-1);
        }
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newposition = this.position;
        newposition.x += translation.x;
        newposition.y += translation.y;

        this.board.Clear(this);

        bool valid = this.board.IsValidPosition(this, newposition);

        if (valid)
        {
            this.position = newposition;
            this.lockTime = 0f;
        }

        this.board.Set(this); 
        return valid;
    }


public void Rotate(int direction)
{
    this.board.Clear(this);

    int originalRotationIndex = this.rotationIndex;
    Vector3Int[] originalCells = (Vector3Int[])this.cells.Clone();

    this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);
    ApplyRotationMatrix(direction);

    if (!TestWallKicks(this.rotationIndex, direction))
    {
        this.rotationIndex = originalRotationIndex;
        this.cells = originalCells;
    }

    this.board.Set(this); 
}


    private void ApplyRotationMatrix(int direction)
    { 
        
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];

            int x, y;

            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            this.cells[i] = new Vector3Int(x, y, 0);

        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
{
    int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

    for (int i = 0; i < data.wallKicks.GetLength(1); i++)
    {
        Vector2Int translation = this.data.wallKicks[wallKickIndex, i];

        Vector3Int testPosition = this.position + (Vector3Int)translation;

        if (this.board.IsValidPosition(this, testPosition))
        {
            this.position = testPosition;
            return true;
        }
    }
    return false;
}


    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

}