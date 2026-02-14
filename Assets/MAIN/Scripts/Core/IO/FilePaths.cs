using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilePaths
{
    private const string HOME_DIRECTORY_SYMBOL = "~/";

    public static readonly string root = $"{Application.dataPath}/gameData/";

    //Runtime Paths
    public static readonly string gameSaves = $"{runtimePath}Save Files/";

    //Resources Font Paths
    public static readonly string resources_font = "Font/";

    //Resources Graphics Paths
    public static readonly string resources_graphics = "Graphics/";
    public static readonly string resources_backgroundImages = $"{resources_graphics}BG Images/";
    public static readonly string resources_backgroundVideos = $"{resources_graphics}BG Videos/";
    public static readonly string resources_backgroundCinematics = $"{resources_graphics}BG Cinematics/";

    //Resources Audio Paths
    public static readonly string resources_audio = "Audio/";
    public static readonly string resources_music = $"{resources_audio}Music/";
    public static readonly string resources_ambience = $"{resources_audio}Ambience/";
    public static readonly string resources_sfx = $"{resources_audio}SFX/";
    public static readonly string resources_ui = $"{resources_audio}UI/";
    public static readonly string resources_voices = $"{resources_audio}Voices/";

    //Resources Dialogue Paths
    public static readonly string resources_dialogueFiles = $"Dialogue Files/";

    //Resources Items Paths
    public static readonly string resources_itemsFiles = $"Items/";

    ///<summary>
    ///Returns the path to the resource using the default path or the root of the resources folder if
    ///</summary/>
    ///<param name="defaultPath"></param>
    ///<param name="resourceName"></param>
    ///<returns></returns>
    public static string GetPathToResource(string defaultPath, string graphicName)
    {
        if(graphicName.StartsWith(HOME_DIRECTORY_SYMBOL))
            return graphicName.Substring(HOME_DIRECTORY_SYMBOL.Length);

        return defaultPath + graphicName;
    }

    public static string runtimePath
    {
        get
        {
            #if UNITY_EDITOR
                return "Assets/appdata/";
            #else
                return Application.persistentDataPath + "/appdata/";
            #endif
        }
    }    
}