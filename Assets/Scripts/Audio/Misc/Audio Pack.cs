using UnityEngine;

namespace Player_System.Audio.Misc
{
    [CreateAssetMenu(menuName = "Player System/Audio/Audio Pack", fileName = "Audio Pack")]
    public class AudioPack : ScriptableObject
    {
        [SerializeField] internal AudioClip[] clips;

        internal AudioClip RandomClip => clips[Random.Range(0, clips.Length)];
    }
}