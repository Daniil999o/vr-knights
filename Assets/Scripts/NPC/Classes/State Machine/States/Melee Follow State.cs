using UnityEngine;

public class MeleeFollowState : GroundedState
{
    private enum FollowPattern
    {
        Direct,
        Flank,
        SprintFlank,
        DodgeAndRush
    }

    private FollowPattern _currentPattern;
    private float _patternTimer;
    private float _updateTargetTimer;
    private const float UpdateTargetTimer = 5f;

    private int _flankDirection = 1;
    private bool _flankPhaseTwo;
    private float _phaseTimer;

    private float _dodgeTimer;
    private bool _dodgeDirection;

    public MeleeFollowState(NPC_AI controller) : base(controller) { }

    public override void Enter() {
        base.Enter();

        Blackboard.agent.isStopped = false;
        Blackboard.agent.updateRotation = true;

        if (!Blackboard.HasTarget) {
            Controller.StartSearchState();
            return;
        }

        PickRandomPattern();
        _patternTimer = Random.Range(Data.followPatternChangeInterval * 0.8f, Data.followPatternChangeInterval * 1.2f);
        _updateTargetTimer = 0f;

        float distance = Blackboard.currentTarget.Distance(Transform);
        
        if (distance > Data.playAttackMinMaxDistance.x && distance < Data.playAttackMinMaxDistance.y)
            Blackboard.voiceController.PlayAttack();
    }

    private void PickRandomPattern() {
        float rand = Random.value;
        _currentPattern = rand switch {
            < 0.25f => FollowPattern.Direct, < 0.5f => FollowPattern.Flank, < 0.75f => FollowPattern.SprintFlank,
            var _ => FollowPattern.DodgeAndRush
        };

        if (_currentPattern == FollowPattern.DodgeAndRush
            && Blackboard.currentTarget.Distance(Transform) < Data.minDistanceToStrafes)
            _currentPattern = FollowPattern.Flank;

        switch (_currentPattern) {
            case FollowPattern.Flank or FollowPattern.SprintFlank:
                _flankDirection = Random.value > 0.5f ? 1 : -1;
                _flankPhaseTwo = false;
                _phaseTimer = 0f;
                break;
            case FollowPattern.DodgeAndRush:
                _dodgeTimer = 0f;
                _dodgeDirection = Random.value > 0.5f;
                break;
        }
    }

    public override void Update(float delta) {
        base.Update(delta);

        if (!Blackboard.HasTarget) {
            Controller.StartSearchState();
            return;
        }

        if (_updateTargetTimer > 0f)
            _updateTargetTimer -= delta;
        else {
            _updateTargetTimer = UpdateTargetTimer;
            UpdateNearestPlayer();
        }

        float targetDistance = Blackboard.currentTarget.Distance(Transform);

        _patternTimer -= delta;
        if (_patternTimer <= 0f) {
            PickRandomPattern();
            _patternTimer = Random.Range(Data.followPatternChangeInterval * 0.8f,
                Data.followPatternChangeInterval * 1.2f
            );
        }

        switch (_currentPattern) {
            case FollowPattern.Direct:
                DirectUpdate(delta, targetDistance);
                break;
            case FollowPattern.Flank:
                FlankUpdate(delta, targetDistance);
                break;
            case FollowPattern.SprintFlank:
                SprintFlankUpdate(delta, targetDistance);
                break;
            case FollowPattern.DodgeAndRush:
                DodgeAndRushUpdate(delta, targetDistance);
                break;
        }

        if (targetDistance < Data.distanceToAttack) Controller.StartAttackState();
    }

    private void DirectUpdate(float delta, float targetDistance) {
        bool distanceIsHigh = targetDistance > Data.distanceToRun;
        float newSpeed = distanceIsHigh ? Data.runSpeed : Data.walkSpeed;
        float newAngular = distanceIsHigh ? Data.runAngular : Data.walkAngular;

        Blackboard.agent.speed = Mathf.Lerp(Blackboard.agent.speed, newSpeed, Data.speedSmooth * delta);
        Blackboard.agent.angularSpeed =
            Mathf.Lerp(Blackboard.agent.angularSpeed, newAngular, Data.angularSmooth * delta);

        Blackboard.agent.SetDestination(Blackboard.currentTarget.Pos);
    }

