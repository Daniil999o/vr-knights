using UnityEngine;

public class CharacterVoiceController : MonoBehaviour
{
    [SerializeField] private CharacterVoiceSO[] randomConfigs;
    
    [Space]
    
    [SerializeField] private CharacterVoiceSO config;

    [Space]

    [SerializeField] private AudioSource voiceSource;

    private float _delay;
    
    private float _voicePitch;

    private bool _died;

    private void Awake() {
        _voicePitch = Random.Range(0.95f, 1.05f);
        
        if (randomConfigs.Length > 0)
            config = randomConfigs[Random.Range(0, randomConfigs.Length)];
    }

    public void PlayHit() {
        if (Random.Range(0f, 1f) < 0.25f) return;
        
        PlayVoice(config.hitClips, true);
    }
    
    public void PlayDie(bool setDied = true) {
        PlayVoice(config.dieClips, true);
        
        if (setDied) _died = true;
    }
    
    public void PlayAttack() {
        if (Random.Range(0f, 1f) < 0.4f) return;
        
        PlayVoice(config.attackClips);
    }
    
    public void PlayFight() {
        if (Random.Range(0f, 1f) < 0.6f) return;
        
        PlayVoice(config.fightClips);
    }

    private void PlayVoice(AudioClip[] clips, bool interrupt = false) {
        if (_died) return;
        
        if (CantSay && !interrupt) return;

        _delay = 2f;
        
        if (clips.Length <= 0) return;
        
        voiceSource.pitch = _voicePitch;
        
        voiceSource.clip = clips[Random.Range(0, clips.Length)];
        voiceSource.Play();
    }

    private void Update() {
        if (_delay > 0f) _delay -= Time.deltaTime;
    }

    private bool CantSay => _delay > 0f;
}