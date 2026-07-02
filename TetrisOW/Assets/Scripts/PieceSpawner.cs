using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PieceSpawner : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    
    public Vector3Int initialSpawnPosition = new Vector3Int(12, 4, 0); 
    public int verticalOffset = 3; 

    void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
    }

    public void UpdateDisplay(List<TetrominoData> queue)
    {
        if (this.tilemap == null) return;

        this.tilemap.ClearAllTiles();

        Vector3Int currentDrawPosition = initialSpawnPosition;
        
        for (int i = 0; i < queue.Count; i++)
        {
            TetrominoData data = queue[i];
            
            for (int j = 0; j < data.cells.Length; j++)
            {
                Vector3Int cellPosition = (Vector3Int)data.cells[j] + currentDrawPosition;
                
                this.tilemap.SetTile(cellPosition, data.tile); 
            }

            currentDrawPosition.y -= verticalOffset; 
        }
    }
}