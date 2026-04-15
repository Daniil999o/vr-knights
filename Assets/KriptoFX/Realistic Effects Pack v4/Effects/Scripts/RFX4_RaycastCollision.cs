using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RFX4_RaycastCollision : MonoBehaviour
{
    public float RaycastDistance = 100;
    public GameObject[] Effects;
    public float Offset = 0;
    public float EnableTimeDelay = 0;

    public float DestroyTime = 3;
    public bool UsePivotPosition;
    public bool UseNormalRotation = true;
    public bool IsWorldSpace = true;
    public bool RealTimeUpdateRaycast;
    public bool DestroyAfterDisabling;
    [HideInInspector]
    public float HUE = -1;
    [HideInInspector]
    public List<GameObject> CollidedInstances = new List<GameObject>();

    const string particlesAdditionalName = "Distance";
    ParticleSystem[] distanceParticles;
    
    public Action<Transform> OnCollisionAction;

    private bool canUpdate;

    public event EventHandler<RFX4_PhysicsMotion.RFX4_CollisionInfo> CollisionEnter;

    [HideInInspector] public bool autoAim;
    [HideInInspector] public float maxAutoAimDistance;

    void Awake()
    {
        distanceParticles = transform.root.GetComponentsInChildren<ParticleSystem>();
    }

    // Use this for initialization
    void OnEnable()
    {
        CollidedInstances.Clear();
        if (EnableTimeDelay > 0.001) Invoke("UpdateRaycast", EnableTimeDelay);
        else UpdateRaycast();
    }

    void OnDisable()
    {
        if (DestroyAfterDisabling)
        {
            foreach (var instance in CollidedInstances)
            {
                Destroy(instance);
            }
        }
    }

    void Update()
    {
        if (canUpdate) {
            UpdateRaycast();
        }
    }


    private void UpdateRaycast()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, RaycastDistance)) {
            Vector3 position;
            if (UsePivotPosition)
                position = raycastHit.transform.position;
            else
                position = raycastHit.point + raycastHit.normal * Offset;

            var handler = CollisionEnter;
            if (handler != null)
                handler(this, new RFX4_PhysicsMotion.RFX4_CollisionInfo { HitPoint = raycastHit.point, HitCollider = raycastHit.collider, HitGameObject = raycastHit.transform.gameObject});

            if (distanceParticles !=null)
            foreach (var rayPS in distanceParticles)
            {

                    if (rayPS != null && rayPS.name.Contains(particlesAdditionalName))
                    rayPS.GetComponent<ParticleSystemRenderer>().lengthScale = (transform.position - raycastHit.point).magnitude / rayPS.main.startSize.constantMax;

            }

            if (CollidedInstances.Count==0)
                foreach (var effect in Effects) {
                    if (effect != null)
                    {
                        var instance = Instantiate(effect, position, new Quaternion()) as GameObject;
                        var effectSettings = instance.GetComponent<RFX4_EffectSettings>();
                        var effectSettingsRoot = GetComponentInParent<RFX4_EffectSettings>();
                        if (effectSettings != null && effectSettingsRoot != null)
                        {
                            //effectSettings.EffectQuality = effectSettingsRoot.EffectQuality;
                            // effectSettings.ForceInitialize();
                        }

                        CollidedInstances.Add(instance);

                        if (HUE > -0.9f) RFX4_ColorHelper.ChangeObjectColorByHUE(instance, HUE);

                        if (autoAim) {
                            float instanceY = instance.transform.position.y;
                            Vector3 pos = instance.transform.position;

                            // LiveInfo nearest = GetNearLiveInfo(pos);
                            // if (nearest) pos = nearest.transform.position;

                            pos.y = instanceY;
                            instance.transform.position = pos;
                        }

                        OnCollisionAction.Invoke(instance.transform);

                        if (!IsWorldSpace)
                            instance.transform.parent = transform;
                        if (UseNormalRotation)
                            instance.transform.LookAt(raycastHit.point + raycastHit.normal);
                        if (DestroyTime > 0.0001f)
                            Destroy(instance, DestroyTime);
                    }
                }
            else
                foreach (var instance in CollidedInstances) {
                    if (instance == null) continue;
                    instance.transform.position = position;
                    if (UseNormalRotation)
                        instance.transform.LookAt(raycastHit.point + raycastHit.normal);
                }
        }
        if (RealTimeUpdateRaycast)
            canUpdate = true;
    }

    private ILivable GetNearLiveInfo(Vector3 point) {
        // List<ILivable> liveInfos = FindObjectsByType<ILivable>(FindObjectsSortMode.None)
        //     .Where(x => x is MonoBehaviour mb && Vector3.Distance(mb.transform.position, point) < maxAutoAimDistance).ToList();
        //
        // if (liveInfos.Count > 0)
        //     return liveInfos.OrderBy(x => Vector3.Distance(x.transform.position, point)).ToArray()[0];

        return null;
    }


    void OnDrawGizmosSelected()
    {
       Gizmos.color = Color.blue;
       Gizmos.DrawLine(transform.position, transform.position + transform.forward * RaycastDistance);
    }
}
