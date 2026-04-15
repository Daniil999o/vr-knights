using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Player_System.AnimationSystem
{
    [AddComponentMenu("Player System/Animation System/Animator Helper")]
    public class AnimatorHelper : MonoBehaviour
    {
        [SerializeField] private AnimatorEvent[] animatorEvents;

        [System.Serializable]
        private struct AnimatorEvent
        {
            public string name;
            public UnityEvent events;
        }

        public void PlayEvent(string eventName) {
            animatorEvents.Where(x => x.name == eventName).ToList()
                .ForEach(x => x.events?.Invoke());
        }
    }
}