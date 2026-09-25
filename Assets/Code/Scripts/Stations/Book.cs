using UnityEngine;

public class Book : Station
{
    [SerializeField] private GameObject bookContentPage;
    public override void Interact()
    {
        bookContentPage.SetActive(!bookContentPage.activeSelf);
        //TODO pause game
    }
}
