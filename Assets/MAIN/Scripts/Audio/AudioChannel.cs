using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioChannel
{
    private const string TRACK_CONTAINER_NAME_FORMAT = "Channel - [{0}]";

    public AudioBus bus { get; private set; }
    public int channelIndex { get; private set; }

    public Transform trackContainer { get; private set; } = null;
    public AudioTrack activeTrack { get ; private set; } = null;
    private List<AudioTrack> tracks = new List<AudioTrack>();

    Coroutine co_volumeLeveling = null;
    bool isLevelingVolume => co_volumeLeveling != null;

    public AudioChannel(int channel, AudioBus bus)
    {
        channelIndex = channel;
        this.bus = bus;

        trackContainer = new GameObject($"Channel - {bus} - [{channel}]").transform;
        trackContainer.SetParent(AudioManager.Instance.transform);
    }

    public AudioTrack PlayTrack(AudioClip clip, bool loop, float volumeCap, string filePath)
    {
        if (TryGetTrack(clip.name, out AudioTrack existingTrack))
        {
            if (!existingTrack.isPlaying)
                existingTrack.Play();

            SetAsActiveTrack(existingTrack);
            return existingTrack;
        }

        AudioMixerGroup mixer = AudioManager.Instance.GetMixerGroup(bus);

        if (mixer == null)
        {
            Debug.LogWarning($"AudioChannel: no mixer assigned for bus '{bus}'. Falling back to musicMixer.");
            mixer = AudioManager.Instance.musicMixer;
        }

        AudioTrack track = new AudioTrack(clip, loop, volumeCap, this, mixer, filePath);

        track.Play();
        SetAsActiveTrack(track);

        return track;
    }

    public bool TryGetTrack(string trackName, out AudioTrack value)
    {
        trackName = trackName.ToLower();

        foreach(var track in tracks)
        {
            if(track.name.ToLower() == trackName)
            {
                value = track;
                return true;
            }
        }

        value = null;
        return false;
    }

    private void SetAsActiveTrack(AudioTrack track)
    {
        if(!tracks.Contains(track))
            tracks.Add(track);
        
        activeTrack = track;

        TryStartVolumeLeveling();
    }

    private void TryStartVolumeLeveling()
    {
        if(!isLevelingVolume)
            co_volumeLeveling = AudioManager.Instance.StartCoroutine(VolumeLeveling());

    }

    private IEnumerator VolumeLeveling()
    {
        while((activeTrack != null && (tracks.Count > 1 || activeTrack.volume != activeTrack.volumeCap)) || (activeTrack == null && tracks.Count> 0))
        {
            for(int i = tracks.Count - 1; i >= 0; i--)
            {
                AudioTrack track = tracks[i];

                float targetVol = activeTrack == track ? track.volumeCap : 0;

                if(track == activeTrack && track.volume == targetVol)
                    continue;
                
                track.volume = Mathf.MoveTowards(track.volume, targetVol, AudioManager.TRACK_TRANSITION_SPEED * Time.deltaTime);

                if(track != activeTrack && track.volume == 0)
                {
                    DestroyTrack(track);
                }
            }
            yield return null;
        }
        co_volumeLeveling = null;
    }

    private void DestroyTrack(AudioTrack track)
    {
        if(tracks.Contains(track))
            tracks.Remove(track);
        
        Object.Destroy(track.root);
    }

    public void StopTrack(bool immediate = false)
    {
        if(activeTrack == null)
            return;
        
        if(immediate)
        {
            DestroyTrack(activeTrack);
            activeTrack = null;
        }   
        else
        {    
            activeTrack = null;
            TryStartVolumeLeveling();
        }
    }
}
