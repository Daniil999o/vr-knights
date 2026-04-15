using UnityEngine;

[CreateAssetMenu(fileName = "Voice", menuName = "Character Voices/Voice")]
public class CharacterVoiceSO : ScriptableObject
{
    public string voiceName;

    [Space]
    
    public AudioClip[] hitClips;
    public AudioClip[] dieClips;

    [Space]

    public AudioClip[] attackClips;
    public AudioClip[] fightClips;
}