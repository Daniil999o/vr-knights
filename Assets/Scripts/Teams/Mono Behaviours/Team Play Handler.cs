using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TeamPlayHandler : MonoBehaviour
{
    private static TeamPlayHandler _instance;

    internal static TeamPlayHandler Instance {
        get {
            if (!_instance) _instance = FindFirstObjectByType<TeamPlayHandler>(FindObjectsInactive.Include);
            return _instance;
        }
    }
    
    private HashSet<TeamPlayer> _players = new HashSet<TeamPlayer>();

    public static void RegisterPlayer(TeamPlayer player) {
        if (!Instance) return;
        
        Instance._players.Add(player);
    }

    public static void UnregisterPlayer(TeamPlayer player) {
        if (!Instance) return;
        
        Instance._players.Remove(player);
    }

    public TeamPlayer GetNearestAnotherTeamPlayer(TeamPlayer me) {
        TeamPlayer[] players = _players.Where(x => !x.IsDead && x.MyTeam != me.MyTeam)
            .OrderBy(x => Vector3.Distance(x.transform.position, me.transform.position)).ToArray();
        
        return players.FirstOrDefault();
    }
}