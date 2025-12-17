using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class GameLeaderboard : MonoBehaviour
{
    [Header("Leaderboard UI")]
    public Transform leaderboardContent;
    public GameObject leaderboardEntryPrefab;
    public Button closeButton;
    
    void Start()
    {
        // Pievieno close button funkcionalitāti
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(HideLeaderboard);
        }
    }
    
    void OnEnable()
    {
        // Izmanto coroutine lai atjaunotu pēc frame delay
        StartCoroutine(RefreshLeaderboardDelayed());
    }
    
    IEnumerator RefreshLeaderboardDelayed()
    {
        // Gaida vienu frame lai viss būtu ready
        yield return null;
        RefreshLeaderboard();
    }
    
    public void RefreshLeaderboard()
    {
        Debug.Log("🔄 Atjauno Leaderboard");
        
        if (leaderboardContent == null)
        {
            Debug.LogError("❌ Leaderboard Content nav piesķirts!");
            return;
        }
        
        if (LeaderboardManager.Instance == null)
        {
            Debug.LogError("❌ LeaderboardManager.Instance nav atrasts!");
            CreatePlaceholderText("Nav leaderboard managera!");
            return;
        }
        
        // Notīra veco saturu (ar realtime unscaled)
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in leaderboardContent)
        {
            toDestroy.Add(child.gameObject);
        }
        
        foreach (GameObject obj in toDestroy)
        {
            DestroyImmediate(obj);
        }
        
        // Iegūst TOP 10 rezultātus
        var topEntries = LeaderboardManager.Instance.GetTopEntries(10);
        
        Debug.Log($"📊 Atrasti {topEntries.Count} leaderboard ieraksti");
        
        if (topEntries.Count == 0)
        {
            CreatePlaceholderText("Nav rezultātu vēl!");
            return;
        }
        
        // Izveido entry katram rezultātam
        for (int i = 0; i < topEntries.Count; i++)
        {
            var entry = topEntries[i];
            CreateLeaderboardEntry(i + 1, entry);
        }
        
        // Force canvas update
        Canvas.ForceUpdateCanvases();
        
        Debug.Log($"✅ Leaderboard atjaunots ar {topEntries.Count} ierakstiem");
    }
    
    void CreateLeaderboardEntry(int rank, LeaderboardEntry entry)
    {
        if (leaderboardEntryPrefab == null)
        {
            Debug.LogError("❌ Leaderboard Entry Prefab nav piesķirts!");
            return;
        }
        
        if (leaderboardContent == null)
        {
            Debug.LogError("❌ Leaderboard Content nav piesķirts!");
            return;
        }
        
        GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContent);
        entryObj.name = $"Entry_{rank}";
        
        // Atrod text komponentus - vairāki mēģinājumi
        TextMeshProUGUI[] allTexts = entryObj.GetComponentsInChildren<TextMeshProUGUI>(true);
        
        TextMeshProUGUI rankTxt = null;
        TextMeshProUGUI nameTxt = null;
        TextMeshProUGUI stepsTxt = null;
        
        // Mēģina atrast ar Find
        Transform rankTransform = entryObj.transform.Find("RankText");
        if (rankTransform != null) rankTxt = rankTransform.GetComponent<TextMeshProUGUI>();
        
        Transform nameTransform = entryObj.transform.Find("NameText");
        if (nameTransform != null) nameTxt = nameTransform.GetComponent<TextMeshProUGUI>();
        
        Transform stepsTransform = entryObj.transform.Find("StepsText");
        if (stepsTransform != null) stepsTxt = stepsTransform.GetComponent<TextMeshProUGUI>();
        
        // Ja nav atrasts, mēģina pēc secības
        if (rankTxt == null && allTexts.Length > 0) rankTxt = allTexts[0];
        if (nameTxt == null && allTexts.Length > 1) nameTxt = allTexts[1];
        if (stepsTxt == null && allTexts.Length > 2) stepsTxt = allTexts[2];
        
        // Atjauno tekstus
        if (rankTxt != null) 
        {
            rankTxt.text = $"#{rank}";
            
            // Īpaša krāsa TOP 3
            if (rank == 1) rankTxt.color = new Color(1f, 0.84f, 0f); // Zelts
            else if (rank == 2) rankTxt.color = new Color(0.75f, 0.75f, 0.75f); // Sudrabs
            else if (rank == 3) rankTxt.color = new Color(0.8f, 0.5f, 0.2f); // Bronza
            else rankTxt.color = Color.white;
            
            Debug.Log($"✅ Rank #{rank} uzstādīts");
        }
        else
        {
            Debug.LogWarning($"⚠️ RankText nav atrasts entry {rank}");
        }
        
        if (nameTxt != null) 
        {
            nameTxt.text = entry.playerName;
            Debug.Log($"✅ Name: {entry.playerName}");
        }
        else
        {
            Debug.LogWarning($"⚠️ NameText nav atrasts entry {rank}");
        }
        
        if (stepsTxt != null) 
        {
            stepsTxt.text = $"{entry.steps} gājieni";
            Debug.Log($"✅ Steps: {entry.steps}");
        }
        else
        {
            Debug.LogWarning($"⚠️ StepsText nav atrasts entry {rank}");
        }
    }
    
    void CreatePlaceholderText(string message)
    {
        GameObject placeholder = new GameObject("PlaceholderText");
        placeholder.transform.SetParent(leaderboardContent);
        placeholder.transform.localScale = Vector3.one;
        
        TextMeshProUGUI text = placeholder.AddComponent<TextMeshProUGUI>();
        text.text = message;
        text.fontSize = 28;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.8f, 0.8f, 0.8f);
        
        RectTransform rt = placeholder.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(600, 100);
        rt.anchoredPosition = Vector2.zero;
        
        Debug.Log($"📝 Placeholder izveidots: {message}");
    }
    
    public void HideLeaderboard()
    {
        Debug.Log("🏆 Aizver Leaderboard");
        
        // Ja ir PauseManager, izmanto to
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.CloseLeaderboard();
        }
        else
        {
            // Citādi vienkārši slēpj paneli
            gameObject.SetActive(false);
        }
    }
}