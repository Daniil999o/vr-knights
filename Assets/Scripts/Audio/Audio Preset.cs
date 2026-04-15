using UnityEngine;
using UnityEngine.Audio;

namespace Player_System.Audio
{
    [System.Serializable]
    internal class AudioPreset
    {
        [SerializeField][Range(0f, 1f)] internal float volume = 1f;

        [Space]

        [SerializeField] internal AudioRolloffMode rollOffMode = AudioRolloffMode.Linear;
        [SerializeField] internal AudioMixerGroup audioMixer;
        
        [Space]

        [SerializeField] internal Vector2 minMaxDistance = new Vector2(1.5f, 15f);
    }
}