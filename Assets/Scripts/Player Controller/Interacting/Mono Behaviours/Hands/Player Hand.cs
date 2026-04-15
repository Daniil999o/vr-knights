using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private HandSettingsSO handSettings;
    
    [field: Space]
    
    [field: SerializeField] public bool IsBusy { get; set; }
    
    [field: Space]
    
    [field: SerializeField] public bool Pinch { get; set; }
    [field: SerializeField] public bool Grab { get; set; }

    [Space]

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private string pinchAnimName;
    [SerializeField] private string grabAnimName;

    private float _pinch;
    private float _grab;

    private int _pinchAnim;
    private int _grabAnim;

    private void Awake() {
        _pinchAnim = Animator.StringToHash(pinchAnimName);
        _grabAnim = Animator.StringToHash(grabAnimName);
    }

    private void Update() {
        _pinch = Mathf.MoveTowards(_pinch, Pinch ? 1f : 0f, handSettings.pinchSpeed * Time.deltaTime);
        _grab = Mathf.MoveTowards(_grab, Grab ? 1f : 0f, handSettings.grabSpeed * Time.deltaTime);
        
        playerAnimator.SetFloat(_pinchAnim, _pinch);
        playerAnimator.SetFloat(_grabAnim, _grab);
    }
}