using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LocalLibrary;

public class MazeController : MonoBehaviour
{
    public bool _isInitialized = false;
    [SerializeField] public int _mazeSize = 10;
    // Start is called before the first frame update
    public int[,] _maze = new int[2002 ,2002];
    GameObject[,] _spriteObjectArrMaze             = new GameObject[1001, 1001];
    SpriteRenderer[,] _spriteRendererArrMaze       = new SpriteRenderer[1001, 1001];
    GameObject[,,] _spriteObjectArrWall            = new GameObject[1001, 1001, 4];
    SpriteRenderer[,,] _spriteRendererArrWall      = new SpriteRenderer[1001, 1001, 4];
    GameObject[,] _spriteObjectMazeObject          = new GameObject[1001, 1001];
    SpriteRenderer[,] _spriteRendererMazeObject    = new SpriteRenderer[1001, 1001];
    BoxCollider2D[,] _colliderMazeObject           = new BoxCollider2D[1001, 1001];
    GameObject[,] _spriteObjectMazeCorner          = new GameObject[1001, 1001];
    SpriteRenderer[,] _spriteRendererMazeCorner    = new SpriteRenderer[1001, 1001];
    [SerializeField] Sprite _spriteMazeTile;
    [SerializeField] Sprite _spriteWallLeft;
    [SerializeField] Sprite _spriteWallCorner;
    [SerializeField] Sprite _spriteWallBottom;
    [SerializeField] Sprite _spriteWallRight;
    [SerializeField] Sprite _spriteWallTop;
    [SerializeField] Sprite _spriteStart;
    [SerializeField] Sprite _spriteEnd;
    [SerializeField] Sprite _spriteTangle;
    [SerializeField] Sprite _spriteEnemy;
    [SerializeField] public Sprite _spriteObstacle;
    [SerializeField] public Sprite _spriteObstacleHighlited;
    [SerializeField] Sprite _spriteUnknown;
    GameObject _objectMazeTilesContainer;
    GameObject _objectMazeWallsContainer;
    GameObject _objectMazeObjectsContainer;
    public Point _startPoint;
    public Point _endPoint;

    public int GetMazeContents(int xCoord, int yCoord)
    {
        if (yCoord >= _mazeSize || yCoord < 0 || xCoord >= _mazeSize || xCoord < 0)
        {
            return 4;
        }

        return _maze[(yCoord + 1) * 2, (xCoord + 1) * 2];
    }

    public void TeleportObstacle(int xCoord2, int yCoord2, int xCoord1, int yCoord1)
    {
        _maze[(yCoord1 + 1) * 2, (xCoord1 + 1) * 2] = 0;
        _maze[(yCoord2 + 1) * 2, (xCoord2 + 1) * 2] = (int) MazeObjects.Obstacle;
        _spriteObjectMazeObject[yCoord2 + 1, xCoord2 + 1] =
            Instantiate(_spriteObjectMazeObject[yCoord1 + 1, xCoord1 + 1]);
        Destroy(_spriteObjectMazeObject[yCoord1 + 1, xCoord1 + 1]);
        _spriteObjectMazeObject[yCoord2 + 1, xCoord2 + 1].name = $"ObstacleX{xCoord2 + 1}Y{yCoord2 + 1}";
        _spriteObjectMazeObject[yCoord2 + 1, xCoord2 + 1].transform.position =
            new Vector3((float)xCoord2, (float)yCoord2, -0.2f);
    }

    void drawWall(int xCoord, int yCoord, Direction direction)
    {
        string objectName;
        Sprite sprite;
        float xPos = xCoord - 1;
        float yPos = yCoord - 1;

        switch (direction)
        {
            case Direction.Bottom:
                objectName = string.Format("WallBottomX{0}Y{1}", xCoord, yCoord);
                sprite = _spriteWallBottom;
                break;

            case Direction.Left:
                objectName = string.Format("WallLeftX{0}Y{1}", xCoord, yCoord);
                sprite = _spriteWallLeft;
                break;

            case Direction.Top:
                objectName = string.Format("WallTopX{0}Y{1}", xCoord, yCoord);
                sprite = _spriteWallTop;
                break;

            case Direction.Right:
                objectName = string.Format("WallRightX{0}Y{1}", xCoord, yCoord);
                sprite = _spriteWallRight;
                break;
            
            default:
                objectName = "Broken";
                sprite = _spriteUnknown;
                break;
        }

        _spriteObjectArrWall[yCoord, xCoord, (int)direction]                     = new GameObject(objectName);
        _spriteRendererArrWall[yCoord, xCoord, (int)direction]                   = _spriteObjectArrWall[yCoord, xCoord, (int)direction].AddComponent<SpriteRenderer>();
        _spriteRendererArrWall[yCoord, xCoord, (int)direction].sprite            = sprite;
        _spriteRendererArrWall[yCoord, xCoord, (int)direction].transform.parent  = _spriteObjectArrWall[yCoord, xCoord, (int)direction].transform;
        _spriteObjectArrWall[yCoord, xCoord, (int)direction].transform.position  = new Vector3(xPos, yPos, -0.1f);

        _spriteObjectArrWall[yCoord, xCoord, (int)direction].transform.parent    = _objectMazeWallsContainer.transform;
    }

