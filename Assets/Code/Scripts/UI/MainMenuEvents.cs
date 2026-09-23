using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class MainMenuEvents : MonoBehaviour
{
    private UIDocument _uiDocument;
    private Button _startButton;
    private Button _exitButton;
    private List<Button> _buttons = new List<Button>();
    [SerializeField] AudioSource clickSound;
    
    private void Awake() 
    {
        _uiDocument = GetComponent<UIDocument>();
        _startButton = _uiDocument.rootVisualElement.Q<Button>("StartButton");
        _exitButton = _uiDocument.rootVisualElement.Q<Button>("ExitButton");

        _startButton.RegisterCallback<ClickEvent>(OnStartButtonClick);
        _exitButton.RegisterCallback<ClickEvent>(OnExitButtonClick);
        
        _buttons = _uiDocument.rootVisualElement.Query<Button>().ToList();
        foreach (Button button in _buttons)
        {
            button.RegisterCallback<ClickEvent>(OnButtonClick);
        }

    }

    private void OnDisable()
    {
        _startButton.UnregisterCallback<ClickEvent>(OnStartButtonClick);
        _exitButton.UnregisterCallback<ClickEvent>(OnExitButtonClick);

        foreach (Button button in _buttons)
        {
            button.UnregisterCallback<ClickEvent>(OnButtonClick);
        }
    }

    private void OnButtonClick(ClickEvent evt)
    {
        clickSound.Play();
    }

    private void OnStartButtonClick(ClickEvent evt)
    {
        Debug.Log("Start Button Clicked");
    }

    private void OnExitButtonClick(ClickEvent evt)
    {
        Debug.Log("Exit Button Clicked");
    }
}
