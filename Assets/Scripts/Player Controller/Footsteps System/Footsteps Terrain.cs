using UnityEngine;
using System.Collections.Generic;

namespace Player_System.Movement.FootstepsSystem
{
    [AddComponentMenu("Player System/Footsteps System/Footsteps Terrain")]
    [RequireComponent(typeof(Terrain))]
    public class FootstepsTerrain : MonoBehaviour
    {
        [SerializeField] private TerrainLayersGroup terrainLayerGroup;
        [SerializeField] private bool autoSetup;
        
        private Terrain _terrain;
        private TerrainData _terrainData;
        private float[,,] _alphamaps;
        private bool _initialized;

        private void Awake() {
            Initialize();
        }

        private void OnValidate() {
            if (autoSetup) {
                autoSetup = false;

                if (TryGetComponent(out Terrain terrain)) {
                    TerrainLayer[] layers = terrain.terrainData.terrainLayers;

                    terrainLayerGroup.layersNames = new List<TerrainLayersGroup.TerrainLayerName>();
                    
                    for (int i = 0; i < layers.Length; i++)
                        terrainLayerGroup.AddLayerName(i, layers[i]);
                }
            }
        }
        
        private void Initialize() {
            _terrain = GetComponent<Terrain>();
            _terrainData = _terrain.terrainData;

            _alphamaps = _terrainData.GetAlphamaps(0, 0, _terrainData.alphamapWidth, _terrainData.alphamapHeight);
            _initialized = true;
        }
        
        public int GetLayerIndex(Vector3 worldPos) {
            if (!_initialized) Initialize();

            Vector3 terrainPos = worldPos - _terrain.transform.position;

            if (terrainPos.x < 0 || terrainPos.z < 0 ||
                terrainPos.x > _terrainData.size.x || terrainPos.z > _terrainData.size.z) {
                return -1;
            }

            float x = Mathf.Clamp01(terrainPos.x / _terrainData.size.x);
            float z = Mathf.Clamp01(terrainPos.z / _terrainData.size.z);

            int mapX = Mathf.RoundToInt(x * (_terrainData.alphamapWidth - 1));
            int mapZ = Mathf.RoundToInt(z * (_terrainData.alphamapHeight - 1));

            int maxIndex = 0;
            float maxVal = 0f;

            for (int i = 0; i < _terrainData.alphamapLayers; i++) {
                float val = _alphamaps[mapZ, mapX, i];
                if (val > maxVal) {
                    maxVal = val;
                    maxIndex = i;
                }
            }

            if (maxIndex >= 0 && terrainLayerGroup && maxIndex < terrainLayerGroup.layersNames.Count)
                return terrainLayerGroup.layersNames[maxIndex].index;
            
            return maxIndex;
        }
    }
}