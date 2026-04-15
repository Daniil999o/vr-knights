using System;
using System.Linq;
using UnityEngine;

namespace Player_System.Movement.FootstepsSystem
{
    public class FoleyZonesTerrainCreator : MonoBehaviour
    {
        [SerializeField] private TreeFoleyData[] treeFoleys;

        [System.Serializable]
        private class TreeFoleyData
        {
            [SerializeField] internal string name = "New Foley";
            
            [Space]
            
            [SerializeField] internal int[] treeIndexes;
            
            [Space]
            
            [SerializeField] internal FoleyData foleyData;
            [SerializeField] internal int layerId;

            [Space]

            [SerializeField] internal Vector3 triggerOffset = Vector3.up * 0.5f;
            [SerializeField] internal float triggerRadius = 2f;
        }

        private void Start() {
            foreach (var x in FindObjectsByType<Terrain>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                AddTriggersToTrees(x);
        }

        private void AddTriggersToTrees(Terrain terrain) {
            TreeInstance[] trees = terrain.terrainData.treeInstances;

            for (int i = 0; i < trees.Length; i++) {
                TreeInstance tree = trees[i];

                if (GetFoleyData(tree.prototypeIndex, out TreeFoleyData treeFoley)) {
                    Vector3 treeWorldPos = Vector3.Scale(tree.position, terrain.terrainData.size) +
                                           terrain.transform.position;
                    
                    GameObject triggerObj = new GameObject($"TreeTrigger_{treeFoley.name}_{i}") {
                        isStatic = true,
                        transform = {
                            position = treeWorldPos, parent = transform
                        },
                        layer = treeFoley.layerId
                    };

                    SphereCollider coll = triggerObj.AddComponent<SphereCollider>();
                    
                    coll.isTrigger = true;
                    
                    coll.center = treeFoley.triggerOffset;
                    coll.radius = (float)Math.Round(treeFoley.triggerRadius * tree.widthScale, 2);

                    FoleyObject foley = triggerObj.AddComponent<FoleyObject>();

                    SetFoleyData(foley, treeFoley.foleyData);
                }
            }
        }

        private bool GetFoleyData(int prototypeIndex, out TreeFoleyData treeFoley) {
            treeFoley = null;
            
            foreach (var x in treeFoleys) {
                if (x.treeIndexes.Contains(prototypeIndex)) {
                    treeFoley = x;
                    return true;
                }
            }

            return false;
        }

        private void SetFoleyData(FoleyObject foley, FoleyData data) => foley.foleyData = data;
    }
}