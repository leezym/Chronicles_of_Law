using UnityEngine;

public sealed class AudioHandle
{
    public string id;
    public AudioBus bus;
    public AudioSource source;
    public bool IsValid => source != null;
}
