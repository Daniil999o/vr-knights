using UnityEngine;

public abstract class FSM_State
{
    protected NPC_AI Controller { get; set; }

    protected NPC_Settings Data => Controller.Data;
    protected NPC_Blackboard Blackboard => Controller.Blackboard;

    
    protected Transform Transform { get; set; }

    public FSM_State(NPC_AI controller) {
        Controller = controller;
        Transform = Controller.transform;
    }
    
    public virtual void Enter() {
        
    }
    
    public virtual void Update(float delta) {
        
    }

    public virtual void UpdateAnimator() {
        
    }

    public virtual void Exit() {
        
    }
}