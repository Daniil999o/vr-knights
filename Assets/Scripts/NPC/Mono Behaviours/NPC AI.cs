using UnityEngine;

public class NPC_AI : MonoBehaviour
{
    [field: SerializeField] public NPC_Settings Data { get; private set; }
    
    [Space]
    
    [SerializeField] private NPC_Blackboard blackboard;
    
    public NPC_Blackboard Blackboard => blackboard;
    
    private NPC_FSM Fsm { get; set; }

    private void Awake() {
        Blackboard.hashNames.UpdateHashes();
    }

    private void Start() {
        Fsm = new NPC_FSM();

        Blackboard.agent.speed = Data.walkSpeed;
        Blackboard.agent.angularSpeed = Data.walkAngular;
        Blackboard.agent.acceleration = Data.acceleration;

        Blackboard.agent.autoBraking = Data.autoBraking;
        Blackboard.agent.stoppingDistance = Data.stoppingDistance;
        
        StartSearchState();
    }

    private void Update() {
        Fsm.CurrentState?.Update(Time.deltaTime);
        Fsm.CurrentState?.UpdateAnimator();
    }

    public void StartSearchState() {
        Fsm.ChangeState(new MeleeSearchTargetState(this));
    }

    public void StartFollowState() {
        Fsm.ChangeState(new MeleeFollowState(this));
    }

    public void StartAttackState() {
        Fsm.ChangeState(new MeleeAttackState(this));
    }
    
    public void StartBlockState() {
        Fsm.ChangeState(new MeleeBlockState(this));
    }

    public void CancelAttack() => StartFollowState();
}