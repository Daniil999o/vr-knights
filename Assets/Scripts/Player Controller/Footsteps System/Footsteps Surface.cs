using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace Player_System.Movement.FootstepsSystem
{
    [CreateAssetMenu(menuName = "Player System/Movement/Footsteps Surface", fileName = "Footsteps Surface")]
    public class FootstepsSurface : ScriptableObject
    {
        public string surfaceName;

        [Space]

        [Header("Checking Options")]
        [SerializeField] private bool useAnywhere;
        [SerializeField] private bool separateChecking;
        [SerializeField] private LayerMask surfaceLayers;
        [SerializeField] private string[] surfaceTags;
        [SerializeField] private int[] surfaceTerrainLayers;

        [Space]

        [Header("Audio Clips")]
        public StepClips[] stepClipsLibrary;
        
        [System.Serializable]
        public struct StepClips
        {
            public StepType stepType;
            public AudioClip[] clips;
        }

        private readonly Dictionary<StepType, StepClips[]> _stepClipsMap = new Dictionary<StepType, StepClips[]>();

        public AudioClip GetRandomClip(StepType stepType) {
            StepClips[] myClips;

            if (_stepClipsMap.TryGetValue(stepType, out StepClips[] stepClips)) {
                myClips = stepClips;
            } else {
                myClips = stepClipsLibrary.Where(x => x.stepType == stepType).ToArray();
                _stepClipsMap[stepType] = myClips;
            }
            
            AudioClip[] clips = myClips.SelectMany(x => x.clips).ToArray();

            return clips[Random.Range(0, clips.Length)];
        }

        public bool IsSurfaceSuitable(GameObject surface, Vector3 point) {
            if (useAnywhere) return true;

            bool isInLayers = (surfaceLayers & (1 << surface.layer)) != 0;
            
            if (isInLayers && separateChecking) return true;

            bool hasTags = surfaceTags != null && surfaceTags.Length > 0;
            bool isInTags = !hasTags && !separateChecking || surfaceTags.Contains(surface.tag);
            
            if (isInTags && separateChecking)
                return true;

            bool hasTerrainLayers = surfaceTerrainLayers != null && surfaceTerrainLayers.Length > 0;
            bool isInTerrainLayers = false;

            if (hasTerrainLayers) {
                if (!surface.TryGetComponent(out FootstepsTerrain terrain) && !separateChecking)
                    isInTerrainLayers = true;
                else if (terrain)
                    isInTerrainLayers = SuitableInTerrainLayers(terrain, surface, point);
            } else if (!separateChecking) {
                isInTerrainLayers = true;
            }
            
            if (hasTerrainLayers && isInTerrainLayers && separateChecking) return true;

            return isInLayers && isInTags && isInTerrainLayers;
        }

        private bool SuitableInTerrainLayers(FootstepsTerrain terrain, GameObject surface, Vector3 point) {
            int layerIndex = terrain.GetLayerIndex(point);

            return surfaceTerrainLayers.Contains(layerIndex);
        }
    }
}