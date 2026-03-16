using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HISTORY
{
    [System.Serializable]
    public class AudioData
    { 
        public int channel = 0;
        public string trackName;
        public string trackPath;
        public float trackVolume;
        public AudioBus bus; // nuevo
        
        public AudioData(AudioChannel channel)
        {
            this.channel = channel.channelIndex;
            this.bus = channel.bus; //nuevo

            if(channel.activeTrack == null)
                return;
            
            var track = channel.activeTrack;
            trackName = track.name;
            trackPath = track.path;
            trackVolume = track.volume;
        }

        public static List<AudioData> Capture()
        {
            var audioChannels = new List<AudioData>();

            foreach (var channel in AudioManager.Instance.channels)
            {
                if(channel.Value.activeTrack == null)
                    continue;

                AudioData data = new AudioData(channel.Value);
                audioChannels.Add(data);
            }

            foreach (var channel in AudioManager.Instance.ambienceChannels)
            {
                if(channel.Value.activeTrack == null)
                    continue;

                AudioData data = new AudioData(channel.Value);
                audioChannels.Add(data);
            }            

            return audioChannels;
            /*List<AudioData> audioChannels = new List<AudioData>();

            foreach(var channel in AudioManager.Instance.channels)
            {
                if(channel.Value.activeTrack == null)
                    continue;

                AudioData data = new AudioData(channel.Value);
                audioChannels.Add(data);
            }

            return audioChannels;*/
        }

        public static void Apply(List<AudioData> data)
        {
            List<int> cache = new List<int>();

            foreach(var channelData in data)
            {
                AudioChannel channel = AudioManager.Instance.TryGetChannel(channelData.channel, channelData.bus, createIfDoesNotExist: true); //nuevo
                if(channel.activeTrack == null || channel.activeTrack.name != channelData.trackName)
                {
                    AudioClip clip = HistoryCache.LoadAudio(channelData.trackPath);
                    if(clip != null)
                    {
                        channel.StopTrack(immediate: true);
                        channel.PlayTrack(clip, true, channelData.trackVolume, channelData.trackPath); //pdte
                    }
                    else
                        Debug.LogWarning($"History State: Could not load audio track '{channelData.trackPath}'");
                }

                cache.Add(channelData.channel);
            }

            foreach(var channel in AudioManager.Instance.channels)
            {
                if(!cache.Contains(channel.Value.channelIndex))
                    channel.Value.StopTrack(immediate: true);
            }
        }
    }
}