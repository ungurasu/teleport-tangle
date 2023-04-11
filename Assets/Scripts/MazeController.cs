using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour
{
    // Start is called before the first frame update
    int[,] _maze = new int[22 , 22];
    GameObject[,] _spriteObjectArrMaze             = new GameObject[11, 11];
    SpriteRenderer[,] _spriteRendererArrMaze       = new SpriteRenderer[11, 11];
    GameObject[,] _spriteObjectArrWallLeft         = new GameObject[11, 11];
    GameObject[,] _spriteObjectArrWallBottom       = new GameObject[11, 11];
    GameObject[,] _spriteObjectArrWallRight        = new GameObject[11, 11];
    GameObject[,] _spriteObjectArrWallTop          = new GameObject[11, 11];
    SpriteRenderer[,] _spriteRendererArrWallLeft   = new SpriteRenderer[11, 11];
    SpriteRenderer[,] _spriteRendererArrWallBottom = new SpriteRenderer[11, 11];
    SpriteRenderer[,] _spriteRendererArrWallRight  = new SpriteRenderer[11, 11];
    SpriteRenderer[,] _spriteRendererArrWallTop    = new SpriteRenderer[11, 11];
    [SerializeField] Sprite _spriteMazeTile;
    [SerializeField] Sprite _spriteWallLeft;
    [SerializeField] Sprite _spriteWallBottom;
    [SerializeField] Sprite _spriteWallRight;
    [SerializeField] Sprite _spriteWallTop;
    GameObject _objectMazeTilesContainer;
    GameObject _objectMazeWallsContainer;

    void Start()
    {
        int xCoord;
        int yCoord;
        float xPos;
        float yPos;

        _objectMazeTilesContainer = new GameObject("MazeTilesContainer");
        _objectMazeWallsContainer = new GameObject("MazeWallsContainer");

        //init maze array
        for (yCoord = 1; yCoord <= 21; yCoord++)
        {
            for (xCoord = 1; xCoord <= 21; xCoord++)
            {
                if (yCoord % 2 == 1 || xCoord % 2 == 1)
                {
                    _maze[yCoord, xCoord] = 1;
                }
                else
                {
                    _maze[yCoord, xCoord] = 0;
                }
            }
        }

        //init maze sprites game objects
        for (yCoord = 1; yCoord <= 10; yCoord++)
        {
            for (xCoord = 1; xCoord <= 10; xCoord++)
            {
                xPos = xCoord - 1;
                yPos = yCoord - 1;

                _spriteObjectArrMaze[yCoord, xCoord]                    = new GameObject(string.Format("MazeTileX{0}Y{1}", xCoord, yCoord));
                _spriteRendererArrMaze[yCoord, xCoord]                  = _spriteObjectArrMaze[yCoord, xCoord].AddComponent<SpriteRenderer>();
                _spriteRendererArrMaze[yCoord, xCoord].sprite           = _spriteMazeTile;
                _spriteRendererArrMaze[yCoord, xCoord].transform.parent = _spriteObjectArrMaze[yCoord, xCoord].transform;
                _spriteObjectArrMaze[yCoord, xCoord].transform.position = new Vector2(xPos, yPos);

                _spriteObjectArrMaze[yCoord, xCoord].transform.parent   = _objectMazeTilesContainer.transform;

                if (_maze[yCoord * 2 - 1, xCoord * 2] == 1) 
                {
                    _spriteObjectArrWallBottom[yCoord, xCoord]                      = new GameObject(string.Format("WallBottomX{0}Y{1}", xCoord, yCoord));
                    _spriteRendererArrWallBottom[yCoord, xCoord]                    = _spriteObjectArrWallBottom[yCoord, xCoord].AddComponent<SpriteRenderer>();
                    _spriteRendererArrWallBottom[yCoord, xCoord].sprite             = _spriteWallBottom;
                    _spriteRendererArrWallBottom[yCoord, xCoord].transform.parent   = _spriteObjectArrWallBottom[yCoord, xCoord].transform;
                    _spriteObjectArrWallBottom[yCoord, xCoord].transform.position   = new Vector2(xPos, yPos);

                    _spriteObjectArrWallBottom[yCoord, xCoord].transform.parent = _objectMazeWallsContainer.transform;
                }
                if (_maze[yCoord * 2 + 1, xCoord * 2] == 1)
                {
                    _spriteObjectArrWallTop[yCoord, xCoord] = new GameObject(string.Format("WallTopX{0}Y{1}", xCoord, yCoord));
                    _spriteRendererArrWallTop[yCoord, xCoord] = _spriteObjectArrWallTop[yCoord, xCoord].AddComponent<SpriteRenderer>();
                    _spriteRendererArrWallTop[yCoord, xCoord].sprite = _spriteWallTop;
                    _spriteRendererArrWallTop[yCoord, xCoord].transform.parent = _spriteObjectArrWallTop[yCoord, xCoord].transform;
                    _spriteObjectArrWallTop[yCoord, xCoord].transform.position = new Vector2(xPos, yPos);

                    _spriteObjectArrWallTop[yCoord, xCoord].transform.parent = _objectMazeWallsContainer.transform;
                }
                if (_maze[yCoord * 2, xCoord * 2 - 1] == 1)
                {
                    _spriteObjectArrWallLeft[yCoord, xCoord] = new GameObject(string.Format("WallLeftX{0}Y{1}", xCoord, yCoord));
                    _spriteRendererArrWallLeft[yCoord, xCoord] = _spriteObjectArrWallLeft[yCoord, xCoord].AddComponent<SpriteRenderer>();
                    _spriteRendererArrWallLeft[yCoord, xCoord].sprite = _spriteWallLeft;
                    _spriteRendererArrWallLeft[yCoord, xCoord].transform.parent = _spriteObjectArrWallLeft[yCoord, xCoord].transform;
                    _spriteObjectArrWallLeft[yCoord, xCoord].transform.position = new Vector2(xPos, yPos);

                    _spriteObjectArrWallLeft[yCoord, xCoord].transform.parent = _objectMazeWallsContainer.transform;
                }
                if (_maze[yCoord * 2, xCoord * 2 + 1] == 1)
                {
                    _spriteObjectArrWallRight[yCoord, xCoord] = new GameObject(string.Format("WallRightX{0}Y{1}", xCoord, yCoord));
                    _spriteRendererArrWallRight[yCoord, xCoord] = _spriteObjectArrWallRight[yCoord, xCoord].AddComponent<SpriteRenderer>();
                    _spriteRendererArrWallRight[yCoord, xCoord].sprite = _spriteWallRight;
                    _spriteRendererArrWallRight[yCoord, xCoord].transform.parent = _spriteObjectArrWallRight[yCoord, xCoord].transform;
                    _spriteObjectArrWallRight[yCoord, xCoord].transform.position = new Vector2(xPos, yPos);

                    _spriteObjectArrWallRight[yCoord, xCoord].transform.parent = _objectMazeWallsContainer.transform;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int xCoord;
        int yCoord;
        float xPos;
        float yPos;

        for (yCoord = 1; yCoord <= 10; yCoord++)
        {
            for (xCoord = 1; xCoord <= 10; xCoord++)
            {
            }
        }
    }
}