    private void FlankUpdate(float delta, float targetDistance) {
        Vector3 targetPos = Blackboard.currentTarget.Pos;
        Vector3 toTarget = targetPos - Transform.position;
        toTarget.y = 0;
        float currentDist = toTarget.magnitude;

        if (!_flankPhaseTwo && currentDist > Data.distanceToAttack + 1f) {
            Vector3 tangent = Vector3.Cross(Vector3.up, toTarget.normalized).normalized;
            Vector3 desiredPos = targetPos - toTarget.normalized * Data.flankCircleRadius
                                 + tangent * _flankDirection * Data.flankCircleRadius * 0.5f;

            Blackboard.agent.speed = Mathf.Lerp(Blackboard.agent.speed, Data.flankSpeed, Data.speedSmooth * delta);
            Blackboard.agent.angularSpeed =
                Mathf.Lerp(Blackboard.agent.angularSpeed, Data.walkAngular, Data.angularSmooth * delta);
            Blackboard.agent.SetDestination(desiredPos);

            float angle = Vector3.Angle(toTarget, targetPos - Transform.position);
            _phaseTimer += delta;
            if (angle > 45f || _phaseTimer > 2.5f) _flankPhaseTwo = true;
        } else {
            float speed = targetDistance > Data.distanceToRun ? Data.runSpeed : Data.lungeSpeed;
            Blackboard.agent.speed = Mathf.Lerp(Blackboard.agent.speed, speed, Data.speedSmooth * delta);
            Blackboard.agent.angularSpeed =
                Mathf.Lerp(Blackboard.agent.angularSpeed, Data.runAngular, Data.angularSmooth * delta);
            Blackboard.agent.SetDestination(targetPos);
        }
    }

    private void SprintFlankUpdate(float delta, float targetDistance) {
        Vector3 targetPos = Blackboard.currentTarget.Pos;
        Vector3 toTarget = targetPos - Transform.position;
        toTarget.y = 0;
        float currentDist = toTarget.magnitude;

        if (!_flankPhaseTwo && currentDist > Data.distanceToAttack + 2f) {
            Vector3 tangent = Vector3.Cross(Vector3.up, toTarget.normalized).normalized;
            Vector3 behindTarget = targetPos - toTarget.normalized * Data.sprintFlankRadius
                                   + tangent * _flankDirection * Data.sprintFlankRadius * 0.7f;

            Blackboard.agent.speed =
                Mathf.Lerp(Blackboard.agent.speed, Data.sprintFlankSpeed, Data.speedSmooth * delta);
            Blackboard.agent.angularSpeed =
                Mathf.Lerp(Blackboard.agent.angularSpeed, Data.runAngular, Data.angularSmooth * delta);
            Blackboard.agent.SetDestination(behindTarget);

            Vector3 dirFromTarget = Transform.position - targetPos;
            float dot = Vector3.Dot(toTarget.normalized, dirFromTarget.normalized);
            _phaseTimer += delta;
            if (dot < -0.3f || _phaseTimer > 3f)
                _flankPhaseTwo = true;
        } else {
            Blackboard.agent.speed = Mathf.Lerp(Blackboard.agent.speed, Data.runSpeed, Data.speedSmooth * delta);
            Blackboard.agent.angularSpeed =
                Mathf.Lerp(Blackboard.agent.angularSpeed, Data.runAngular, Data.angularSmooth * delta);
            Blackboard.agent.SetDestination(targetPos);
        }
    }

    private void DodgeAndRushUpdate(float delta, float targetDistance) {
        Vector3 targetPos = Blackboard.currentTarget.Pos;
        Vector3 toTarget = targetPos - Transform.position;
        toTarget.y = 0;
        Vector3 right = Vector3.Cross(Vector3.up, toTarget.normalized).normalized;

        _dodgeTimer += delta;

        if (_dodgeTimer < 1.5f) {
            float dodgeOffset = Mathf.Sin(_dodgeTimer * Data.dodgeStrafeSpeed) * Data.dodgeStrafeDistance;
            Vector3 dodgeTarget = targetPos - toTarget.normalized * Data.distanceToAttack
                                  + right * (_dodgeDirection ? dodgeOffset : -dodgeOffset);

            Blackboard.agent.speed =
                Mathf.Lerp(Blackboard.agent.speed, Data.dodgeStrafeSpeed, Data.speedSmooth * delta);
            Blackboard.agent.angularSpeed =
                Mathf.Lerp(Blackboard.agent.angularSpeed, Data.walkAngular, Data.angularSmooth * delta);
            Blackboard.agent.SetDestination(dodgeTarget);
        } else {
            float speed = targetDistance > Data.distanceToRun ? Data.runSpeed : Data.lungeSpeed;
            Blackboard.agent.speed = Mathf.Lerp(Blackboard.agent.speed, speed, Data.speedSmooth * delta);
            Blackboard.agent.angularSpeed =
                Mathf.Lerp(Blackboard.agent.angularSpeed, Data.runAngular, Data.angularSmooth * delta);
            Blackboard.agent.SetDestination(targetPos);

            if (targetDistance < Data.distanceToAttack + 1f) _dodgeTimer = 0f;
        }
    }

    public override void Exit() {
        base.Exit();
    }
}