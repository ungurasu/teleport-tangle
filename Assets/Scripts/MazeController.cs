using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point 
{
    int x;
    int y;

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

    void PartitionMazeGood() {
        bool[,] visited = new bool[1001, 1001];
        int yCoord;
        int xCoord;
        Stack<Point> stackWalls = new Stack<Point>();

        for (yCoord = 1; yCoord <= _mazeSize; yCoord++)
        {
            for (xCoord = 1; xCoord <= _mazeSize; xCoord++)
            {
                visited[yCoord, xCoord] = false;
            }
        }

        visited[1, 1] = true;
        stackWalls.Push(new Point(3, 2));
        stackWalls.Push(new Point(2, 3));

        while (stackWalls.Count > 0)
        {
            Point crntPoint = stackWalls.Pop();
        }
    }

    void PartitionMaze(int topLeftX, int topLeftY, int botRightX, int botRightY, bool horizontal)
    {
        int doorCoord;
        int fixdCoord;
        int iterCoord;

        if (botRightX - topLeftX <= 3 || topLeftY - botRightY <= 3)
        {
            Debug.Log("[ERROR] Bad coords!");
            return;
        }

        if (horizontal == true)
        {
            do
            {
                fixdCoord = Random.Range(botRightY + 1, topLeftY); //on Y axis
            } while (fixdCoord % 2 == 0) ;

            do
            {
                doorCoord = Random.Range(topLeftX + 1, botRightX); //on X axis
            } while (doorCoord % 2 == 1);


            Debug.Log(string.Format("[DEBUG] Parameters {0} {1} {2} {3} {4} \n Fixed Y {5} Door X {6}", topLeftX, topLeftY, botRightX, botRightY, horizontal, fixdCoord, doorCoord));

            if (fixdCoord % 2 == 0)
            {
                Debug.Log(string.Format("[ERROR] Invalid fixed coord: {0}.\n{1} {2} {3} {4} {5} {6}", fixdCoord, topLeftX, topLeftY, botRightX, botRightY, horizontal, doorCoord));
            }

            for (iterCoord = topLeftX + 1; iterCoord < botRightX; iterCoord++)
            {
                if (iterCoord != doorCoord)
                {
                    _maze[fixdCoord, iterCoord] = 1;
                }
            }

            PartitionMaze(topLeftX, topLeftY, botRightX, fixdCoord, false);
            PartitionMaze(topLeftX, fixdCoord, botRightX, botRightY, false);
        }
        else
        {
            do
            {
                fixdCoord = Random.Range(topLeftX + 1, botRightX); //on X axis
            } while (fixdCoord % 2 == 0);

            do
            {
                doorCoord = Random.Range(botRightY + 1, topLeftY); //on Y axis
            } while (doorCoord % 2 == 1);

            Debug.Log(string.Format("[DEBUG] Parameters {0} {1} {2} {3} {4} \n Fixed X {5} Door Y {6}", topLeftX, topLeftY, botRightX, botRightY, horizontal, fixdCoord, doorCoord));

            if (fixdCoord % 2 == 0)
            {
                Debug.Log(string.Format("[ERROR] Invalid fixed coord: {0}.\n{1} {2} {3} {4} {5} {6}", fixdCoord, topLeftX, topLeftY, botRightX, botRightY, horizontal, doorCoord));
            }

            for (iterCoord = botRightY + 1; iterCoord < topLeftY; iterCoord++)
            {
                if (iterCoord != doorCoord)
                {
                    _maze[iterCoord, fixdCoord] = 1;
                }
            }

            PartitionMaze(topLeftX, topLeftY, fixdCoord, botRightY, true);
            PartitionMaze(fixdCoord, topLeftY, botRightX, botRightY, true);
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
        PartitionMaze(1, _mazeSize * 2 + 1, _mazeSize * 2 + 1, 1,true);

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
