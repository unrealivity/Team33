using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;
    
    private void Awake()
    {
        PauseManager.PauseStateChanged += OnPauseStateChanged;
        pauseMenu.SetActive(false);
        continueButton.onClick.AddListener(OnResumePressed);
        exitButton.onClick.AddListener(OnExitPressed);
    }

    private void OnDestroy()
    {
        PauseManager.PauseStateChanged -= OnPauseStateChanged;
        pauseMenu.SetActive(false);
        continueButton.onClick.RemoveListener(OnResumePressed);
        exitButton.onClick.RemoveListener(OnExitPressed);
    }

    private void OnResumePressed()
    {
        PauseManager.TogglePause();
        pauseMenu.SetActive(false);
    }

    private void OnExitPressed()
    {   
        SceneManager.LoadScene("MainMenu");
    }
    
    private void OnPauseStateChanged(bool isPaused)
    {
        pauseMenu.SetActive(isPaused && PauseManager.PauseOwner == null);
    }
}
