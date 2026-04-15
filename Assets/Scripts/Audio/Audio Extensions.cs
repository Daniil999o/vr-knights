using UnityEngine;

namespace Player_System.Audio
{
    internal static class AudioExtensions
    {
        internal static AudioSource AddAudioSourceWithPreset(this GameObject obj, AudioPreset preset) {
            AudioSource source = obj.AddComponent<AudioSource>();

            if (preset == null) {
                preset = new AudioPreset {
                    volume = 1f, rollOffMode = AudioRolloffMode.Linear, minMaxDistance = new Vector2(1.5f, 15f)
                };
            }

            if (preset.audioMixer) source.outputAudioMixerGroup = preset.audioMixer;
            
            source.volume = preset.volume;

            source.spatialBlend = 1f;
            
            source.rolloffMode = preset.rollOffMode;
            
            source.minDistance = preset.minMaxDistance.x;
            source.maxDistance = preset.minMaxDistance.y;

            return source;
        }

        internal static AudioSource ShotAudioSourceWithPreset(Vector3 pos, AudioPreset preset) {
            GameObject obj = new GameObject($"Audio Shot");
            obj.transform.position = pos;

            return obj.AddAudioSourceWithPreset(preset);
        }
    }
}