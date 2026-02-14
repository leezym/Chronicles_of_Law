using System;
using System.Collections;
using GAME;
using UnityEngine;
using UnityEngine.Video;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_GraphicPanels : CMD_DatabaseExtension
    {
        private static string PARAM_PANEL = "-panel";
        private static string PARAM_LAYER = "-capa";
        private static string PARAM_MEDIA = "-media";
        private static string PARAM_SPEED = "-vel";
        private static string PARAM_USEVIDEOAUDIO = "-audio";
        //private static string PARAM_GENDER = "-genero";
        //private static string PARAM_LEVEL = "-nivel";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("configcapasmultimedia", new Func<string[], IEnumerator>(SetLayerMedia));
            database.AddCommand("borrarcapasmultimedia", new Func<string[], IEnumerator>(ClearLayerMedia));
            database.AddCommand("mostrarcinematica", new Func<string[], IEnumerator>(SetCinematic));
        }

        private static IEnumerator SetLayerMedia(string[] data)
        {
            //Parameters available to function
            PanelType panelName;
            int layer = 0;
            string mediaName = "";
            float transitionSpeed = 0;
            bool useAudio = false;
            string pathToGraphic = "";
            UnityEngine.Object graphic = null;

            //Now get the parameters
            var parameters = ConvertDataToParameters(data);

            //Try to get the panel that this media is applied to
            parameters.TryGetValue(PARAM_PANEL, out panelName);
            GraphicPanel panel = GraphicPanelManager.Instance.GetPanel(panelName);
            if(panel == null)
            {
                Debug.LogError($"Unable to grab panel '{panelName}' because it is not a valid panel. Please check the panel name and adjust the command.");
                yield break;
            }

            //Try to get the layer to apply this graphic to
            parameters.TryGetValue(PARAM_LAYER, out layer, defaultValue: 0);

            //Try to get the graphic
            parameters.TryGetValue(PARAM_MEDIA, out mediaName);

            //Try to get the speed of the transition if it is not an inmmediate effect
            parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultValue: 1);

            //If this is a video, try to get whether we use audio from the video or not
            parameters.TryGetValue(PARAM_USEVIDEOAUDIO, out useAudio, defaultValue: false);

            //Now run the logic
            pathToGraphic = FilePaths.GetPathToResource(FilePaths.resources_backgroundImages, mediaName);
            graphic = Resources.Load<Texture>(pathToGraphic);

            if(graphic == null)
            {
                pathToGraphic = FilePaths.GetPathToResource(FilePaths.resources_backgroundVideos, mediaName);
                graphic = Resources.Load<VideoClip>(pathToGraphic);
            }

            if(graphic == null)
            {
                Debug.LogError($"Could not find media file called '{mediaName}' in the Resources directories. Please specify the field path within resources and make sure it exists!");
                yield break;                
            }

            //Lets try to get the layer to apply the media to
            GraphicLayer graphicLayer  = panel.GetLayer(layer, createIfDoesNotExist: true);

            if(graphic is Texture)
            {
                yield return graphicLayer.SetTexture(graphic as Texture, transitionSpeed, pathToGraphic);
            }
            else 
            {
                yield return graphicLayer.SetVideo(graphic as VideoClip, transitionSpeed, useAudio, true, pathToGraphic, AudioBus.Ambience);
            }
        }

        private static IEnumerator ClearLayerMedia(string[] data)
        {
            //Parameters available to function
            PanelType panelName;
            int layer = 0;
            float transitionSpeed = 0;

            //Now get the parameters
            var parameters = ConvertDataToParameters(data);

            //Try to get the panel that this media is applied to
            parameters.TryGetValue(PARAM_PANEL, out panelName);
            GraphicPanel panel = GraphicPanelManager.Instance.GetPanel(panelName);
            if(panel == null)
            {
                Debug.LogError($"Unable to grab panel '{panelName}' because it is not a valid panel. Please check the panel name and adjust the command.");
                yield break;
            }

            //Try to get the layer to apply this graphic to
            parameters.TryGetValue(PARAM_LAYER, out layer, defaultValue: -1);

            //Try to get the speed of the transition if it is not an inmmediate effect
            parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultValue: 1);

            if(layer == -1)
                panel.Clear(transitionSpeed);
            else
            {
                GraphicLayer graphicLayer = panel.GetLayer(layer);
                if(graphicLayer == null)
                {
                    Debug.LogError($"Could not clear layer [{layer}] on panel '{panel.panelName}'");
                    yield break;
                }

                graphicLayer.Clear(transitionSpeed);
            } 
        }

        private static IEnumerator SetCinematic(string[] data)
        {
            //Parameters available to function
            PanelType panelName = PanelType.cinematica;
            int layer = 0;
            string mediaName = "";
            float transitionSpeed = 0;
            bool useAudio = true;
            //string gender = "";
            //int level = 0;
            string pathToGraphic = "";
            UnityEngine.Object graphic = null;

            //Now get the parameters
            var parameters = ConvertDataToParameters(data);

            //Try to get the panel that this media is applied to
            GraphicPanel panel = GraphicPanelManager.Instance.GetPanel(panelName);
            if(panel == null)
            {
                Debug.LogError($"Unable to grab panel '{panelName}' because it is not a valid panel. Please check the panel name and adjust the command.");
                yield break;
            }

            //Try to get the layer to apply this graphic to
            parameters.TryGetValue(PARAM_LAYER, out layer, defaultValue: 0);

            //Try to get the graphic
            parameters.TryGetValue(PARAM_MEDIA, out mediaName);

            //Try to get the speed of the transition if it is not an inmmediate effect
            parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultValue: 1);

            //If this is a video, try to get whether we use audio from the video or not
            parameters.TryGetValue(PARAM_USEVIDEOAUDIO, out useAudio, defaultValue: true);

            //Now run the logic
            GameManager gm = GameManager.Instance;
            pathToGraphic = FilePaths.GetPathToResource(FilePaths.resources_backgroundCinematics, $"Nivel.{gm.GetCurrentGameLevel()}/{gm.GetCurrentGameGender()}/{mediaName}");
            graphic = Resources.Load<VideoClip>(pathToGraphic);

            if(graphic == null)
            {
                Debug.LogError($"Could not find media file called '{mediaName}' in the Resources directories. Please specify the field path within resources and make sure it exists!");
                yield break;                
            }

            //Lets try to get the layer to apply the media to
            GraphicLayer graphicLayer  = panel.GetLayer(layer, createIfDoesNotExist: true);

            yield return graphicLayer.SetVideo(graphic as VideoClip, transitionSpeed, useAudio, false, pathToGraphic, AudioBus.Cinematic);
        }
    }
}