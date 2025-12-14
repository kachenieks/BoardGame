using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LadderConnection
{
    public int fromTile;  // No kura tile
    public int toTile;    // Uz kuru tile
    public bool isLadder; // true = kāpnes uz augšu, false = čūska uz leju
}

public class LadderSystem : MonoBehaviour
{
    [Header("Kāpņu un čūsku savienojumi")]
    [Tooltip("Uzstādi tile numurus Inspector")]
    public List<LadderConnection> connections = new List<LadderConnection>();

    private static LadderSystem instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Automātiski izveido savienojumus, ja nav uzstādīti
        if (connections.Count == 0)
        {
            SetupDefaultLadders();
        }

        LogConnections();
    }

    void SetupDefaultLadders()
    {
        // 6x5 = 30 tiles layout
        // 2 kāpnes uz augšu, 2 čūskas uz leju

        connections.Add(new LadderConnection 
        { 
            fromTile = 3,   // Tile 3
            toTile = 14,    // -> uz Tile 14 (kāpnes)
            isLadder = true 
        });

        connections.Add(new LadderConnection 
        { 
            fromTile = 8,   // Tile 8
            toTile = 21,    // -> uz Tile 21 (kāpnes)
            isLadder = true 
        });

        connections.Add(new LadderConnection 
        { 
            fromTile = 17,  // Tile 17
            toTile = 6,     // -> uz Tile 6 (čūska)
            isLadder = false 
        });

        connections.Add(new LadderConnection 
        { 
            fromTile = 24,  // Tile 24
            toTile = 11,    // -> uz Tile 11 (čūska)
            isLadder = false 
        });

        Debug.Log("✅ Uzstādīti 4 savienojumi (2 kāpnes, 2 čūskas)");
    }

    public static int GetDestinationTile(int currentTile)
    {
        if (instance == null) return currentTile;

        foreach (var connection in instance.connections)
        {
            if (connection.fromTile == currentTile)
            {
                string type = connection.isLadder ? "🪜 KĀPNES" : "🐍 ČŪSKA";
                Debug.Log($"{type}: Tile {currentTile} -> Tile {connection.toTile}");
                return connection.toTile;
            }
        }

        return currentTile; // Nav savienojuma
    }

    void LogConnections()
    {
        Debug.Log("=== KĀPNES UN ČŪSKAS ===");
        foreach (var conn in connections)
        {
            string emoji = conn.isLadder ? "🪜" : "🐍";
            string type = conn.isLadder ? "KĀPNES" : "ČŪSKA";
            Debug.Log($"{emoji} {type}: Tile {conn.fromTile} → Tile {conn.toTile}");
        }
    }
}