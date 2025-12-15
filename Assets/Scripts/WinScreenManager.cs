using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WinScreenManager : MonoBehaviour
{
    public static WinScreenManager Instance;
    
    [Header("Win Screen UI")]
    public GameObject winScreenPanel;
    public TextMeshProUGUI winnerNameText;
    public TextMeshProUGUI stepsText;
    public TextMeshProUGUI rankText;
    
    [Header("Leaderboard UI")]
    public GameObject leaderboardPanel;
    public Transform leaderboardContent;
    public GameObject leaderboardEntryPrefab;
    
    [Header("Buttons")]
    public Button playAgainButton;
    public Button menuButton;
    public Button showLeaderboardButton;
    public Button closeLeaderboardButton;
    
    private int totalSteps = 0;
    private string winnerName = "";
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Sākumā slēpj visus paneļus
        if (winScreenPanel != null) winScreenPanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
        
        // Pievieno button listeners
        if (playAgainButton != null) playAgainButton.onClick.AddListener(PlayAgain);
        if (menuButton != null) menuButton.onClick.AddListener(GoToMenu);
        if (showLeaderboardButton != null) showLeaderboardButton.onClick.AddListener(ShowLeaderboard);
        if (closeLeaderboardButton != null) closeLeaderboardButton.onClick.AddListener(HideLeaderboard);
    }
    
    public void ShowWinScreen(string playerName, int steps)
    {
        winnerName = playerName;
        totalSteps = steps;
        
        Debug.Log($"🎉 WIN SCREEN: {playerName} uzvarēja ar {steps} gājieniem!");
        
        if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(true);
            
            // Atjauno UI
            if (winnerNameText != null)
            {
                winnerNameText.text = $"🏆 {playerName} UZVARĒJA!";
            }
            
            if (stepsText != null)
            {
                stepsText.text = $"Pabeigts {steps} gājienos";
            }
            
            // Saglabā rezultātu leaderboard
            SaveAndShowRank();
        }
    }
    
    void SaveAndShowRank()
    {
        if (LeaderboardManager.Instance != null)
        {
            string characterName = PlayerPrefs.GetString("PlayerName", "Player");
            
            LeaderboardManager.Instance.AddEntry(
                winnerName, 
                characterName, 
                totalSteps
            );
            
            // Atjauno rangu
            int rank = LeaderboardManager.Instance.GetPlayerRank(winnerName);
            
            if (rankText != null && rank > 0)
            {
                rankText.text = $"Tavs rangs: #{rank}";
            }
            else if (rankText != null)
            {
                rankText.text = "Tavs pirmais rezultāts!";
            }
        }
    }
    
    public void ShowLeaderboard()
    {
        if (leaderboardPanel == null || LeaderboardManager.Instance == null)
        {
            Debug.LogWarning("⚠️ Leaderboard panel vai manager nav atrasts!");
            return;
        }
        
        leaderboardPanel.SetActive(true);
        
        // Notīra veco saturu
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }
        
        // Iegūst TOP 10 rezultātus
        var topEntries = LeaderboardManager.Instance.GetTopEntries(10);
        
        Debug.Log($"📊 Rāda {topEntries.Count} leaderboard ierakstus");
        
        if (topEntries.Count == 0)
        {
            // Ja nav ierakstu, parādi placeholder
            GameObject placeholder = new GameObject("Placeholder");
            placeholder.transform.SetParent(leaderboardContent);
            
            TextMeshProUGUI text = placeholder.AddComponent<TextMeshProUGUI>();
            text.text = "Nav rezultātu. Esi pirmais!";
            text.fontSize = 24;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            
            return;
        }
        
        for (int i = 0; i < topEntries.Count; i++)
        {
            var entry = topEntries[i];
            
            GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            
            // Atrod text komponentus
            TextMeshProUGUI rankTxt = entryObj.transform.Find("RankText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI nameTxt = entryObj.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI stepsTxt = entryObj.transform.Find("StepsText")?.GetComponent<TextMeshProUGUI>();
            
            if (rankTxt != null) rankTxt.text = $"#{i + 1}";
            if (nameTxt != null) nameTxt.text = entry.playerName;
            if (stepsTxt != null) stepsTxt.text = $"{entry.steps} gājieni";
            
            // Highlight spēlētāja rezultāts
            if (entry.playerName == winnerName)
            {
                Image bg = entryObj.GetComponent<Image>();
                if (bg != null)
                {
                    bg.color = new Color(1f, 0.9f, 0.3f, 0.3f); // Dzeltens highlight
                }
            }
        }
    }
    
    void HideLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
    }
    
    void PlayAgain()
    {
        Debug.Log("🔄 Restartē spēli...");
        
        // Pārlādē spēles scēnu
        SceneManager.LoadScene(1);
    }
    
    void GoToMenu()
    {
        Debug.Log("🏠 Atgriežas uz galveno izvēlni...");
        SceneManager.LoadScene(0);
    }
}