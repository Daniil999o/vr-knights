using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameInfo : MonoBehaviour
{
    [SerializeField] private TeamPlayer[] npcPrefabs;
    
    [Space]
    
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private TeamDataSO[] teams;
    
    private void Update() {
        if (Keyboard.current.f5Key.wasPressedThisFrame)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            SpawnNpc(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            SpawnNpc(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            SpawnNpc(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            SpawnNpc(3);
    }

    private void SpawnNpc(int type) {
        Vector3 pos = spawnPoints[type].position;

        Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Length)], pos, Quaternion.identity).SetTeam(teams[type]);
    }
}