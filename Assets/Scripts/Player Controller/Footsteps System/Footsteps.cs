using System;
using System.Linq;
using UnityEngine;
using Player_System.Audio.Misc;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using AudioExtensions = Player_System.Audio.AudioExtensions;

namespace Player_System.Movement.FootstepsSystem
{
    [AddComponentMenu("Player System/Footsteps System/Footsteps")]
    public class Footsteps : MonoBehaviour
    {
        [Header("Active Surface")]
        [SerializeField] private FootstepsSurface[] surfaces;
        [SerializeField] private LayerMask collectableLayers;

        [Space]

        [SerializeField] private AudioSource armorSource;
        [SerializeField] private AudioPack armorWalkClips;
        [SerializeField] private AudioPack armorRunClips;
        
        [Space]
        
        [Header("Audio")]
        public StepSource[] stepSources;
        
        [Serializable]
        public struct StepSource
        {
            public StepType stepType;
            public AudioSource source;
        }
        

        internal StepType MyStepType { get; set; } = StepType.Walk;

        private AudioSource _runS;
        private AudioSource _walkS;
        private AudioSource _crouchS;
        private AudioSource _jumpS;
        private AudioSource _landS;
        private AudioSource _ladderS;

        private readonly Dictionary<StepType, AudioSource> _sourcesMap = new Dictionary<StepType, AudioSource>();

        private AudioSource GetSourceByStep(StepType stepType) {
            if (_sourcesMap.TryGetValue(stepType, out AudioSource s))
                return s;
            
            return stepSources.Where(x => x.stepType == stepType).Select(x => x.source).FirstOrDefault();
        }

        private Transform _transform;

        private FootstepsSurface GetSuitableSurface(GameObject surface, Vector3 point) {
            FootstepsSurface[] suitableSurfaces= surfaces.Where(x => x.IsSurfaceSuitable(surface, point)).ToArray();

            return suitableSurfaces.FirstOrDefault();
        }
        
        public float ControllerSpeed { get; set; }

        private void Awake() {
            _transform = GetComponent<Transform>();

            foreach (var x in (StepType[])Enum.GetValues(typeof(StepType))) {
                StepType stepType = x;
                _sourcesMap.Add(stepType, GetSourceByStep(stepType));
            }
        }

        private void OnDisable() {
            foreach (var x in stepSources)
                x.source.Stop();
        }

        internal void PlayFoley(FoleyData foleyData, Vector3 pos) {
            if (!foleyData) return;

            AudioSource source = AudioExtensions.ShotAudioSourceWithPreset(pos, foleyData.audioPreset);

            source.clip = foleyData.RandomClip;
            
            source.Play();
            
            Destroy(source.gameObject, source.clip.length + 0.1f);
        }

        public void PlayStepVault() => PlayFootStep(StepType.Walk, false);
        public void PlayStepVaultLand() => PlayFootStep(StepType.Land, false);

        public void PlayStepWalk() {
            if (MyStepType != StepType.Walk) return;
            
            PlayFootStep(StepType.Walk);
        }

        public void PlayStepCrouch() {
            if (MyStepType != StepType.Crouch) return;
            
            PlayFootStep(StepType.Crouch);
        }

        public void PlayStepRun() {
            if (MyStepType != StepType.Run) return;
            
            PlayFootStep(StepType.Run);
        }

        public void PlayStepJump() => PlayFootStep(StepType.Jump);
        public void PlayStepLand() => PlayFootStep(StepType.Land);

        public void PlayFootStep(StepType stepType, bool takeSpeed = true) {
            if (takeSpeed && ControllerSpeed < 0.01f) return;
            
            Ray ray = new Ray(_transform.position + Vector3.up * 0.1f, Vector3.down);
            
            if (!Physics.Raycast(ray, out RaycastHit hit, collectableLayers))
                return;

            FootstepsSurface surface = GetSuitableSurface(hit.transform.gameObject, hit.point);

            if (!surface) return;
            
            AudioSource source = GetSourceByStep(stepType);

            if (!source) return;

            if (armorSource) {
                AudioPack pack = stepType == StepType.Run ? armorRunClips : armorWalkClips;

                armorSource.pitch = Random.Range(0.9f, 1.1f);
                armorSource.PlayOneShot(pack.RandomClip);
            }
            
            source.pitch = Random.Range(0.9f, 1.1f);
            source.PlayOneShot(surface.GetRandomClip(stepType));
        }
    }
}