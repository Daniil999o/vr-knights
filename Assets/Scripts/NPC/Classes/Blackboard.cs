using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class NPC_Blackboard
{
    [field: SerializeField] public TeamPlayer TeamPlayer { get; private set; }
    
    [Space]
    
    public NavMeshAgent agent;
    
    public float RealSpeed => agent.velocity.magnitude;

    [Space]

    public Animator animator;
    public AnimatorHashNames hashNames;
    
    [Space]
    
    public CharacterVoiceController voiceController;

    [Space]

    [Header("Runtime")]
    public NPC_Target currentTarget;
    
    public bool HasTarget => currentTarget is { Active: true };

    [System.Serializable]
    public class AnimatorHashNames
    {
        [SerializeField] private string SpeedId = "Speed";
        
        [Space]
        
        [SerializeField] private string AttackId = "Attack";
        [SerializeField] private string AttackIndexId = "Attack Index";
        [SerializeField] private string BlockId = "Block";

        public int Speed { get; set; }
        
        public int Attack { get; set; }
        public int AttackIndex { get; set; }
        public int Block { get; set; }
        
        public void UpdateHashes() {
            Speed = Animator.StringToHash(SpeedId);

            Attack = Animator.StringToHash(AttackId);
            AttackIndex = Animator.StringToHash(AttackIndexId);
            Block = Animator.StringToHash(BlockId);
        }
    }
}

[System.Serializable]
public class NPC_Target
{
    public TeamPlayer player;
    private Transform _transform;

    public bool Active => player && !player.IsDead;

    public Vector3 Pos => _transform.position;
        
    public NPC_Target(TeamPlayer p) {
        player = p;
        _transform = player.transform;
    }

    public float Distance(Transform t) => Vector3.Distance(t.position, Pos);
}