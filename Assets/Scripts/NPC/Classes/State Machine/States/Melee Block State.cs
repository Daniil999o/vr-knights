using UnityEngine;

public class MeleeBlockState : GroundedState
{
    private enum BlockPattern
    {
        StandGround,
        Backstep,
        Strafing
    }

    private BlockPattern _currentPattern;
    private float _blockTimer;
    private int _strafeDirection = 1;
    private float _strafeTimer;
    private Vector3 _backstepTarget;

    public MeleeBlockState(NPC_AI controller) : base(controller) { }

    public override void Enter() {
        base.Enter();

        Blackboard.agent.updateRotation = false;
        Blackboard.agent.isStopped = false;

        Blackboard.animator.SetBool(Blackboard.hashNames.Block, true);

        _blockTimer = Random.Range(Data.minMaxBlockTimer.x, Data.minMaxBlockTimer.y);

        PickRandomPattern();
    }

    private void PickRandomPattern() {
        float rand = Random.value;
        _currentPattern = rand switch {
            < 0.33f => BlockPattern.StandGround, < 0.66f => BlockPattern.Backstep, var _ => BlockPattern.Strafing
        };

        switch (_currentPattern) {
            case BlockPattern.StandGround:
                Blackboard.agent.isStopped = true;
                break;
            case BlockPattern.Backstep:
                _backstepTarget = Vector3.zero;
                break;
            case BlockPattern.Strafing:
                _strafeDirection = Random.value > 0.5f ? 1 : -1;
                _strafeTimer = 0f;
                Blackboard.agent.speed = Data.blockStrafeSpeed;
                break;
        }
    }

    public override void Update(float delta) {
        base.Update(delta);

        _blockTimer -= delta;
        if (_blockTimer <= 0f) {
            Controller.StartFollowState();
            return;
        }

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
            case BlockPattern.StandGround:
                break;
            case BlockPattern.Backstep:
                BackstepUpdate(delta);
                break;
            case BlockPattern.Strafing:
                StrafingUpdate(delta);
                break;
        }
    }

    private void BackstepUpdate(float delta) {
        Vector3 toTarget = Blackboard.currentTarget.Pos - Transform.position;
        toTarget.y = 0;

        if (_backstepTarget == Vector3.zero) {
            _backstepTarget = Transform.position - toTarget.normalized * Data.blockBackstepDistance;
            Blackboard.agent.speed = Data.blockBackstepSpeed;
            Blackboard.agent.SetDestination(_backstepTarget);
        }

        if (Vector3.Distance(Transform.position, _backstepTarget) < 0.3f) {
            Blackboard.agent.isStopped = true;
        } else {
            Vector3 desiredPos = Blackboard.currentTarget.Pos - toTarget.normalized * Data.blockBackstepDistance;
            
            if (!(Vector3.Distance(_backstepTarget, desiredPos) > 0.5f)) return;
            
            _backstepTarget = desiredPos;
            Blackboard.agent.SetDestination(_backstepTarget);
        }
    }

    private void StrafingUpdate(float delta) {
        Blackboard.agent.isStopped = false;
        Blackboard.agent.speed = Data.blockStrafeSpeed;

        Vector3 toTarget = Blackboard.currentTarget.Pos - Transform.position;
        toTarget.y = 0;
        Vector3 right = Vector3.Cross(Vector3.up, toTarget.normalized).normalized;

        Vector3 basePos = Blackboard.currentTarget.Pos - toTarget.normalized * Data.distanceToAttack;

        _strafeTimer += delta;
        if (_strafeTimer >= Data.blockStrafeChangeTime) {
            _strafeTimer = 0f;
            _strafeDirection *= -1;
        }

        float offset = Mathf.Sin(_strafeTimer * Mathf.PI / Data.blockStrafeChangeTime) * Data.blockStrafeDistance *
                       _strafeDirection;
        Vector3 desiredPos = basePos + right * offset;

        Blackboard.agent.SetDestination(desiredPos);
    }

    public override void Exit() {
        base.Exit();

        Blackboard.animator.SetBool(Blackboard.hashNames.Block, false);

        Blackboard.agent.isStopped = false;
        Blackboard.agent.updateRotation = true;
        Blackboard.agent.speed = Data.walkSpeed;
        Blackboard.agent.autoBraking = Data.autoBraking;
    }
}