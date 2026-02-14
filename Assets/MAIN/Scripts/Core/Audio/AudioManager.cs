using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public enum AudioBus
{
    Music,
    Ambience,
    UI,
    SFX,
    Voices,
    Cinematic
}

public class AudioManager : MonoBehaviour
{
    private const string SFX_PARENT_NAME = "SFX";
    private const string UI_PARENT_NAME = "UI";
    private const string SFX_NAME_FORMAT = "SFX - [{0}]";
    public const float TRACK_TRANSITION_SPEED = 1f;

    public static AudioManager Instance { get; private set; }

    public Dictionary<int, AudioChannel> channels = new Dictionary<int, AudioChannel>();
    public Dictionary<int, AudioChannel> ambienceChannels = new Dictionary<int, AudioChannel>();

    [Header("Mixer Groups")]
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup ambienceMixer;
    public AudioMixerGroup uiMixer;
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup voiceMixer;
    public AudioMixerGroup cinematicMixer;

    [Header("Audio Events")]
    public AudioLibrarySO audioLibrary;

    private readonly Dictionary<string, float> _lastPlayById = new(System.StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _playingCountById = new(System.StringComparer.OrdinalIgnoreCase);
    
    [Header("Pooling")]
    [SerializeField] private int uiPoolSize = 12;
    [SerializeField] private int sfxPoolSize = 24;
    private AudioSourcePool uiPool;
    private AudioSourcePool sfxPool;

    private readonly Dictionary<string, AudioHandle> activeLoopsById = new Dictionary<string, AudioHandle>(StringComparer.OrdinalIgnoreCase);

    private Transform uiRoot;
    private Transform sfxRoot;

    private void Awake()
    {
        if(Instance == null)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }

        uiRoot = new GameObject(UI_PARENT_NAME).transform;
        uiRoot.SetParent(transform, false);

        sfxRoot = new GameObject(SFX_PARENT_NAME).transform;
        sfxRoot.SetParent(transform, false);

        if (uiMixer == null) Debug.LogWarning("AudioManager: uiMixer is not assigned.");
        if (sfxMixer == null) Debug.LogWarning("AudioManager: sfxMixer is not assigned.");

        uiPool = new AudioSourcePool(uiRoot, uiMixer, uiPoolSize);
        sfxPool = new AudioSourcePool(sfxRoot, sfxMixer, sfxPoolSize);

        if (audioLibrary != null) audioLibrary.Build();
    }

    public AudioMixerGroup GetMixerGroup(AudioBus bus)
    {
        return bus switch
        {
            AudioBus.Music => musicMixer,
            AudioBus.Ambience => ambienceMixer,
            AudioBus.UI => uiMixer,
            AudioBus.SFX => sfxMixer,
            AudioBus.Voices => voiceMixer,
            AudioBus.Cinematic => cinematicMixer,
            _ => null
        };
    }

    public AudioSource PlaySoundEffect(string filePath, AudioMixerGroup mixer = null, float volume = 1, bool loop = false)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if(clip == null)
        {
            Debug.LogError($"Could not load audio from path '{filePath}'. Please ensure it exists within Resources!");
            return null;
        }

