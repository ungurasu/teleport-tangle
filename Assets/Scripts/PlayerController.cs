using UnityEngine;
using LocalLibrary;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Font _fontScore;
    bool _isInitialized = false;
    Canvas _canvas;
    Point _playerCoords;
    Point _previousCoords;
    GameObject _playerObject;
    GameObject _camera;
    GameObject _maskObject;
    GameObject _arrowBottomObject;
    GameObject _arrowLeftObject;
    GameObject _arrowTopObject;
    GameObject _arrowRightObject;
    GameObject _exitButton;
    GameObject[] _arrowObjects = new GameObject[4];
    [SerializeField] GameObject _scoreTextObject;
    BoxCollider2D[] _arrowColliders = new BoxCollider2D[4];
    SpriteRenderer _playerRenderer;
    SpriteRenderer _maskRenderer;
    SpriteRenderer _arrowBottomRenderer;
    SpriteRenderer _arrowLeftRenderer;
    SpriteRenderer _arrowTopRenderer;
    SpriteRenderer _arrowRightRenderer;
    SpriteRenderer _exitButtonRenderer;
    SpriteRenderer[] _arrowRenderers = new SpriteRenderer[4];
    [SerializeField] Sprite[] _spritePlayer = new Sprite[4];
    [SerializeField] Sprite _spriteMask;
    [SerializeField] Sprite[] _spriteArrow = new Sprite[4];
    [SerializeField] Sprite _spriteExitButton;
    [SerializeField] MazeController _mazeController;
    TextMeshProUGUI _scoreTextMeshPro;
    Vector2 _swipeStartPos;
    Vector2 _swipeEndPos;
    Vector2 _swipeDelta;
    Vector3 _targetPos;
    [SerializeField] float _playerSlideSpeed = 10f;
    float _targetDistance;
    Direction _targetDirection;
    PlayerStates _playerState;
    private GameObject _selectedObstacle;
    private int _score = 0;
    
    private Vector3[] _arrowOffsets =
    {
        new Vector3(0, -3f, 0),
        new Vector3(-3f, 0, 0),
        new Vector3(0, 3f, 0),
        new Vector3(3f, 0 , 0)
    };
    

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

        _exitButton = new GameObject("ExitButton");
        _exitButtonRenderer = _exitButton.AddComponent<SpriteRenderer>();
        _exitButtonRenderer.sprite = _spriteExitButton;
        _exitButtonRenderer.transform.parent = _exitButton.transform;
        _exitButton.AddComponent<BoxCollider2D>();
        _exitButton.transform.position = new Vector3(_playerCoords.x, _playerCoords.y, -0.5f) + new Vector3(7f, 3f, 0);


        for (int i = 0; i <= 3; i++)
        {
            _arrowObjects[i] = new GameObject($"Arrow{(Direction)i}");
            _arrowRenderers[i] = _arrowObjects[i].AddComponent<SpriteRenderer>();
            _arrowRenderers[i].sprite = _spriteArrow[i];
            _arrowObjects[i].transform.position = new Vector3(_playerCoords.x, _playerCoords.y, -0.5f) + _arrowOffsets[i];
            _arrowColliders[i] = _arrowObjects[i].AddComponent<BoxCollider2D>();
        }

        _scoreTextMeshPro = _scoreTextObject.GetComponent<TextMeshProUGUI>();
        _scoreTextMeshPro.text = $"Moves: {_score}";
    }

    void incrementScore(int increment)
    {
        _score += increment;
        _scoreTextMeshPro.text = $"Moves: {_score}";
    }

    void updateArrows()
    {

        for (int i = 0; i <= 3; i++)
        {
            float z_coord = -0.5f;
            
            switch ((Direction)i)
            {
                case Direction.Bottom:
                    if (_playerCoords.y < 2 || !_mazeController.validateMove((_playerCoords.x + 1) * 2, (_playerCoords.y + 1) * 2 - 4))
                    {
                        z_coord = 1;
                        _arrowColliders[i].enabled = false;
                    }
                    else
                    {
                        z_coord = -0.5f;
                        _arrowColliders[i].enabled = true;
                    }

                    break;
                    
                case Direction.Left:
                    if (_playerCoords.x < 2 || !_mazeController.validateMove((_playerCoords.x + 1) * 2 - 4, (_playerCoords.y + 1) * 2))
                    {
                        z_coord = 1;
                        _arrowColliders[i].enabled = false;
                    }
                    else
                    {
                        z_coord = -0.5f;
                        _arrowColliders[i].enabled = true;
                    }

                    break;
                    
                case Direction.Top:
                    if (_playerCoords.y > _mazeController._mazeSize - 3 || !_mazeController.validateMove((_playerCoords.x + 1) * 2, (_playerCoords.y + 1) * 2 + 4))
                    {
                        z_coord = 1;
                        _arrowColliders[i].enabled = false;
                    }
                    else
                    {
                        z_coord = -0.5f;
                        _arrowColliders[i].enabled = true;
                    }

                    break;
                    
                case Direction.Right:
                    if (_playerCoords.x > _mazeController._mazeSize - 3 || !_mazeController.validateMove((_playerCoords.x + 1) * 2 + 4, (_playerCoords.y + 1) * 2))
                    {
                        z_coord = 1;
                        _arrowColliders[i].enabled = false;
                    }
                    else
                    {
                        z_coord = -0.5f;
                        _arrowColliders[i].enabled = true;
                    }

                    break;
            }

            _arrowObjects[i].transform.position = new Vector3(_playerCoords.x, _playerCoords.y, z_coord) + _arrowOffsets[i];
        }
    }

    void placePlayerAt(int xCoord, int yCoord, Direction direction)
    {
        _playerCoords.x = xCoord;
        _playerCoords.y = yCoord;
        _playerObject.transform.position = new Vector3(_playerCoords.x, _playerCoords.y, -0.3f);
        _maskObject.transform.position = new Vector3(_playerCoords.x + 0.5f, _playerCoords.y + 0.5f, -0.4f);
        _camera.transform.position = new Vector3(_playerCoords.x + 0.5f, _playerCoords.y + 0.5f, -10f);
        _exitButton.transform.position = new Vector3(_playerCoords.x + 3f, _playerCoords.y + 7f, -0.5f);
        
        updateArrows();
        
        _playerRenderer.sprite = _spritePlayer[(int)direction];
    }

    void slidePlayer(Direction direction, float speed)
    {
        Vector2 moveVector;

        switch (direction)
        {
            case Direction.Top:
                moveVector = new Vector2(0f, 1f) * (speed * Time.deltaTime);
                break;

            case Direction.Bottom:
                moveVector = new Vector2(0f, -1f) * (speed * Time.deltaTime);
                break;

            case Direction.Left:
                moveVector = new Vector2(-1f, 0f) * (speed * Time.deltaTime);
                break;

            case Direction.Right:
                moveVector = new Vector2(1f, 0f) * (speed * Time.deltaTime);
                break;

            default:
                moveVector = new Vector2(0f, 0f) * (speed * Time.deltaTime);
                break;
        }

        _playerObject.transform.position      += new Vector3(moveVector.x, moveVector.y, 0);
        _maskObject.transform.position        += new Vector3(moveVector.x, moveVector.y, 0);
        _camera.transform.position            += new Vector3(moveVector.x, moveVector.y, 0);
        _exitButton.transform.position        += new Vector3(moveVector.x, moveVector.y, 0);

        for (int i = 0; i <= 3; i++)
        {
            _arrowObjects[i].transform.position += new Vector3(moveVector.x, moveVector.y, 0);
        }

        _playerRenderer.sprite = _spritePlayer[(int)direction];
    } 

    void attemptMovePlayer(Direction direction)
    {
        bool acceptedMove = false;

        switch (direction)
        {
            case Direction.Bottom:
                if (_mazeController.validateMove((_playerCoords.x + 1) * 2, (_playerCoords.y + 1) * 2 - 1) &&
                    _mazeController.validateMove((_playerCoords.x + 1) * 2, (_playerCoords.y + 1) * 2 - 2)
                    )
                {
                    _targetPos = _playerObject.transform.position + new Vector3(0f, -1f, 0f);
                    acceptedMove = true;
                }
                break;

            case Direction.Top:
                if (_mazeController.validateMove((_playerCoords.x + 1) * 2, (_playerCoords.y + 1) * 2 + 1) &&
                    _mazeController.validateMove((_playerCoords.x + 1) * 2, (_playerCoords.y + 1) * 2 + 2)
                    )
                {
                    _targetPos = _playerObject.transform.position + new Vector3(0f, 1f, 0f);
                    acceptedMove = true;
                }
                break;

            case Direction.Left:
                if (_mazeController.validateMove((_playerCoords.x + 1) * 2 - 1, (_playerCoords.y + 1) * 2) &&
                    _mazeController.validateMove((_playerCoords.x + 1) * 2 - 2, (_playerCoords.y + 1) * 2)
                    )
                {
                    _targetPos = _playerObject.transform.position + new Vector3(-1f, 0f, 0f);
                    acceptedMove = true;
                }
                break;

            case Direction.Right:
                if (_mazeController.validateMove((_playerCoords.x + 1) * 2 + 1, (_playerCoords.y + 1) * 2) &&
                    _mazeController.validateMove((_playerCoords.x + 1) * 2 + 2, (_playerCoords.y + 1) * 2))
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

    void teleportPlayer(int x, int y, Direction direction)
    {
        if (_mazeController.validateMove((x + 1) * 2, (y + 1) * 2))
        {
            _previousCoords = _playerCoords;
            _targetPos = new Vector3( x, y, _playerObject.transform.position.z);
            _targetDirection = direction;
            _playerState = PlayerStates.FadeOut;
        }
    }

    Direction detectSwipe()
    {
        Vector2 swipeDirection;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            _swipeStartPos = Input.GetTouch(0).position;
            _swipeDelta = Vector2.zero;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            _swipeDelta += Input.GetTouch(0).deltaPosition;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            if (_swipeDelta == Vector2.zero)
            {
                return Direction.None;
            }

            _swipeEndPos = Input.GetTouch(0).position;

            swipeDirection = _swipeEndPos - _swipeStartPos;

            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
            {
                if (swipeDirection.x > 0)
                {
                    return Direction.Right;
                }
                else
                {
                    return Direction.Left;
                }
            }
            else
            {
                if (swipeDirection.y > 0)
                {
                    return Direction.Top;
                }
                else
                {
                    return Direction.Bottom;
                }
            }
        }
        else
        {
            return Direction.None;
        }
    }

    void handleGuiTouch()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && _swipeDelta == Vector2.zero)
        {
            Vector2 raycast = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            RaycastHit2D raycastHit = Physics2D.Raycast(raycast, Vector2.zero);
            if (raycastHit)
            {
                if (raycastHit.collider.name == "ArrowTop")
                {
                    teleportPlayer(_playerCoords.x, _playerCoords.y + 2, Direction.Top);
                }
                else if (raycastHit.collider.name == "ArrowLeft")
                {
                    teleportPlayer(_playerCoords.x - 2, _playerCoords.y, Direction.Left);
                }
                else if (raycastHit.collider.name == "ArrowBottom")
                {
                    teleportPlayer(_playerCoords.x, _playerCoords.y - 2, Direction.Bottom);
                }
                else if (raycastHit.collider.name == "ArrowRight")
                {
                    teleportPlayer(_playerCoords.x + 2, _playerCoords.y, Direction.Right);
                }
                else if (raycastHit.collider.name.Contains("Obstacle"))
                {
                    _selectedObstacle = GameObject.Find(raycastHit.collider.name);
                    SpriteRenderer spriteRenderer = _selectedObstacle.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = _mazeController._spriteObstacleHighlited;
                    _playerState = PlayerStates.ListenObstacleTeleport;
                }
                else if (raycastHit.collider.name == "ExitButton")
                {
                    SceneManager.LoadScene("Scenes/MenuScene");
                }
            }
        }
    }
    
    void handleObstacleTouch()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && _swipeDelta == Vector2.zero)
        {
            Vector2 raycast = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            RaycastHit2D raycastHit = Physics2D.Raycast(raycast, Vector2.zero);
            
            if (raycastHit)
            {
                if (raycastHit.collider.name == _selectedObstacle.name)
                {
                    SpriteRenderer spriteRenderer = _selectedObstacle.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = _mazeController._spriteObstacle;
                    _playerState = PlayerStates.ListenInput;
                }
                else if (raycastHit.collider.name == "ExitButton")
                {
                    SceneManager.LoadScene("Scenes/MenuScene");
                }
            }
            else
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = Camera.main.nearClipPlane;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                Point worldPosFloored = new Point(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y));

                if (0 < worldPosFloored - _playerCoords
                    && worldPosFloored - _playerCoords < 3 
                    && _mazeController.GetMazeContents(worldPosFloored.x, worldPosFloored.y) == (int) MazeObjects.Floor)
                {
                    SpriteRenderer spriteRenderer = _selectedObstacle.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = _mazeController._spriteObstacle;
                    
                    _mazeController.TeleportObstacle(
                        worldPosFloored.x,
                        worldPosFloored.y,
                        (int) _selectedObstacle.transform.position.x,
                        (int) _selectedObstacle.transform.position.y
                    );
                    
                    _playerState = PlayerStates.ListenInput;
                    Destroy(_selectedObstacle);
                }
            }
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

    void handleGuiClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 raycast = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D raycastHit = Physics2D.Raycast(raycast, Vector2.zero);
            if (raycastHit)
            {
                if (raycastHit.collider.name == "ArrowTop")
                {
                    teleportPlayer(_playerCoords.x, _playerCoords.y + 2, Direction.Top);
                }
                else if (raycastHit.collider.name == "ArrowLeft")
                {
                    teleportPlayer(_playerCoords.x - 2, _playerCoords.y, Direction.Left);
                }
                else if (raycastHit.collider.name == "ArrowBottom")
                {
                    teleportPlayer(_playerCoords.x, _playerCoords.y - 2, Direction.Bottom);
                }
                else if (raycastHit.collider.name == "ArrowRight")
                {
                    teleportPlayer(_playerCoords.x + 2, _playerCoords.y, Direction.Right);
                }
                else if (raycastHit.collider.name.Contains("Obstacle"))
                {
                    _selectedObstacle = GameObject.Find(raycastHit.collider.name);
                    SpriteRenderer spriteRenderer = _selectedObstacle.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = _mazeController._spriteObstacleHighlited;
                    _playerState = PlayerStates.ListenObstacleTeleport;
                }
                else if (raycastHit.collider.name == "ExitButton")
                {
                    SceneManager.LoadScene("Scenes/MenuScene");
                }
            }
        }
    }
    
    void handleObstacleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 raycast = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D raycastHit = Physics2D.Raycast(raycast, Vector2.zero);
            
            if (raycastHit)
            {
                if (raycastHit.collider.name == _selectedObstacle.name)
                {
                    SpriteRenderer spriteRenderer = _selectedObstacle.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = _mazeController._spriteObstacle;
                    _playerState = PlayerStates.ListenInput;
                }
                else if (raycastHit.collider.name == "ExitButton")
                {
                    SceneManager.LoadScene("Scenes/MenuScene");
                }
            }
            else
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = Camera.main.nearClipPlane;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                Point worldPosFloored = new Point(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y));

                if (0 < worldPosFloored - _playerCoords
                    && worldPosFloored - _playerCoords < 3 
                    && _mazeController.GetMazeContents(worldPosFloored.x, worldPosFloored.y) == (int) MazeObjects.Floor)
                {
                    SpriteRenderer spriteRenderer = _selectedObstacle.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = _mazeController._spriteObstacle;
                    
                    _mazeController.TeleportObstacle(
                        worldPosFloored.x,
                        worldPosFloored.y,
                        (int) _selectedObstacle.transform.position.x,
                        (int) _selectedObstacle.transform.position.y
                        );
                    
                    _playerState = PlayerStates.ListenInput;
                    Destroy(_selectedObstacle);
                }
            }
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
                    #if !UNITY_EDITOR_WIN
                    Direction swipe = detectSwipe();

                    if (swipe != Direction.None)
                    {
                        attemptMovePlayer(swipe);
                    }
                    else
                    {
                        handleGuiTouch();
                    }
                    #endif

                    #if UNITY_EDITOR_WIN
                    Direction keyMove = detectKeyPress();

                    if (keyMove != Direction.None)
                    {
                        attemptMovePlayer(keyMove);
                    }
                    
                    handleGuiClick();
                    #endif
                    
                    break;


                case PlayerStates.Moving:
                    _targetDistance = Vector3.Distance(_targetPos, _playerObject.transform.position);
                    slidePlayer(_targetDirection, _playerSlideSpeed);
                    
                    if (Vector3.Distance(_targetPos, _playerObject.transform.position) >= _targetDistance)
                    {
                        placePlayerAt((int)_targetPos.x, (int)_targetPos.y, _targetDirection);

                        if (_playerRenderer.color.a > 0)
                        {
                            _playerState = PlayerStates.ListenInput;
                            incrementScore(1);
                        }
                        else
                        {
                            _playerState = PlayerStates.FadeIn;
                        }

                    }

                    break;
                
                
                case PlayerStates.ListenObstacleTeleport:
                    
                    #if !UNITY_EDITOR_WIN
                    handleObstacleTouch();
                    #endif
                    
                    #if UNITY_EDITOR_WIN
                    handleObstacleClick();
                    #endif
                    
                    break;
                
                case PlayerStates.FadeOut:
                    _playerRenderer.color -= new Color(
                        0,
                        0,
                        0,
                        Time.deltaTime > _playerRenderer.color.a ?
                            _playerRenderer.color.a : Time.deltaTime
                    );

                    if (_playerRenderer.color.a <= 0)
                    {
                        _playerState = PlayerStates.Moving;
                    }

                    break;
                
                case PlayerStates.FadeIn:
                    _playerRenderer.color += new Color(
                        0,
                        0,
                        0,
                        Time.deltaTime + _playerRenderer.color.a > 1 ?
                            1 - _playerRenderer.color.a : Time.deltaTime
                    );

                    if (_playerRenderer.color.a >= 1)
                    {
                        _playerState = PlayerStates.ListenInput;
                        
                        _mazeController.PlaceTangle(_previousCoords.x - 1, _previousCoords.y - 1);
                        _mazeController.PlaceTangle(_previousCoords.x, _previousCoords.y - 1);
                        _mazeController.PlaceTangle(_previousCoords.x + 1, _previousCoords.y - 1);
                        _mazeController.PlaceTangle(_previousCoords.x - 1, _previousCoords.y);
                        _mazeController.PlaceTangle(_previousCoords.x, _previousCoords.y);
                        _mazeController.PlaceTangle(_previousCoords.x + 1, _previousCoords.y);
                        _mazeController.PlaceTangle(_previousCoords.x - 1, _previousCoords.y + 1);
                        _mazeController.PlaceTangle(_previousCoords.x, _previousCoords.y + 1);
                        _mazeController.PlaceTangle(_previousCoords.x + 1, _previousCoords.y + 1);
                        
                        incrementScore(3);
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

                placePlayerAt(_mazeController._startPoint.x - 1, _mazeController._startPoint.y - 1, tempDir);
                _isInitialized = true;
            }
        }
    }
}
