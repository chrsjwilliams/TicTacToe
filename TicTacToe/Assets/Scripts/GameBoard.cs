using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoardInfo
{
    public int col;
    public int row;
}

public class GameBoard : MonoBehaviour
{
    private float _tileOffset = 1.6f;
    public TileSpace[,] gameBoard;

    [SerializeField] public TileSpace tile;

    private TaskManager _tm = new TaskManager();
    public void Init(BoardInfo info)
    {
        // TODO: Board Entry Animation
        gameBoard = new TileSpace[info.col, info.row];
        for (int x = 0; x < info.col; x++)
        {
            for (int y = 0; y < info.row; y++)
            {
                float xPos = x * _tileOffset;
                float yPos = y * _tileOffset;
                Vector3 tilePos = new Vector3(xPos, yPos, 0);
                TileSpace tile = Instantiate(Services.Prefabs.Tile, tilePos, Quaternion.identity, transform);
                tile.name = "TileSpace X: [" + x + ", " + y + "]";
                gameBoard[x, y] = tile;
                tile.gameObject.SetActive(false);
            }
        }

        Task entryAnimation = new BoardEntryAnimation();
        _tm.Do(entryAnimation);
    }

    

// Update is called once per frame
    void Update()
    {
        _tm.Update();
    }  
    
}
