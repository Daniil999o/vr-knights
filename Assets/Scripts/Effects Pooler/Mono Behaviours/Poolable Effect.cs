using EasyPoolKit;
using UnityEngine;
using System.Linq;
using UnityEngine.VFX;
using System.Collections.Generic;

public class PoolableEffect : RecyclableMonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject[] objs;
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private VisualEffect[] vfxEffects;
    [SerializeField] private AudioSource[] sources;

    [Space]

    [SerializeField] private AudioSource audioPlayer;
    [SerializeField] private HitClips[] hitClips;

    [System.Serializable]
    private struct HitClips
    {
        public HitStrength hitStrength;
        public AudioClip[] clips;
    }

    private float _despawnTime;

    public void Initialize(HitStrength strength, Vector3 position, Quaternion rotation, Transform parent = null) {
        Dictionary<HitStrength, AudioClip[]> clipsByHitStrength =
            hitClips.ToDictionary(x => x.hitStrength, x => x.clips);
        
        transform.position = position;
        transform.rotation = rotation;
        
        transform.SetParent(parent);

        foreach (var x in objs) x.SetActive(true);
        foreach (var x in particles) x.Play();
        foreach (var x in vfxEffects) x.Play();
        foreach (var x in sources) x.Play();

        if (clipsByHitStrength.TryGetValue(strength, out AudioClip[] clips)) {
            audioPlayer.pitch = Random.Range(0.9f, 1.1f);
            audioPlayer.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
        
        _despawnTime = EffectsPooler.Instance.AutoDespawnTime;
    }

    public override void OnObjectUpdate(float deltaTime) {
        if (!(_despawnTime > 0)) return;
        
        _despawnTime -= deltaTime;
        
        if (_despawnTime <= 0) DespawnSelf();
    }

    public override void OnObjectDespawn() {
        base.OnObjectDespawn();
        
        foreach (var x in objs) x.SetActive(false);
        foreach (var x in particles) x.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        foreach (var x in vfxEffects) x.Stop();
        foreach (var x in sources) x.Stop();
    }
}