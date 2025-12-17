using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Debug poga lai uzreiz uzvarētu spēli testēšanai
/// Teleportē galveno spēlētāju uz pēdējo rūtiņu
/// </summary>
public class DebugWinButton : MonoBehaviour
{
    [Header("UI")]
    public Button debugButton;
    public TextMeshProUGUI buttonText;
    
    [Header("Settings")]
    public KeyCode debugHotkey = KeyCode.F1;
    public bool showButton = true; // Vai rādīt pogu (var slēpt production build)
    
    void Start()
    {
        // Setup button
        if (debugButton != null)
        {
            debugButton.onClick.AddListener(TriggerInstantWin);
            
            // Poga redzama tikai ja ir enabled
            debugButton.gameObject.SetActive(showButton);
            
            if (buttonText != null)
            {
                buttonText.text = "🏆 DEBUG WIN";
            }
        }
        
        Debug.Log($"🐛 Debug Win Button gatavs! Spied {debugHotkey} vai pogu lai uzvarētu uzreiz");
    }
    
    void Update()
    {
        // Hotkey support
        if (Input.GetKeyDown(debugHotkey))
        {
            TriggerInstantWin();
        }
    }
    
    public void TriggerInstantWin()
    {
        Debug.Log("🏆 DEBUG: Triggering instant win...");
        
        // Atrod galveno spēlētāju
        PlayerMovement mainPlayer = FindMainPlayer();
        
        if (mainPlayer == null)
        {
            Debug.LogError("❌ Nav atrasts main player!");
            return;
        }
        
        Debug.Log($"🎯 Teleportē {mainPlayer.name} uz finiša līniju");
        
        // Trigger win condition - PlayerMovement pats teleportēs un parādīs win screen
        mainPlayer.CheckForWin();
        
        Debug.Log("✅ Instant win triggered!");
    }
    
    PlayerMovement FindMainPlayer()
    {
        PlayerMovement[] allPlayers = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);
        
        foreach (PlayerMovement player in allPlayers)
        {
            if (player.isMainPlayer)
            {
                return player;
            }
        }
        
        return null;
    }
    
    // Editor helper - izveido pogu automātiski
#if UNITY_EDITOR
    [ContextMenu("Create Debug Win Button UI")]
    void CreateDebugButtonUI()
    {
        // Atrod Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ Nav Canvas!");
            return;
        }
        
        // Izveido Button GameObject
        GameObject buttonObj = new GameObject("DebugWinButton");
        buttonObj.transform.SetParent(canvas.transform, false);
        
        // Add Button component
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(1f, 0.84f, 0f, 0.8f); // Zelts
        
        // RectTransform setup - apakšējais labais stūris
        RectTransform rt = buttonObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 0); // Bottom-right
        rt.anchorMax = new Vector2(1, 0);
        rt.pivot = new Vector2(1, 0);
        rt.anchoredPosition = new Vector2(-20, 20);
        rt.sizeDelta = new Vector2(150, 50);
        
        // Izveido Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "🏆 DEBUG WIN";
        text.fontSize = 18;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.black;
        text.fontStyle = FontStyles.Bold;
        
        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;
        
        // Pievieno šo script uz šo objektu
        debugButton = button;
        buttonText = text;
        
        Debug.Log("✅ Debug Win Button UI izveidots!");
    }
#endif
}