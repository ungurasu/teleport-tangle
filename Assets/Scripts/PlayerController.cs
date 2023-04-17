using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LocalLibrary;

public class PlayerController : MonoBehaviour
{
    bool _isInitialized = false;
    Point _playerCoords;
    GameObject _playerObject;
    GameObject _camera;
    GameObject _maskObject;
    SpriteRenderer _playerRenderer;
    SpriteRenderer _maskRenderer;
    [SerializeField] Sprite[] _spritePlayer = new Sprite[4];
    [SerializeField] Sprite _spriteMask;
    [SerializeField] MazeController _mazeController;
    Vector2 _swipeStartPos;
    Vector2 _swipeEndPos;
    Vector3 _targetPos;
    [SerializeField] float _playerSlideSpeed = 10f;
    float _targetDistance;
    Direction _targetDirection;
    PlayerStates _playerState;

    void createPerspective()
    {
        _playerObject = new GameObject("Player");
        _playerRenderer = _playerObject.AddComponent<SpriteRenderer>();
        _playerRenderer.sprite = _spritePlayer[(int)Direction.Top];
        _playerRenderer.transform.parent = _playerObject.transform;
        _playerObject.transform.position = new Vector3(_playerCoords.x, _playerCoords.y, -0.3f);

        _maskObject = new GameObject("Mask");
        _maskRenderer = _maskObject.AddComponent<SpriteRenderer>();
        _maskRenderer.sprite = _spriteMask;
        _maskRenderer.transform.parent = _maskObject.transform;
        _maskObject.transform.position = new Vector3(_playerCoords.x, _playerCoords.y, -0.4f);
    }

    void movePlayer(int xCoord, int yCoord, Direction direction)
    {
        _playerCoords.x = xCoord;
        _playerCoords.y = yCoord;
        _playerObject.transform.position = new Vector3(_playerCoords.x, _playerCoords.y, -0.3f);
        _maskObject.transform.position = new Vector3(_playerCoords.x + 0.5f, _playerCoords.y + 0.5f, -0.4f);
        _camera.transform.position = new Vector3(_playerCoords.x + 0.5f, _playerCoords.y + 0.5f, -10f);

        _playerRenderer.sprite = _spritePlayer[(int)direction];
    }

    void slidePlayer(Direction direction, float speed)
    {
        Vector2 moveVector;

        switch (direction)
        {
            case Direction.Top:
                moveVector = new Vector2(0f, 1f) * speed * Time.deltaTime;
                break;

            case Direction.Bottom:
                moveVector = new Vector2(0f, -1f) * speed * Time.deltaTime;
                break;

            case Direction.Left:
                moveVector = new Vector2(-1f, 0f) * speed * Time.deltaTime;
                break;

            case Direction.Right:
                moveVector = new Vector2(1f, 0f) * speed * Time.deltaTime;
                break;

            default:
                moveVector = new Vector2(0f, 0f) * speed * Time.deltaTime;
                break;
        }

        _playerObject.transform.position += new Vector3(moveVector.x, moveVector.y, 0);
        _maskObject.transform.position   += new Vector3(moveVector.x, moveVector.y, 0);
        _camera.transform.position       += new Vector3(moveVector.x, moveVector.y, 0);

        _playerRenderer.sprite = _spritePlayer[(int)direction];
    } 

    void attemptMovePlayer(Direction direction)
    {
        bool acceptedMove = false;

        switch (direction)
        {
            case Direction.Bottom:
                if (_mazeController.validateMove((_playerCoords.x + 1) * 2, (_playerCoords.y + 1) * 2 - 1))
                {
                    _targetPos = _playerObject.transform.position + new Vector3(0f, -1f, 0f);
                    acceptedMove = true;
                }
                break;

            case Direction.Top:
                if (_mazeController.validateMove((_playerCoords.x + 1) * 2, (_playerCoords.y + 1) * 2 + 1))
                {
                    _targetPos = _playerObject.transform.position + new Vector3(0f, 1f, 0f);
                    acceptedMove = true;
                }
                break;

            case Direction.Left:
                if (_mazeController.validateMove((_playerCoords.x + 1) * 2 - 1, (_playerCoords.y + 1) * 2))
                {
                    _targetPos = _playerObject.transform.position + new Vector3(-1f, 0f, 0f);
                    acceptedMove = true;
                }
                break;

            case Direction.Right:
                if (_mazeController.validateMove((_playerCoords.x + 1) * 2 + 1, (_playerCoords.y + 1) * 2))
                {
                    _targetPos = _playerObject.transform.position + new Vector3(1f, 0f, 0f);
                    acceptedMove = true;
                }
                break;

            default:
                break;
        }

        if (acceptedMove)
        {
            _targetDirection = direction;
            _targetDistance = Vector3.Distance(_targetPos, _playerObject.transform.position);
            _playerState = PlayerStates.Moving;
        }
    }

    Direction detectSwipe()
    {
        Vector2 swipeDirection;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            _swipeStartPos = Input.GetTouch(0).position;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            _swipeEndPos = Input.GetTouch(0).position;

            swipeDirection = _swipeEndPos - _swipeStartPos;

            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
            {
                if (swipeDirection.x > 0)
                {
                    Debug.Log("Right");
                    return Direction.Right;
                }
                else
                {
                    Debug.Log("Left");
                    return Direction.Left;
                }
            }
            else
            {
                if (swipeDirection.y > 0)
                {
                    Debug.Log("Top");
                    return Direction.Top;
                }
                else
                {
                    Debug.Log("Bottom");
                    return Direction.Bottom;
                }
            }
        }
        else
        {
            return Direction.None;
        }
    }

    #if UNITY_EDITOR_WIN
    Direction detectKeyPress()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            return Direction.Left;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            return Direction.Right;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            return Direction.Top;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            return Direction.Bottom;
        }
        else
        {
            return Direction.None;
        }
    }
    #endif

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        _playerState = PlayerStates.ListenInput;
        _playerCoords = new Point(0, 0);
        _camera = GameObject.Find("Main Camera");
        createPerspective();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isInitialized)
        {
            switch (_playerState)
            {
                case PlayerStates.ListenInput:
                    Direction swipe = detectSwipe();

                    if (swipe != Direction.None)
                    {
                        attemptMovePlayer(swipe);
                    }

#if UNITY_EDITOR_WIN
                    Direction keyMove = detectKeyPress();

                    if (keyMove != Direction.None)
                    {
                        attemptMovePlayer(keyMove);
                    }
#endif
                    break;


                case PlayerStates.Moving:
                    _targetDistance = Vector3.Distance(_targetPos, _playerObject.transform.position);
                    slidePlayer(_targetDirection, _playerSlideSpeed);
                    
                    if (Vector3.Distance(_targetPos, _playerObject.transform.position) >= _targetDistance)
                    {
                        movePlayer((int)_targetPos.x, (int)_targetPos.y, _targetDirection);
                        _playerState = PlayerStates.ListenInput;
                    }

                    break;


                default:
                    break;
            }
        }
        else
        {
            if (_mazeController._isInitialized) 
            {
                Direction tempDir;
                if (_mazeController._startPoint.x == 1)
                {
                    tempDir = Direction.Right;
                }
                else if (_mazeController._startPoint.x == _mazeController._mazeSize)
                {
                    tempDir = Direction.Left;
                }
                else if (_mazeController._startPoint.y == 1)
                {
                    tempDir = Direction.Top;
                }
                else
                {
                    tempDir = Direction.Bottom;
                }

                movePlayer(_mazeController._startPoint.x - 1, _mazeController._startPoint.y - 1, tempDir);
                _isInitialized = true;
            }
        }
    }
}
