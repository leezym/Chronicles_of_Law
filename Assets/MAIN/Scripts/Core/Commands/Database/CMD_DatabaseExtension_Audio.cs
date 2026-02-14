using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_Audio : CMD_DatabaseExtension
    {
        private static string PARAM_SOUND = "-sonido";
        private static string PARAM_CHANNEL = "-canal";
        private static string PARAM_LOOP = "-loop";
        private static string PARAM_VOLUME = "-vol";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("reproducirSonido", new Action<string[]>(PlaySFX));
            database.AddCommand("detenerSonido", new Action<string>(StopSFX));

            database.AddCommand("reproducirVoz", new Action<string[]>(PlayVoice));
            database.AddCommand("detenerVoz", new Action<string>(StopSFX));

            database.AddCommand("reproducirMusica", new Action<string[]>(PlaySong));
            database.AddCommand("reproducirAmbiente", new Action<string[]>(PlayAmbience));
            database.AddCommand("detenerMusica", new Action<string>(StopSong)); 
            database.AddCommand("detenerAmbiente", new Action<string>(StopAmbience)); 
        }

        private static void PlaySFX(string[] data)
        {
            string filePath;
            float volumeCap;
            bool loop;

            var parameters = ConvertDataToParameters(data);

            //Try to get the name or path to the track
            parameters.TryGetValue(PARAM_SOUND, out filePath);
            filePath = FilePaths.GetPathToResource(FilePaths.resources_sfx, filePath);

            //Try to get the max volume of the track
            parameters.TryGetValue(PARAM_VOLUME, out volumeCap, defaultValue: 1f);

            parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: false);

            //Run the logic
            AudioClip sound = Resources.Load<AudioClip>(filePath);

            if(sound == null)
            {
                Debug.LogError($"Could not load sound from path '{filePath}.' Please ensure it exists within Resources!");
                return;
            }

            //AudioManager.Instance.PlaySoundEffect(sound, volumeCap, loop);
            AudioManager.Instance.PlaySoundEffect(sound, volume: volumeCap, loop: loop);
        }

        private static void PlayVoice(string[] data)
        {
            string filePath;
            float volumeCap;
            bool loop;

            var parameters = ConvertDataToParameters(data);

            //Try to get the name or path to the track
            parameters.TryGetValue(PARAM_SOUND, out filePath);
            filePath = FilePaths.GetPathToResource(FilePaths.resources_voices, filePath);

            parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: false);
            
            //Try to get the max volume of the track
            parameters.TryGetValue(PARAM_VOLUME, out volumeCap, defaultValue: 1f);

            //Run the logic
            AudioClip sound = Resources.Load<AudioClip>(filePath);

            if(sound == null)
            {
                Debug.LogError($"Could not load sound from path '{filePath}.' Please ensure it exists within Resources!");
                return;
            }

            AudioManager.Instance.PlayVoice(sound, volumeCap, loop);
        }

        private static void PlaySong(string[] data)
        {
            string filePath;
            int channel;
            AudioBus bus = AudioBus.Music;

            var parameters = ConvertDataToParameters(data);

            //Try to get the name or path to the track
            parameters.TryGetValue(PARAM_SOUND, out filePath);
            filePath = FilePaths.GetPathToResource(FilePaths.resources_music, filePath);

            //Try to get the channel for this track
            parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultValue: 0);

            PlayTrack(filePath, channel, parameters, bus);
        }

        private static void PlayAmbience(string[] data)
        {
            string filePath;
            int channel;
            AudioBus bus = AudioBus.Ambience;

            var parameters = ConvertDataToParameters(data);

            //Try to get the name or path to the track
            parameters.TryGetValue(PARAM_SOUND, out filePath);
            filePath = FilePaths.GetPathToResource(FilePaths.resources_ambience, filePath);

            //Try to get the channel for this track
            parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultValue: 0);

            PlayTrack(filePath, channel, parameters, bus);
        }

        private static void PlayTrack(string filePath, int channel, CommandParameters parameters, AudioBus bus)
        {
            bool loop;
            float volumeCap;
            
            //Try to get the max volume of the track
            parameters.TryGetValue(PARAM_VOLUME, out volumeCap, defaultValue: 1f);

            parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: true);

            //Run the logic
            AudioClip sound = Resources.Load<AudioClip>(filePath);

            if(sound == null)
            {
                Debug.LogError($"Could not load sound from path '{filePath}.' Please ensure it exists within Resources!");
                return;
            }

            AudioManager.Instance.PlayTrack(sound, bus, channel, loop, volumeCap, filePath);
        }

        public static void StopSFX(string data)
        {
            AudioManager.Instance.StopSoundEffect(data);
        }

        public static void StopSong(string data)
        {
            if(data == string.Empty)
                StopTrack("1");
            else
                StopTrack(data);
        }

        public static void StopAmbience(string data)
        {
            if(data == string.Empty)
                StopTrack("0");
            else
                StopTrack(data);
        }

        public static void StopTrack(string data)
        {
            if(int.TryParse(data, out int channel))
                AudioManager.Instance.StopTrack(channel);
            else
                AudioManager.Instance.StopTrack(data);
        }
    }
}