    void drawObject(int xCoord, int yCoord, MazeObjects mazeObject)
    {
        string objectName;
        Sprite sprite;
        float xPos = xCoord - 1;
        float yPos = yCoord - 1;

        switch (mazeObject)
        {
            case MazeObjects.Start:
                objectName = $"StartLabelX{xCoord}Y{yCoord}";
                sprite = _spriteStart;
                break;

            case MazeObjects.End:
                objectName = $"EndLabelX{xCoord}Y{yCoord}";
                sprite = _spriteEnd;
                break;
            
            case MazeObjects.Tangle:
                objectName = $"TangleX{xCoord}Y{yCoord}";
                sprite = _spriteTangle;
                break;
            
            case MazeObjects.Enemy:
                objectName = $"EnemyX{xCoord}Y{yCoord}";
                sprite = _spriteEnemy;
                break;
            
            case MazeObjects.Obstacle:
                objectName = $"ObstacleX{xCoord}Y{yCoord}";
                sprite = _spriteObstacle;
                break;

            default:
                objectName = "Broken";
                sprite = _spriteUnknown;
                break;
        }

        _spriteObjectMazeObject[yCoord, xCoord] = new GameObject(objectName);
        _spriteRendererMazeObject[yCoord, xCoord] = _spriteObjectMazeObject[yCoord, xCoord].AddComponent<SpriteRenderer>();
        _spriteRendererMazeObject[yCoord, xCoord].sprite = sprite;
        _spriteRendererMazeObject[yCoord, xCoord].transform.parent = _spriteObjectMazeObject[yCoord, xCoord].transform;

        if (objectName.Contains("Obstacle"))
        {
            _colliderMazeObject[yCoord, xCoord] = _spriteObjectMazeObject[yCoord, xCoord].AddComponent<BoxCollider2D>();
        }

        _spriteObjectMazeObject[yCoord, xCoord].transform.position = new Vector3(xPos, yPos, -0.2f);

        _spriteObjectMazeObject[yCoord, xCoord].transform.parent = _objectMazeObjectsContainer.transform;
    }

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

        xCoord = Random.Range(1, _mazeSize + 1);
        yCoord = Random.Range(1, _mazeSize + 1);
        visited[yCoord, xCoord] = true;
        wallsList[0] = new Point(xCoord * 2 - 1, yCoord * 2);
        wallsList[1] = new Point(xCoord * 2 + 1, yCoord * 2);
        wallsList[2] = new Point(xCoord * 2, yCoord * 2 - 1);
        wallsList[3] = new Point(xCoord * 2, yCoord * 2 + 1);
        wallsListLenght = 4;

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

