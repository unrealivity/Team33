using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScreenUI : MonoBehaviour
{
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;
    
    private void Awake()
    {
        playAgainButton.onClick.AddListener(OnPlayAgainPressed);
        mainMenuButton.onClick.AddListener(mainMenuPressed);
    }

    private void OnDestroy()
    {
        playAgainButton.onClick.RemoveListener(OnPlayAgainPressed);
        mainMenuButton.onClick.RemoveListener(mainMenuPressed);
    }

    private void OnPlayAgainPressed()
    {
        SceneManager.LoadScene("Main");
    }

    private void mainMenuPressed()
    {   
        SceneManager.LoadScene("MainMenu");
    }

}
