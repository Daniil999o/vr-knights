using System.Collections.Generic;
using UnityEngine;

namespace Player_System.Movement.FootstepsSystem
{
    [CreateAssetMenu(menuName = "Player System/Movement/Terrain Layers Group", fileName = "Terrain Layers Group")]
    public class TerrainLayersGroup : ScriptableObject
    {
        [SerializeField] internal List<TerrainLayerName> layersNames;

        [System.Serializable]
        internal struct TerrainLayerName
        {
            [SerializeField] internal string name;
            [SerializeField] internal int index;
        }

        internal void AddLayerName(int index, TerrainLayer layer) {
            layersNames.Add(new TerrainLayerName {
                    name = layer.name, index = index
                }
            );
        }
    }
}