            if (2 <= currentWall.x && currentWall.x <= _mazeSize * 2 && 2 <= currentWall.y && currentWall.y <= _mazeSize * 2 )
            {
                if (currentWall.x % 2 == 1) // vertical wall
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
                else if (currentWall.y % 2 == 1)// horizontal wall
                {
                    if (visited[(currentWall.y - 1) / 2, currentWall.x / 2] == false && visited[(currentWall.y + 1) / 2, currentWall.x / 2] == true) // bottom cell
                    {
                        visited[(currentWall.y - 1) / 2, currentWall.x / 2] = true;
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
    }

    void PlaceStartEnd()
    {
        Direction startDir;
        Direction endDir;

        startDir = (Direction) Random.Range(0, 5);
        do
        {
            endDir = (Direction)Random.Range(0, 5);
        } while (endDir == startDir);

        switch (startDir) 
        {
            case Direction.Bottom:
                _startPoint.x = Random.Range(1, _mazeSize + 1);
                _startPoint.y = 1;
                break;

            case Direction.Top:
                _startPoint.x = Random.Range(1, _mazeSize + 1);
                _startPoint.y = _mazeSize;
                break;

            case Direction.Left:
                _startPoint.x = 1;
                _startPoint.y = Random.Range(1, _mazeSize + 1);
                break;

            case Direction.Right:
                _startPoint.x = _mazeSize;
                _startPoint.y = Random.Range(1, _mazeSize + 1);
                break;

            default:
                _startPoint.x = _mazeSize / 2;
                _startPoint.y = 1;
                break;
        }

        switch (endDir)
        {
            case Direction.Bottom:
                _endPoint.y = 1;
                do
                {
                    _endPoint.x = Random.Range(1, _mazeSize + 1);
                } while (_startPoint - _endPoint < _mazeSize - 1);
                break;

            case Direction.Top:
                _endPoint.y = _mazeSize;
                do
                {
                    _endPoint.x = Random.Range(1, _mazeSize + 1);
                } while (_startPoint - _endPoint < _mazeSize - 1);
                break;

            case Direction.Left:
                _endPoint.x = 1;
                do
                {
                    _endPoint.y = Random.Range(1, _mazeSize + 1);
                } while (_startPoint - _endPoint < _mazeSize - 1);
                break;

            case Direction.Right:
                _endPoint.x = _mazeSize;
                do
                {
                    _endPoint.y = Random.Range(1, _mazeSize + 1);
                } while (_startPoint - _endPoint < _mazeSize - 1);
                break;

            default:
                _endPoint.x = _mazeSize / 2;
                _endPoint.y = _mazeSize;
                break;
        }

        _maze[_startPoint.y * 2, _startPoint.x * 2] = (int)MazeObjects.Start;
        _maze[_endPoint.y * 2, _endPoint.x * 2]     = (int)MazeObjects.End;
    }

    void PlaceEnemiesObstacles()
    {
        int xCoord;
        int yCoord;
        int noEnemies = _mazeSize * _mazeSize / 100;
        int noObstacles = _mazeSize * _mazeSize / 100;
        
        for (int i = 1; i <= noEnemies; i++)
        {
            do
            {
                xCoord = Random.Range(0, _mazeSize - 1);
                yCoord = Random.Range(0, _mazeSize - 1);
            } while (_maze[(yCoord + 1) * 2, (xCoord + 1) * 2] != (int) MazeObjects.Floor);

            _maze[(yCoord + 1) * 2, (xCoord + 1) * 2] = (int)MazeObjects.Enemy;
        }
        
        for (int i = 1; i <= noObstacles; i++)
        {
            do
            {
                xCoord = Random.Range(0, _mazeSize - 1);
                yCoord = Random.Range(0, _mazeSize - 1);
            } while (_maze[(yCoord + 1) * 2, (xCoord + 1) * 2] != (int) MazeObjects.Floor);

            _maze[(yCoord + 1) * 2, (xCoord + 1) * 2] = (int)MazeObjects.Obstacle;
        }
    }

    public bool PlaceTangle(int xCoord, int yCoord)
    {
        if (0 <= xCoord && xCoord < _mazeSize && 0 <= yCoord && yCoord < _mazeSize && _maze[(yCoord + 1) * 2, (xCoord + 1) * 2] == (int) MazeObjects.Floor)
        {
            _maze[(yCoord + 1) * 2, (xCoord + 1) * 2] = (int)MazeObjects.Tangle;
            drawObject(xCoord + 1, yCoord + 1, MazeObjects.Tangle);
            return true;
        }
        
        return false;
    }

    public bool validateMove(int xCoord, int yCoord)
    {
        if (_maze[yCoord, xCoord] == (int)MazeObjects.Floor || _maze[yCoord, xCoord] == (int)MazeObjects.Start || _maze[yCoord, xCoord] == (int)MazeObjects.End)
        {
            return true;
        }
        else
        {
            return false;
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
        _objectMazeObjectsContainer = new GameObject("MazeObjectsContainer");

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

        PlaceStartEnd();
        
        PlaceEnemiesObstacles();

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

                _spriteObjectMazeCorner[yCoord, xCoord]                    = new GameObject(string.Format("MazeCornerX{0}Y{1}", xCoord, yCoord));
                _spriteRendererMazeCorner[yCoord, xCoord]                  = _spriteObjectMazeCorner[yCoord, xCoord].AddComponent<SpriteRenderer>();
                _spriteRendererMazeCorner[yCoord, xCoord].sprite           = _spriteWallCorner;
                _spriteRendererMazeCorner[yCoord, xCoord].transform.parent = _spriteObjectMazeCorner[yCoord, xCoord].transform;
                _spriteObjectMazeCorner[yCoord, xCoord].transform.position = new Vector3(xPos, yPos, -0.005f);

                _spriteObjectArrMaze[yCoord, xCoord].transform.parent    = _objectMazeTilesContainer.transform;
                _spriteObjectMazeCorner[yCoord, xCoord].transform.parent = _objectMazeTilesContainer.transform;

                if (_maze[yCoord * 2 - 1, xCoord * 2] == 1) 
                {
                    drawWall(xCoord, yCoord, Direction.Bottom);
                }
                if (_maze[yCoord * 2 + 1, xCoord * 2] == 1)
                {
                    drawWall(xCoord, yCoord, Direction.Top);
                }
                if (_maze[yCoord * 2, xCoord * 2 - 1] == 1)
                {
                    drawWall(xCoord, yCoord, Direction.Left);
                }
                if (_maze[yCoord * 2, xCoord * 2 + 1] == 1)
                {
                    drawWall(xCoord, yCoord, Direction.Right);
                }

                if (_maze[yCoord * 2, xCoord * 2] > 1)
                {
                    drawObject(xCoord, yCoord, (MazeObjects) _maze[yCoord * 2, xCoord * 2]);
                }
            }
        }

        _isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
