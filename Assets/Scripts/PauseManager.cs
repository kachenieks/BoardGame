using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    
    [Header("Pause UI")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    public GameObject leaderboardPanel;
    
    [Header("EventSystem (optional)")]
    public EventSystem eventSystem;
    
    private bool isPaused = false;
    
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
        
        // Automātiski atrod EventSystem ja nav piesaistīts
        if (eventSystem == null)
        {
            eventSystem = FindObjectOfType<EventSystem>();
        }
    }
    
    void Start()
    {
        // Sākumā viss slēpts
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
    }
    
    void Update()
    {
        // Spied ESC lai pause/unpause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                // Ja ir atvērts settings vai leaderboard, vispirms tos aizver
                if (settingsPanel != null && settingsPanel.activeSelf)
                {
                    CloseSettings();
                }
                else if (leaderboardPanel != null && leaderboardPanel.activeSelf)
                {
                    CloseLeaderboard();
                }
                else
                {
                    // Citādi unpause spēli
                    Resume();
                }
            }
            else
            {
                // Pause spēli
                Pause();
            }
        }
    }
    
    public void Pause()
    {
        if (pauseMenuPanel == null)
        {
            Debug.LogError("❌ Pause Menu Panel nav piesķirts!");
            return;
        }
        
        isPaused = true;
        Time.timeScale = 0f; // Apstādina spēli
        pauseMenuPanel.SetActive(true);
        
        Debug.Log("⏸️ Spēle uz pauzes");
    }
    
    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f; // Atjauno spēli
        
        // Aizver visus paneļus
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
        
        Debug.Log("▶️ Spēle turpinās");
    }
    
    public void RestartGame()
    {
        Debug.Log("🔄 Restartē spēli");
        
        Time.timeScale = 1f; // Atjauno laiku pirms scene reload
        isPaused = false;
        
        // Pārlādē pašreizējo scenu
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    
    public void ReturnToMainMenu()
    {
        Debug.Log("🏠 Atgriežas uz Main Menu");
        
        Time.timeScale = 1f; // Atjauno laiku
        isPaused = false;
        
        // Ielādē MainMenu scenu
        SceneManager.LoadScene("MainMenu");
    }
    
    // Settings kontrole
    public void OpenSettings()
    {
        Debug.Log("⚙️ Atvēra Settings no Pause");
        
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            
            // Pārlādē settings values
            if (SettingsManager.Instance != null)
            {
                SettingsManager.Instance.ReloadSettings();
            }
        }
    }
    
    public void CloseSettings()
    {
        Debug.Log("⚙️ Aizvēra Settings");
        
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        }
    }
    
    // Leaderboard kontrole
    public void OpenLeaderboard()
    {
        Debug.Log("🏆 Atvēra Leaderboard no Pause");
        
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            
            // Ja ir GameLeaderboard komponents, atjauno to
            var lb = leaderboardPanel.GetComponent<GameLeaderboard>();
            if (lb != null)
            {
                lb.RefreshLeaderboard();
            }
        }
    }
    
    public void CloseLeaderboard()
    {
        Debug.Log("🏆 Aizvēra Leaderboard");
        
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        }
    }
    
    public bool IsPaused()
    {
        return isPaused;
    }
    
    // Debug funkcija lai pārbaudītu vai EventSystem strādā
    void OnGUI()
    {
        if (!isPaused) return;
        
        if (eventSystem != null && !eventSystem.enabled)
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(10, 10, 300, 30), "⚠️ EventSystem ir disabled!");
        }
    }
}