        return PlaySoundEffect(clip, mixer, volume, loop);
    }

    public AudioSource PlaySoundEffect(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, bool loop = false)
    {
        AudioSource effectSource = new GameObject(string.Format(SFX_NAME_FORMAT, clip.name)).AddComponent<AudioSource>();
        effectSource.transform.SetParent(sfxRoot);
        effectSource.transform.position = sfxRoot.position;

        effectSource.clip = clip;

        if(mixer == null)
            mixer = sfxMixer;

        effectSource.outputAudioMixerGroup = mixer;
        effectSource.volume = volume;
        effectSource.loop = loop;
        effectSource.spatialBlend = 0;

        effectSource.Play();

        if(!loop)
            Destroy(effectSource.gameObject, clip.length + 1);

        return effectSource;
    }

    public void StopSoundEffect(AudioClip clip) => StopSoundEffect(clip.name);

    public void StopSoundEffect(string soundName)
    {
        soundName = soundName.ToLower();

        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
        foreach(var source in sources)
        {
            if (source.clip != null && source.clip.name != null && source.clip.name.ToLower() == soundName)
            {
                Destroy(source.gameObject);
                return;
            }
        }
    }

    public AudioSource PlayVoice(string filePath, float volume = 1, bool loop = false)
    {
        return PlaySoundEffect(filePath, voiceMixer, volume, loop);
    }

    public AudioSource PlayVoice(AudioClip clip, float volume = 1, bool loop = false)
    {
        return PlaySoundEffect(clip, voiceMixer, volume, loop);
    }

    public AudioTrack PlayTrack(string filePath, AudioBus bus, int channel = 0, bool loop = true, float volumeCap = 1f)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if(clip == null)
        {
            Debug.LogError($"Could not load audio from path '{filePath}'. Please ensure it exists within Resources!");
            return null;
        }

        return PlayTrack(clip, bus, channel, loop, volumeCap, filePath);
    }

    public AudioTrack PlayTrack(AudioClip clip, AudioBus bus, int channel = 0, bool loop = true, float volumeCap = 1f, string filePath = "")
    {
        AudioChannel audioChannel = TryGetChannel(channel, bus, createIfDoesNotExist: true);
        AudioTrack track = audioChannel.PlayTrack(clip, loop, volumeCap, filePath);
        return track;
    }

    public void StopTrack(int channel)
    {
        AudioChannel c = TryGetChannel(channel, createIfDoesNotExist: false);

        if(c == null)
            return;

        c.StopTrack();
    }

    public void StopTrack(string trackName)
    {
        trackName = trackName.ToLower();

        foreach(var channel in channels.Values)
        {
            if(channel.activeTrack != null && channel.activeTrack.name.ToLower() == trackName)
            {
                channel.StopTrack();
                return;
            }
        }
    }

    public void PlayEvent(string id)
    {
        if (audioLibrary == null)
        {
            Debug.LogWarning("AudioManager: audioLibrary not assigned.");
            return;
        }

        if (!audioLibrary.TryGet(id, out var e) || e == null)
        {
            Debug.LogWarning($"AudioManager: AudioEvent not found: '{id}'");
            return;
        }

        PlayEvent(e);
    }

    public void PlayEvent(AudioEventSO e)
    {
        if (e == null) return;

        if (e.bus == AudioBus.Music || e.bus == AudioBus.Ambience)
        {
            Debug.LogWarning($"AudioEvent '{e.id}' is {e.bus}. Use channel-based PlayTrack for crossfade/persistence.");
            return;
        }

        // Cooldown
        if (e.cooldownSeconds > 0f)
        {
            if (_lastPlayById.TryGetValue(e.id, out var last) && Time.unscaledTime - last < e.cooldownSeconds)
                return;

            _lastPlayById[e.id] = Time.unscaledTime;
        }

        // Polyphony
        if (e.maxSimultaneous > 0)
        {
            _playingCountById.TryGetValue(e.id, out var count);
            if (count >= e.maxSimultaneous) return;
            _playingCountById[e.id] = count + 1;
        }
        
        if (e.bus == AudioBus.UI)
        {
            var clip = e.PickClip();
            if (clip == null) { DecrementPolyphonyIfNeeded(e); return; }
            PlayOneShot_FromPool(uiPool, e, clip);
            return;
        }

        if (e.bus == AudioBus.SFX)
        {
            var clip = e.PickClip();
            if (clip == null) { DecrementPolyphonyIfNeeded(e); return; }

            if (!e.loop)
            {
                PlayOneShot_FromPool(sfxPool, e, clip);
            }
            else
            {
                if (activeLoopsById.TryGetValue(e.id, out var existing) && existing != null && existing.source != null)
                    return;

                var h = PlayLoop_FromPool(sfxPool, e, clip);
                activeLoopsById[e.id] = h;
            }
            return;
        }
    }

    private void PlayOneShot_FromPool(AudioSourcePool pool, AudioEventSO e, AudioClip clip)
    {
        var src = pool.Get();

        src.outputAudioMixerGroup = (e.bus == AudioBus.UI) ? uiMixer : sfxMixer; 
        src.clip = clip;
        src.loop = false;
        src.volume = e.volume;
        src.pitch = e.PickPitch();
        src.spatialBlend = 0f;

        src.Play();

        StartCoroutine(ReleaseToPoolAfter(pool, src, clip.length, e));
    }

    private AudioHandle PlayLoop_FromPool(AudioSourcePool pool, AudioEventSO e, AudioClip clip)
    {
        var src = pool.Get();

        src.outputAudioMixerGroup = (e.bus == AudioBus.UI) ? uiMixer : sfxMixer;
        src.clip = clip;
        src.loop = true;
        src.volume = e.volume;
        src.pitch = e.PickPitch();
        src.spatialBlend = 0f;

        src.Play();

        return new AudioHandle
        {
            id = e.id,
            bus = e.bus,
            source = src
        };
    }

    public AudioHandle PlayLoop(AudioClip clip, AudioBus bus, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return null;

        var pool = (bus == AudioBus.UI) ? uiPool : sfxPool;
        var mixer = (bus == AudioBus.UI) ? uiMixer : sfxMixer;

        var src = pool.Get();
        src.outputAudioMixerGroup = mixer;
        src.clip = clip;
        src.loop = true;
        src.volume = volume;
        src.pitch = pitch;
        src.spatialBlend = 0f;

        src.Play();

        return new AudioHandle
        {
            id = clip.name,
            bus = bus,
            source = src
        };
    }

    public void StopLoop(AudioHandle handle)
    {
        if (handle == null || handle.source == null) return;

        var pool = (handle.bus == AudioBus.UI) ? uiPool : sfxPool;
        pool.Release(handle.source);

        handle.source = null;
    }

    public AudioHandle PlayLoopById(string id, AudioClip clip, AudioBus bus, float volume = 1f, float pitch = 1f)
    {
        if (string.IsNullOrWhiteSpace(id) || clip == null) return null;

        if (activeLoopsById.TryGetValue(id, out var existing) && existing != null && existing.source != null)
            return existing;

        var h = PlayLoop(clip, bus, volume, pitch);
        if (h != null)
        {
            h.id = id;
            activeLoopsById[id] = h;
        }
        return h;
    }

    public void StopLoop(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return;

        if (!activeLoopsById.TryGetValue(id, out var h) || h == null) return;

        StopLoop(h);
        activeLoopsById.Remove(id);

        DecrementPolyphonyById(id);
    }

    private System.Collections.IEnumerator ReleaseToPoolAfter(AudioSourcePool pool, AudioSource src, float seconds, AudioEventSO eRef)
    {
        yield return new WaitForSecondsRealtime(seconds);

        pool.Release(src);

        // polyphony decrement (si usas maxSimultaneous)
        DecrementPolyphonyIfNeeded(eRef);
    }

    private void DecrementPolyphonyIfNeeded(AudioEventSO e)
    {
        if (e == null) return;
        if (e.maxSimultaneous <= 0) return;

        _playingCountById.TryGetValue(e.id, out var count);
        count = Mathf.Max(0, count - 1);
        _playingCountById[e.id] = count;
    }

    private void DecrementPolyphonyById(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return;

        _playingCountById.TryGetValue(id, out var count);
        count = Mathf.Max(0, count - 1);
        _playingCountById[id] = count;
    }

    /*public AudioChannel TryGetChannel(int channelNumber, bool createIfDoesNotExist = false)
    {
        AudioChannel channel;

        if(channels.TryGetValue(channelNumber, out channel))
        {
            return channel;
        }
        else if(createIfDoesNotExist)
        {
            channel = new AudioChannel(channelNumber);

            channels.Add(channelNumber, channel);
            return channel;
        }

        return null;
    }*/

    public AudioChannel TryGetChannel(int channelNumber, bool createIfDoesNotExist = false) => TryGetChannel(channelNumber, AudioBus.Music, createIfDoesNotExist); // nuevo

    public AudioChannel TryGetChannel(int channelNumber, AudioBus bus, bool createIfDoesNotExist = false) // nuevo
    {
        var dict = (bus == AudioBus.Ambience) ? ambienceChannels : channels;
        AudioChannel channel;

        if(dict.TryGetValue(channelNumber, out channel))        
            return channel;
        
        if(createIfDoesNotExist)
        {
            channel = new AudioChannel(channelNumber, bus);

            dict.Add(channelNumber, channel);
            return channel;
        }

        return null;
    }
}
