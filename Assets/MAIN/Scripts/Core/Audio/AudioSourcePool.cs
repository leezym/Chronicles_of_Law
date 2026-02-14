using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public sealed class AudioSourcePool
{
    private readonly Transform root;
    private readonly AudioMixerGroup mixer;
    private readonly Queue<AudioSource> free = new();
    private int created = 0;

    public AudioSourcePool(Transform root, AudioMixerGroup mixer, int initialSize)
    {
        this.root = root;
        this.mixer = mixer;

        for (int i = 0; i < initialSize; i++)
            free.Enqueue(CreateNew());
    }

    private AudioSource CreateNew()
    {
        created++;
        var go = new GameObject($"PooledAudioSource_{created}");
        go.transform.SetParent(root, false);

        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.spatialBlend = 0f;
        src.outputAudioMixerGroup = mixer;

        return src;
    }

    public AudioSource Get()
    {
        return free.Count > 0 ? free.Dequeue() : CreateNew();
    }

    public void Release(AudioSource src)
    {
        if (src == null) return;

        // Limpieza para evitar “estado sucio”
        src.Stop();
        src.clip = null;
        src.loop = false;
        src.pitch = 1f;
        src.volume = 1f;
        src.time = 0f;
        src.spatialBlend = 0f;
        src.outputAudioMixerGroup = mixer;

        free.Enqueue(src);
    }
}