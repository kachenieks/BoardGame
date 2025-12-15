using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public string characterName;
    public int steps;
    public long timestamp;
    
    public LeaderboardEntry(string name, string character, int stepCount)
    {
        playerName = name;
        characterName = character;
        steps = stepCount;
        timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}

[Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;
    
    private const string LEADERBOARD_KEY = "LocalLeaderboard";
    private const int MAX_ENTRIES = 100;
    
    private LeaderboardData leaderboardData;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("✅ LeaderboardManager Instance izveidots");
            
            LoadLeaderboard();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void LoadLeaderboard()
    {
        Debug.Log("📊 Ielādē leaderboard...");
        
        if (PlayerPrefs.HasKey(LEADERBOARD_KEY))
        {
            string json = PlayerPrefs.GetString(LEADERBOARD_KEY);
            
            try
            {
                leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);
                Debug.Log($"✅ Ielādēti {leaderboardData.entries.Count} leaderboard ieraksti");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"⚠️ Neizdevās ielādēt leaderboard: {e.Message}");
                leaderboardData = new LeaderboardData();
            }
        }
        else
        {
            Debug.Log("📊 Jauns leaderboard - vēl nav ierakstu");
            leaderboardData = new LeaderboardData();
        }
    }
    
    public void AddEntry(string playerName, string characterName, int steps)
    {
        Debug.Log($"💾 Pievieno leaderboard: {playerName} ({characterName}) - {steps} gājieni");
        
        var entry = new LeaderboardEntry(playerName, characterName, steps);
        leaderboardData.entries.Add(entry);
        
        // Sakārto pēc gājieniem (mazāk ir labāk)
        leaderboardData.entries = leaderboardData.entries
            .OrderBy(e => e.steps)
            .Take(MAX_ENTRIES)
            .ToList();
        
        SaveLeaderboard();
    }
    
    void SaveLeaderboard()
    {
        Debug.Log("💾 Saglabā leaderboard...");
        
        string json = JsonUtility.ToJson(leaderboardData);
        PlayerPrefs.SetString(LEADERBOARD_KEY, json);
        PlayerPrefs.Save();
        
        Debug.Log("✅ Leaderboard saglabāts!");
    }
    
    public List<LeaderboardEntry> GetTopEntries(int count = 10)
    {
        if (leaderboardData == null || leaderboardData.entries == null)
        {
            return new List<LeaderboardEntry>();
        }
        
        return leaderboardData.entries
            .OrderBy(e => e.steps)
            .Take(count)
            .ToList();
    }
    
    public int GetPlayerRank(string playerName)
    {
        if (leaderboardData == null || leaderboardData.entries == null)
        {
            return -1;
        }
        
        var sorted = leaderboardData.entries.OrderBy(e => e.steps).ToList();
        
        for (int i = 0; i < sorted.Count; i++)
        {
            if (sorted[i].playerName == playerName)
            {
                return i + 1;
            }
        }
        
        return -1;
    }
    
    // ✅ PAPILDUS: Metode lai notīrītu leaderboard (debugging)
    public void ClearLeaderboard()
    {
        PlayerPrefs.DeleteKey(LEADERBOARD_KEY);
        leaderboardData = new LeaderboardData();
        Debug.Log("🧹 Leaderboard notīrīts!");
    }
}