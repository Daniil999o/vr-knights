using UnityEngine;

public class BlockStateBehaviour : StateMachineBehaviour
{
    private NPC_MeleeControl _cachedMeleeControl;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_cachedMeleeControl) 
            _cachedMeleeControl = animator.GetComponent<NPC_MeleeControl>();
        
        _cachedMeleeControl.Blocking = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_cachedMeleeControl) 
            _cachedMeleeControl = animator.GetComponent<NPC_MeleeControl>();
        
        _cachedMeleeControl.Blocking = false;
    }
}