using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point 
{
    public int x;
    public int y;

    public Point(int initX, int initY)
    {
        x = initX;
        y = initY;
    }
}

public class MazeController : MonoBehaviour
{

    [SerializeField] int _mazeSize = 10;
    // Start is called before the first frame update
    int[,] _maze = new int[2002 ,2002];
    GameObject[,] _spriteObjectArrMaze             = new GameObject[1001, 1001];
    SpriteRenderer[,] _spriteRendererArrMaze       = new SpriteRenderer[1001, 1001];
    GameObject[,] _spriteObjectArrWallLeft         = new GameObject[1001, 1001];
    GameObject[,] _spriteObjectArrWallBottom       = new GameObject[1001, 1001];
    GameObject[,] _spriteObjectArrWallRight        = new GameObject[1001, 1001];
    GameObject[,] _spriteObjectArrWallTop          = new GameObject[1001, 1001];
    SpriteRenderer[,] _spriteRendererArrWallLeft   = new SpriteRenderer[1001, 1001];
    SpriteRenderer[,] _spriteRendererArrWallBottom = new SpriteRenderer[1001, 1001];
    SpriteRenderer[,] _spriteRendererArrWallRight  = new SpriteRenderer[1001, 1001];
    SpriteRenderer[,] _spriteRendererArrWallTop    = new SpriteRenderer[1001, 1001];
    [SerializeField] Sprite _spriteMazeTile;
    [SerializeField] Sprite _spriteWallLeft;
    [SerializeField] Sprite _spriteWallBottom;
    [SerializeField] Sprite _spriteWallRight;
    [SerializeField] Sprite _spriteWallTop;
    GameObject _objectMazeTilesContainer;
    GameObject _objectMazeWallsContainer;

    void PartitionMaze() {
        bool[,] visited = new bool[1001, 1001];
        int yCoord;
        int xCoord;
        int wallsListLenght;
        Point[] wallsList = new Point[4001];
        Point[] wallsListTemp = new Point[4001];

        for (yCoord = 1; yCoord <= _mazeSize; yCoord++)
        {
            for (xCoord = 1; xCoord <= _mazeSize; xCoord++)
            {
                visited[yCoord, xCoord] = false;
            }
        }

        visited[1, 1] = true;
        wallsList[0] = new Point(2, 3);
        wallsList[1] = new Point(3, 2);
        wallsListLenght = 2;

        while (wallsListLenght> 0)
        {
            int index = Random.Range(0, wallsListLenght);

            Point currentWall = wallsList[index];

            System.Array.Copy(wallsList, 0, wallsListTemp, 0, index);
            if (index < wallsList.Length)
            {
                System.Array.Copy(wallsList, index + 1, wallsListTemp, index, wallsList.Length - index - 1);
            }
            wallsList = wallsListTemp;
            wallsListLenght--;

            if (currentWall.x % 2 == 1 && currentWall.x > 1 && currentWall.x < _mazeSize * 2 + 1) // vertical wall
            {
                if (visited[currentWall.y / 2, (currentWall.x - 1) / 2] == false && visited[currentWall.y / 2, (currentWall.x + 1) / 2] == true) // left cell
                {
                    visited[currentWall.y / 2, (currentWall.x - 1) / 2] = true;
                    _maze[currentWall.y, currentWall.x] = 0;
                    wallsList[wallsListLenght] = new Point(currentWall.x - 1, currentWall.y + 1); // above wall
                    wallsList[wallsListLenght + 1] = new Point(currentWall.x - 1, currentWall.y - 1); // bottom wall
                    wallsList[wallsListLenght + 2] = new Point(currentWall.x - 2, currentWall.y); // left wall
                    wallsListLenght += 3;
                }
                else if (visited[currentWall.y / 2, (currentWall.x - 1) / 2] == true && visited[currentWall.y / 2, (currentWall.x + 1) / 2] == false) // right cell
                {
                    visited[currentWall.y / 2, (currentWall.x + 1) / 2] = true;
                    _maze[currentWall.y, currentWall.x] = 0;
                    wallsList[wallsListLenght] = new Point(currentWall.x + 1, currentWall.y + 1); // above wall
                    wallsList[wallsListLenght + 1] = new Point(currentWall.x + 1, currentWall.y - 1); // bottom wall
                    wallsList[wallsListLenght + 2] = new Point(currentWall.x + 2, currentWall.y); // right wall
                    wallsListLenght += 3;
                }
            }
            else if (currentWall.y % 2 == 1 && currentWall.y > 1 && currentWall.y < _mazeSize * 2 + 1)// horizontal wall
            {
                if (visited[(currentWall.y - 1) / 2, currentWall.x / 2] == false && visited[(currentWall.y + 1) / 2, currentWall.x / 2] == true) // bottom cell
                {
                    visited[(currentWall.y - 1)/ 2, currentWall.x / 2] = true;
                    _maze[currentWall.y, currentWall.x] = 0;
                    wallsList[wallsListLenght] = new Point(currentWall.x, currentWall.y - 2); // bottom wall
                    wallsList[wallsListLenght + 1] = new Point(currentWall.x + 1, currentWall.y - 1); // right wall
                    wallsList[wallsListLenght + 2] = new Point(currentWall.x - 1, currentWall.y - 1); // left wall
                    wallsListLenght += 3;
                }
                else if (visited[(currentWall.y - 1) / 2, currentWall.x / 2] == true && visited[(currentWall.y + 1) / 2, currentWall.x / 2] == false) // top cell
                {
                    visited[(currentWall.y + 1) / 2, currentWall.x / 2] = true;
                    _maze[currentWall.y, currentWall.x] = 0;
                    wallsList[wallsListLenght] = new Point(currentWall.x, currentWall.y + 2); // top wall
                    wallsList[wallsListLenght + 1] = new Point(currentWall.x + 1, currentWall.y + 1); // right wall
                    wallsList[wallsListLenght + 2] = new Point(currentWall.x - 1, currentWall.y + 1); // left wall
                    wallsListLenght += 3;
                }
            }
        }
    }

