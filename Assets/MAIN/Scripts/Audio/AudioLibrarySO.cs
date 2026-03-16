using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio Library", fileName = "AudioLibrary")]
public class AudioLibrarySO : ScriptableObject
{
    public List<AudioEventSO> events = new();

    private Dictionary<string, AudioEventSO> _map;

    public void Build()
    {
        _map = new Dictionary<string, AudioEventSO>(System.StringComparer.OrdinalIgnoreCase);
        foreach (var e in events)
        {
            if (e == null) continue;
            if (string.IsNullOrWhiteSpace(e.id)) continue;
            _map[e.id] = e; // si hay duplicado, el último gana (y es detectable en validación editor si quieres)
        }
    }

    public bool TryGet(string id, out AudioEventSO e)
    {
        if (_map == null) Build();
        return _map.TryGetValue(id, out e);
    }
}

