using UnityEngine;

public class MeleeAttackState : GroundedState
{
    private enum MovementPattern
    {
        StandGround,
        Strafing,
        Sidestep,
        PingPong
    }

    private MovementPattern _currentPattern;
    private int _strafeDirection = 1;
    private float _strafeTimer;

    private float _phaseTimer;
    private bool _phaseTwoActive;

    private Vector3 _sidestepTarget;
    private Vector3 _pingPongBackPos;
    private Vector3 _pingPongLungeTarget;
    private bool _pingPongBackstepDone;

    public MeleeAttackState(NPC_AI controller) : base(controller) { }

    public override void Enter() {
        base.Enter();

        if (Random.Range(0f, 1f) < Data.blockChanceAfterAttack) {
            Controller.StartBlockState();
            return;
        }

        Blackboard.agent.isStopped = false;
        Blackboard.agent.updateRotation = false;
        Blackboard.agent.autoBraking = false;
        Blackboard.agent.acceleration = 200f;
        Blackboard.agent.angularSpeed = 0f;

        PickRandomPattern();

        Blackboard.animator.SetFloat(Blackboard.hashNames.AttackIndex, Random.Range(0, Data.meleeAttacksCount + 1));
        Blackboard.animator.SetTrigger(Blackboard.hashNames.Attack);
        
        Blackboard.voiceController.PlayFight();
    }

    private void PickRandomPattern() {
        float rand = Random.value;
        _currentPattern = rand switch {
            < 0.25f => MovementPattern.StandGround, < 0.5f => MovementPattern.Strafing,
            < 0.75f => MovementPattern.Sidestep, var _ => MovementPattern.PingPong
        };

        switch (_currentPattern) {
            case MovementPattern.Strafing:
                _strafeDirection = Random.value > 0.5f ? 1 : -1;
                _strafeTimer = 0f;
                break;
            case MovementPattern.Sidestep:
                _strafeDirection = Random.value > 0.5f ? 1 : -1;
                _phaseTimer = 0f;
                _phaseTwoActive = false;
                break;
            case MovementPattern.PingPong:
                _phaseTimer = 0f;
                _pingPongBackstepDone = false;
                break;
        }
    }

    public override void Update(float delta) {
        base.Update(delta);

        if (!Blackboard.HasTarget) {
            Controller.StartSearchState();
            return;
        }

        Vector3 dirToTarget = Blackboard.currentTarget.Pos - Transform.position;
        dirToTarget.y = 0;
        
        if (dirToTarget != Vector3.zero) {
            Quaternion lookRot = Quaternion.LookRotation(dirToTarget);
            Transform.rotation = Quaternion.Lerp(Transform.rotation, lookRot, Data.rotateToAttackSpeed * delta);
        }

        switch (_currentPattern) {
            case MovementPattern.StandGround:
                StandGroundUpdate();
                break;
            case MovementPattern.Strafing:
                StrafingUpdate(delta);
                break;
            case MovementPattern.Sidestep:
                SidestepUpdate(delta);
                break;
            case MovementPattern.PingPong:
                PingPongUpdate(delta);
                break;
        }
    }

    private void StandGroundUpdate() {
        Blackboard.agent.isStopped = true;
    }

    private void StrafingUpdate(float delta) {
        Blackboard.agent.speed = Data.strafeSpeed;

        Vector3 toTarget = Blackboard.currentTarget.Pos - Transform.position;
        toTarget.y = 0;
        Vector3 right = Vector3.Cross(Vector3.up, toTarget.normalized).normalized;

        Vector3 basePos = Blackboard.currentTarget.Pos - toTarget.normalized * Data.distanceToAttack;

        _strafeTimer += delta * Data.strafeSpeed * 0.5f;
        float desiredOffset = Mathf.Sin(_strafeTimer) * Data.strafeDistance * _strafeDirection;

        Vector3 desiredPos = basePos + right * desiredOffset;
        Blackboard.agent.SetDestination(desiredPos);
    }

    private void SidestepUpdate(float delta) {
        Blackboard.agent.isStopped = false;

        Vector3 toTarget = Blackboard.currentTarget.Pos - Transform.position;
        toTarget.y = 0;
        Vector3 right = Vector3.Cross(Vector3.up, toTarget.normalized).normalized;

        if (!_phaseTwoActive) {
            _phaseTimer += delta;
            Blackboard.agent.speed = Data.lungeSpeed;

            if (_phaseTimer <= Data.sidestepPhase1Time) {
                _sidestepTarget = Transform.position + right * _strafeDirection * Data.strafeDistance * 1.5f;
                Blackboard.agent.SetDestination(_sidestepTarget);
            } else {
                _phaseTwoActive = true;
            }
        } else {
            Blackboard.agent.speed = Data.backstepSpeed;
            Vector3 returnPos = Blackboard.currentTarget.Pos - toTarget.normalized * Data.distanceToAttack;
            Blackboard.agent.SetDestination(returnPos);

            if (Vector3.Distance(Transform.position, returnPos) < 0.3f) Blackboard.agent.isStopped = true;
        }
    }

    private void PingPongUpdate(float delta) {
        Blackboard.agent.isStopped = false;
        Vector3 toTarget = Blackboard.currentTarget.Pos - Transform.position;
        toTarget.y = 0;

        if (!_pingPongBackstepDone) {
            _phaseTimer += delta;
            Blackboard.agent.speed = Data.backstepSpeed;

            if (_pingPongBackPos == Vector3.zero)
                _pingPongBackPos = Transform.position - toTarget.normalized * Data.backstepDistance;

            Blackboard.agent.SetDestination(_pingPongBackPos);

            if (_phaseTimer < Data.pingPongBackstepTime)
                return;
            
            _pingPongBackstepDone = true;
            _pingPongLungeTarget = Blackboard.currentTarget.Pos;
        } else {
            Blackboard.agent.speed = Data.lungeSpeed;
            Blackboard.agent.SetDestination(_pingPongLungeTarget);
        }
    }

    public override void Exit() {
        base.Exit();

        Blackboard.agent.isStopped = false;
        Blackboard.agent.updateRotation = true;
        Blackboard.agent.speed = Data.walkSpeed;
        Blackboard.agent.acceleration = Data.acceleration;
        Blackboard.agent.angularSpeed = Data.walkAngular;
        Blackboard.agent.autoBraking = Data.autoBraking;
    }
}