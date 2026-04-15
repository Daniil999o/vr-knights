public class MeleeSearchTargetState : GroundedState
{
    private const float SearchTimer = 3f;

    private float _timer;
    
    public MeleeSearchTargetState(NPC_AI controller) : base(controller) { }
    
    public override void Enter() {
        base.Enter();

        Blackboard.agent.isStopped = true;
    }

    public override void Update(float delta) {
        base.Update(delta);

        if (Blackboard.HasTarget) {
            Controller.StartFollowState();
            return;
        }

        if (_timer > 0) _timer -= delta;

        _timer = SearchTimer;
        
        UpdateNearestPlayer();
    }

    public override void Exit() {
        base.Exit();

        Blackboard.agent.isStopped = false;
    }
}