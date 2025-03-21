using UnityEngine;

[System.Serializable]
public class Items
{
    public string nameFolder;
    public string nameItem;
    public Sprite sprite;

    public Items(string nameFolder, string nameItem, Sprite sprite)
    {
        this.nameFolder = nameFolder;
        this.nameItem = nameItem;
        this.sprite = sprite;
    }
}