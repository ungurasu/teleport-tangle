using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LocalLibrary;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private MenuStates _currentState = MenuStates.ListenInput;
    private SpriteRenderer _spriteRendererBanner;
    private SpriteRenderer _spriteRendererStartButton;
    private SpriteRenderer _spriteRendererQuitButton;
    
    // Start is called before the first frame update
    void Start()
    {
        _spriteRendererBanner = GameObject.Find("Banner").GetComponent<SpriteRenderer>();
        _spriteRendererStartButton = GameObject.Find("StartButton").GetComponent<SpriteRenderer>();
        _spriteRendererQuitButton = GameObject.Find("QuitButton").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
        {
            case MenuStates.ListenInput:
                handleGuiClick();
                handleGuiTouch();
                break;
            
            case MenuStates.FadeOut:
                _spriteRendererBanner.color -= new Color(0, 0, 0, 0.01f);
                _spriteRendererStartButton.color -= new Color(0, 0, 0, 0.01f);
                _spriteRendererQuitButton.color -= new Color(0, 0, 0, 0.01f);

                if (_spriteRendererBanner.color.a <= 0)
                {
                    SceneManager.LoadScene("Scenes/MainScene");
                }

                break;
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
                if (raycastHit.collider.name == "StartButton")
                {
                    _currentState = MenuStates.FadeOut;
                    GameObject.Find("StartButton").GetComponent<BoxCollider2D>().enabled = false;
                    GameObject.Find("QuitButton").GetComponent<BoxCollider2D>().enabled = false;
                }
                else if (raycastHit.collider.name == "QuitButton")
                {
                    Debug.Log("Quit");
                    Application.Quit();
                }
            }
        }
    }

    void handleGuiTouch()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Vector2 raycast = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            RaycastHit2D raycastHit = Physics2D.Raycast(raycast, Vector2.zero);
            if (raycastHit)
            {
                if (raycastHit.collider.name == "StartButton")
                {
                    _currentState = MenuStates.FadeOut;
                    GameObject.Find("StartButton").GetComponent<BoxCollider2D>().enabled = false;
                    GameObject.Find("QuitButton").GetComponent<BoxCollider2D>().enabled = false;
                }
                else if (raycastHit.collider.name == "QuitButton")
                {
                    Debug.Log("Quit");
                    Application.Quit();
                }
            }
        }
    }
}
