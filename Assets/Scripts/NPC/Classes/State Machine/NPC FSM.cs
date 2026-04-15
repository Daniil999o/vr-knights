public class NPC_FSM
{
    public FSM_State CurrentState { get; private set; }

    public void ChangeState(FSM_State newState) {
        CurrentState?.Exit();
        
        CurrentState = newState;
        CurrentState.Enter();
    }
}