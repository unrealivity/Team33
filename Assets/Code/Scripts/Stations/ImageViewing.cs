using UnityEngine;

public class ImageViewing : Station
{
    [SerializeField] private GameObject content;
    public override void Interact()
    {
        content.SetActive(!content.activeSelf);
        //TODO pause game
    }
}
