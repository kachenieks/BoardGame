using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MainMenuLeaderboard : MonoBehaviour
{
    [Header("Leaderboard UI")]
    public GameObject leaderboardPanel;
    public Transform leaderboardContent;
    public GameObject leaderboardEntryPrefab;
    public Button closeButton;
    
    void Start()
    {
        // Sākumā slēpj leaderboard paneli
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
        
        // Pievieno close button funkcionalitāti
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideLeaderboard);
        }
    }
    
    public void ShowLeaderboard()
    {
        Debug.Log("📊 Atver leaderboard no Main Menu");
        
        if (leaderboardPanel == null)
        {
            Debug.LogError("❌ Leaderboard Panel nav piešķirts!");
            return;
        }
        
        if (LeaderboardManager.Instance == null)
        {
            Debug.LogError("❌ LeaderboardManager.Instance nav atrasts!");
            return;
        }
        
        // Parāda paneli
        leaderboardPanel.SetActive(true);
        
        // Notīra veco saturu
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }
        
        // Iegūst TOP 10 rezultātus
        var topEntries = LeaderboardManager.Instance.GetTopEntries(10);
        
        Debug.Log($"📊 Atrasti {topEntries.Count} leaderboard ieraksti");
        
        if (topEntries.Count == 0)
        {
            // Ja nav ierakstu, parādi placeholder tekstu
            CreatePlaceholderText("Nav rezultātu. Spēlē lai iegūtu pirmo vietu!");
            return;
        }
        
        // Izveido entry katram rezultātam
        for (int i = 0; i < topEntries.Count; i++)
        {
            var entry = topEntries[i];
            CreateLeaderboardEntry(i + 1, entry);
        }
    }
    
    void CreateLeaderboardEntry(int rank, LeaderboardEntry entry)
    {
        if (leaderboardEntryPrefab == null)
        {
            Debug.LogError("❌ Leaderboard Entry Prefab nav piešķirts!");
            return;
        }
        
        GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContent);
        
        // Atrod text komponentus
        TextMeshProUGUI rankTxt = entryObj.transform.Find("RankText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI nameTxt = entryObj.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI stepsTxt = entryObj.transform.Find("StepsText")?.GetComponent<TextMeshProUGUI>();
        
        // Ja nav atrasts ar Find, mēģina atrast pirmajā līmenī
        if (rankTxt == null) rankTxt = entryObj.GetComponentInChildren<TextMeshProUGUI>();
        
        // Atjauno tekstus
        if (rankTxt != null) 
        {
            rankTxt.text = $"#{rank}";
            
            // Īpaša krāsa TOP 3
            if (rank == 1) rankTxt.color = new Color(1f, 0.84f, 0f); // Zelts
            else if (rank == 2) rankTxt.color = new Color(0.75f, 0.75f, 0.75f); // Sudrabs
            else if (rank == 3) rankTxt.color = new Color(0.8f, 0.5f, 0.2f); // Bronza
        }
        
        if (nameTxt != null) nameTxt.text = entry.playerName;
        if (stepsTxt != null) stepsTxt.text = $"{entry.steps} gājieni";
    }
    
    void CreatePlaceholderText(string message)
    {
        GameObject placeholder = new GameObject("PlaceholderText");
        placeholder.transform.SetParent(leaderboardContent);
        
        TextMeshProUGUI text = placeholder.AddComponent<TextMeshProUGUI>();
        text.text = message;
        text.fontSize = 28;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.8f, 0.8f, 0.8f);
        
        RectTransform rt = placeholder.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(600, 100);
    }
    
    public void HideLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
            Debug.Log("📊 Leaderboard aizvērts");
        }
    }
}