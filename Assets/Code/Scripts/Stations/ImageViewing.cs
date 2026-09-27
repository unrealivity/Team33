using UnityEngine;

public class ImageViewing : Station
{
    [SerializeField] private GameObject content;
    private bool _isShowingContent;
    
    private void Awake()
    {
        PauseManager.PauseStateChanged += OnPauseStateChanged;
    }

    private void OnDestroy()
    {
        PauseManager.PauseStateChanged -= OnPauseStateChanged;
    }
    
    public override void Interact()
    {
        if (_isShowingContent)
        {
            _isShowingContent = false;
            content.SetActive(false);
            PauseManager.SetPaused(false);
        }
        else
        {
            _isShowingContent = true;
            content.SetActive(true);
            PauseManager.SetPaused(true, this);
        }
    }
    
    private void OnPauseStateChanged(bool isPaused)
    {
        if (isPaused) return;
        if (!_isShowingContent) return;

        _isShowingContent = false;
        content.SetActive(false);
    }
}
