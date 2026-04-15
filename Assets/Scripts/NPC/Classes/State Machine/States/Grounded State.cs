public abstract class GroundedState : FSM_State
{
    public GroundedState(NPC_AI controller) : base(controller) { }

    public override void Enter() {
        base.Enter();
    }

    public override void Update(float delta) {
        base.Update(delta);
    }

    public override void UpdateAnimator() {
        base.UpdateAnimator();
        
        Blackboard.animator.SetFloat(Blackboard.hashNames.Speed, Blackboard.RealSpeed);
    }

    public override void Exit() {
        base.Exit();
    }
    
    protected void UpdateNearestPlayer() {
        TeamPlayer p = TeamPlayHandler.Instance.GetNearestAnotherTeamPlayer(Blackboard.TeamPlayer);
        
        if (!p) {
            Blackboard.currentTarget = null;
            return;
        }

        Blackboard.currentTarget = new NPC_Target(p);
    }
}