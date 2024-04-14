using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace History
{
    [System.Serializable]
    public class AudioData
    { 
        public int channel = 0;
        public string trackName;
        public string trackPath;
        public float trackVolume;
        public bool loop;
        
        public AudioData(AudioChannel channel)
        {
            this.channel = channel.channelIndex;

            if(channel.activeTrack == null)
                return;
            
            var track = channel.activeTrack;
            trackName = track.name;
            trackPath = track.path;
            trackVolume = track.volume;
            loop = track.loop;
        }

        public static List<AudioData> Capture()
        {
            List<AudioData> audioChannels = new List<AudioData>();

            foreach(var channel in AudioManager.Instance.channels)
            {
                if(channel.Value.activeTrack == null)
                    continue;

                AudioData data = new AudioData(channel.Value);
                audioChannels.Add(data);
            }

            return audioChannels;
        }
    }
}