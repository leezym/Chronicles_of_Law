using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio Event", fileName = "AE_")]
public class AudioEventSO : ScriptableObject
{
    [Header("Identity")]
    public string id;                 // "ui.click.button"
    public AudioBus bus = AudioBus.SFX;

    [Header("Clips (variants)")]
    public AudioClip[] clips;

    [Header("Volume")]
    [Range(0f, 1f)] public float volume = 1f;

    [Header("Pitch")]
    public bool randomizePitch = false;
    [Range(-3f, 3f)] public float pitchMin = 1f;
    [Range(-3f, 3f)] public float pitchMax = 1f;

    [Header("Anti-spam")]
    [Min(0f)] public float cooldownSeconds = 0f;   // evita hover/click spameado

    [Header("Polyphony")]
    [Min(0)] public int maxSimultaneous = 0;       // 0 = ilimitado

    [Header("Loop (solo si aplica)")]
    public bool loop = false;

    public AudioClip PickClip()
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }

    public float PickPitch()
    {
        if (!randomizePitch) return 1f;
        return Random.Range(pitchMin, pitchMax);
    }
}
