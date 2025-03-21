using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameSave : MonoBehaviour
{
    public VNGameSave save;
    void Start()
    {
        VNGameSave.activeFile = new VNGameSave();
    }

    public void Save()
    {
        VNGameSave.activeFile.Save();
    }

    public void Load()
    {
       try
        {
            save = VNGameSave.Load($"{FilePaths.gameSaves}{VNGameSave.TEMP_NAME}{VNGameSave.FILE_TYPE}", activateOnLoad: true);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Do something because we found an error. {e.ToString()}");
        }
    }
}
