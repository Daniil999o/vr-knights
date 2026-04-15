using UnityEngine;

[System.Serializable]
public class YG_IndexedMaterial
{
    [SerializeField] private Renderer renderer;
    [SerializeField] private int materialIndex;

    private Material _myMaterial;

    public Material StaticMaterial {
        get => renderer.materials[materialIndex];
        set {
            Material[] mats = renderer.materials;
            mats[materialIndex] = value;
            
            renderer.materials = mats;
        }
    }
    
    public Material RuntimeMaterial {
        get {
            if (!_myMaterial) {
                Material[] mats = renderer.materials;
                mats[materialIndex] = _myMaterial = new Material(mats[materialIndex]);
                renderer.materials = mats;
            }

            return _myMaterial;
        }
        set {
            Material[] mats = renderer.materials;
            mats[materialIndex] = _myMaterial = value;
            renderer.materials = mats;
        }
    }
}