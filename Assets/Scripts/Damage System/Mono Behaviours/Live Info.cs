using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Sirenix.OdinInspector;
using FIMSpace.FProceduralAnimation;

public class LiveInfo : MonoBehaviour, ILivable
{
    [Header("Team Player")]
    [SerializeField] private TeamPlayer teamPlayer;
    
    [Space]
    
    [Header("Health")]
    [SerializeField] private bool immortal;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private bool died;

    [Space]

    [Header("Body Parts")]
    [SerializeField] private bool setKinematicOnStart;
    [SerializeField] private bool autoSetup;
    [SerializeField] private BodyPart[] bodyParts;

    [Space]

    [Header("Events")]
    [SerializeField] private float minHitToInvoke = 10f;
    [SerializeField] private UnityEvent onHitEvent;
    [SerializeField] private UnityEvent onDieEvent;

    [Space]

    [Header("Animator")]
    [SerializeField] private bool disableRagdollOnStart;
    [SerializeField] private RagdollAnimator2 ragdollAnimator2;
    [SerializeField] private Animator animator;

    [Space]

    [Header("Components")]
    [SerializeField] private NPC_AI npcAI;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Behaviour[] componentsToDisableAfterDie;
    [SerializeField] private GameObject[] objectsToDestroyAfterDie;

    [Button("Kill")]
    private void Kill() => CurrentHealth = 0f;
    
    private void OnValidate() {
        if (!autoSetup) return;
        
        bodyParts = GetComponentsInChildren<BodyPart>();
    }

    public float CurrentHealth {
        get => currentHealth;
        set {
            currentHealth = Mathf.Clamp(value, 0f, maxHealth);
            UpdateHealth();
        }
    }

    private bool BodyPartsKinematic {
        set {
            foreach (var x in bodyParts) x.IsKinematic = value;
        }
    }

    private void Start() {
        if (setKinematicOnStart) BodyPartsKinematic = true;
        
        if (disableRagdollOnStart && ragdollAnimator2)
            ragdollAnimator2.Handler.AnimatingMode = RagdollHandler.EAnimatingMode.Off;
    }
    
    public void ApplyDamage(DamageInfo damageInfo) {
        float damage = damageInfo.DamageAmount;
        
        if (immortal)
            damage = 0f;
        
        CurrentHealth -= damage;
        
        if (damage > minHitToInvoke) onHitEvent?.Invoke();
    }

    private void UpdateHealth() {
        if (CurrentHealth <= 0f)
            Die();
    }

    private void Die() {
        if (died) return;

        teamPlayer.IsDead = true;

        died = true;

        if (npcAI) {
            npcAI.Blackboard.agent.enabled = false;
            npcAI.enabled = false;
        }

        if (characterController)
            characterController.enabled = false;

        foreach (var x in componentsToDisableAfterDie) x.enabled = false;
        foreach (var x in objectsToDestroyAfterDie) Destroy(x);

        if (animator) animator.enabled = false;
        
        if (ragdollAnimator2) {
            ragdollAnimator2.RagdollBlend = 1f;
            ragdollAnimator2.Handler.AnimatingMode = RagdollHandler.EAnimatingMode.Falling;
            ragdollAnimator2.Handler.MusclesPower = 0.01f;
            ragdollAnimator2.User_UpdateAllBonesParametersAfterManualChanges();
        }
        
        onDieEvent?.Invoke();

        StartCoroutine(IE_FreezeDummy());
    }

    private IEnumerator IE_FreezeDummy() {
        if (!ragdollAnimator2) yield break;
        
        yield return new WaitForSeconds(10f);
        
        ragdollAnimator2.User_FreezeAndDestroyRagdollDummy();
    }
}