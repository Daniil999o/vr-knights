using UnityEngine;
using Player_System.Audio;

namespace Player_System.Movement.FootstepsSystem
{
    [CreateAssetMenu(menuName = "Player System/Movement/Foley Data", fileName = "Foley Data")]
    public class FoleyData : ScriptableObject
    {
        [SerializeField] private string foleyName = "New Foley";
        
        [Space]
        
        [SerializeField] private AudioClip[] foleyClips;

        [Space]

        [SerializeField] internal AudioPreset audioPreset;

        internal AudioClip RandomClip => foleyClips[Random.Range(0, foleyClips.Length)];
    }
}