    void Start()
    {
        int xCoord;
        int yCoord;
        float xPos;
        float yPos;

        _objectMazeTilesContainer = new GameObject("MazeTilesContainer");
        _objectMazeWallsContainer = new GameObject("MazeWallsContainer");

        //init maze array
        for (yCoord = 1; yCoord <= _mazeSize*2 + 1; yCoord++)
        {
            for (xCoord = 1; xCoord <= _mazeSize * 2 + 1; xCoord++)
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

        //partition maze
        PartitionMaze();

        //init maze sprites game objects
        for (yCoord = 1; yCoord <= _mazeSize; yCoord++)
        {
            for (xCoord = 1; xCoord <= _mazeSize; xCoord++)
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
                    _spriteObjectArrWallBottom[yCoord, xCoord].transform.position   = new Vector3(xPos, yPos, -0.1f);

                    _spriteObjectArrWallBottom[yCoord, xCoord].transform.parent = _objectMazeWallsContainer.transform;
                }
                if (_maze[yCoord * 2 + 1, xCoord * 2] == 1)
                {
                    _spriteObjectArrWallTop[yCoord, xCoord] = new GameObject(string.Format("WallTopX{0}Y{1}", xCoord, yCoord));
                    _spriteRendererArrWallTop[yCoord, xCoord] = _spriteObjectArrWallTop[yCoord, xCoord].AddComponent<SpriteRenderer>();
                    _spriteRendererArrWallTop[yCoord, xCoord].sprite = _spriteWallTop;
                    _spriteRendererArrWallTop[yCoord, xCoord].transform.parent = _spriteObjectArrWallTop[yCoord, xCoord].transform;
                    _spriteObjectArrWallTop[yCoord, xCoord].transform.position = new Vector3(xPos, yPos, -0.1f);

                    _spriteObjectArrWallTop[yCoord, xCoord].transform.parent = _objectMazeWallsContainer.transform;
                }
                if (_maze[yCoord * 2, xCoord * 2 - 1] == 1)
                {
                    _spriteObjectArrWallLeft[yCoord, xCoord] = new GameObject(string.Format("WallLeftX{0}Y{1}", xCoord, yCoord));
                    _spriteRendererArrWallLeft[yCoord, xCoord] = _spriteObjectArrWallLeft[yCoord, xCoord].AddComponent<SpriteRenderer>();
                    _spriteRendererArrWallLeft[yCoord, xCoord].sprite = _spriteWallLeft;
                    _spriteRendererArrWallLeft[yCoord, xCoord].transform.parent = _spriteObjectArrWallLeft[yCoord, xCoord].transform;
                    _spriteObjectArrWallLeft[yCoord, xCoord].transform.position = new Vector3(xPos, yPos, -0.1f);

                    _spriteObjectArrWallLeft[yCoord, xCoord].transform.parent = _objectMazeWallsContainer.transform;
                }
                if (_maze[yCoord * 2, xCoord * 2 + 1] == 1)
                {
                    _spriteObjectArrWallRight[yCoord, xCoord] = new GameObject(string.Format("WallRightX{0}Y{1}", xCoord, yCoord));
                    _spriteRendererArrWallRight[yCoord, xCoord] = _spriteObjectArrWallRight[yCoord, xCoord].AddComponent<SpriteRenderer>();
                    _spriteRendererArrWallRight[yCoord, xCoord].sprite = _spriteWallRight;
                    _spriteRendererArrWallRight[yCoord, xCoord].transform.parent = _spriteObjectArrWallRight[yCoord, xCoord].transform;
                    _spriteObjectArrWallRight[yCoord, xCoord].transform.position = new Vector3(xPos, yPos, -0.1f);

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
