using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameTurnManager : MonoBehaviour
{
    [Header("UI Elements (aizpildās automātiski)")]
    public TextMeshProUGUI turnInfoText;
    public GameObject diceObject;

    [Header("Spēlētāju info")]
    public int currentPlayerIndex = 0;
    public List<PlayerMovement> allPlayers = new List<PlayerMovement>();
    private List<int> finishedPlayers = new List<int>();

    private bool gameEnded = false;
    private bool gameStarted = false;

    public static GameTurnManager Instance;

    void Awake()
    {
        Debug.Log($"🔍 [{Time.frameCount}] GameTurnManager.Awake() sākās");
        Debug.Log($"   Objekta nosaukums: {gameObject.name}");
        Debug.Log($"   Scene: {gameObject.scene.name}");
        Debug.Log($"   Instance pirms pārbaudes: {Instance}");
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"✅ [{Time.frameCount}] GameTurnManager Instance iestatīts (DontDestroyOnLoad)");
            Debug.Log($"   Instance tagad ir: {Instance.gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"⚠️ [{Time.frameCount}] Jau eksistē Instance!");
            Debug.LogWarning($"   Esošais Instance: {Instance.gameObject.name} (scene: {Instance.gameObject.scene.name})");
            Debug.LogWarning($"   Šis objekts: {gameObject.name} (scene: {gameObject.scene.name})");
            Debug.LogWarning($"   Iznīcinu šo objektu: {gameObject.name}");
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        Debug.Log($"🟢 [{Time.frameCount}] GameTurnManager.OnEnable(): {gameObject.name}");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        Debug.Log($"🟡 [{Time.frameCount}] GameTurnManager.OnDisable(): {gameObject.name}");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnDestroy()
    {
        Debug.Log($"💀 [{Time.frameCount}] GameTurnManager.OnDestroy(): {gameObject.name}");
        if (Instance == this)
        {
            Debug.LogError("⚠️ SINGLETON INSTANCE TIEK IZNĪCINĀTS!");
            Instance = null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"🎬 Scene ielādēta: {scene.name}, mode: {mode}");
        Debug.Log($"   GameTurnManager.Instance: {Instance}");
        Debug.Log($"   Šis objekts: {gameObject.name}");
        
        // ✅ Ja ielādēta spēles scēna, atrodam UI elementus
        if (scene.buildIndex == 1) // Level1 scene
        {
            FindUIElements();
        }
    }

    void Start()
    {
        Debug.Log($"🎮 [{Time.frameCount}] GameTurnManager: Start() - gaida spēlētājus...");
        
        // Ja jau esam spēles scēnā, atrodam UI
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            FindUIElements();
        }
        
        StartCoroutine(InitializeGame());
    }

    // ✅ JAUNA METODE: Atrod UI elementus runtime
    void FindUIElements()
    {
        Debug.Log("🔍 Meklē UI elementus scēnā...");
        
        // Atrod Turn Info Text
        if (turnInfoText == null)
        {
            // Meklē pēc nosaukuma vai tipa
            TextMeshProUGUI[] allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
            
            foreach (var text in allTexts)
            {
                // Pieņemam, ka tas ir vienīgais TMP teksts Canvas vai ir konkrēts nosaukums
                if (text.gameObject.name.Contains("TurnInfo") || 
                    text.gameObject.name.Contains("Turn") ||
                    text.gameObject.name.Contains("Info"))
                {
                    turnInfoText = text;
                    Debug.Log($"✅ Atrasts Turn Info Text: {text.gameObject.name}");
                    break;
                }
            }
            
            // Ja joprojām nav atrasts, ņem pirmo Canvas text
            if (turnInfoText == null && allTexts.Length > 0)
            {
                turnInfoText = allTexts[0];
                Debug.Log($"⚠️ Izmanto pirmo TMP tekstu: {turnInfoText.gameObject.name}");
            }
        }
        
        // Atrod Dice Object
        if (diceObject == null)
        {
            DiceRollScript dice = FindFirstObjectByType<DiceRollScript>();
            if (dice != null)
            {
                diceObject = dice.gameObject;
                Debug.Log($"✅ Atrasts Dice Object: {diceObject.name}");
            }
            else
            {
                Debug.LogWarning("⚠️ Nav atrasts DiceRollScript objekts!");
            }
        }
    }

    IEnumerator InitializeGame()
    {
        int expectedPlayerCount = PlayerPrefs.GetInt("PlayerCount", 2);
        Debug.Log($"📊 InitializeGame: gaida {expectedPlayerCount} spēlētājus");

        while (!gameStarted)
        {
            PlayerMovement[] foundPlayers =
                FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);

            if (foundPlayers.Length > 0)
            {
                Debug.Log($"   Atrasti {foundPlayers.Length}/{expectedPlayerCount} spēlētāji");
            }

            if (foundPlayers.Length >= expectedPlayerCount)
            {
                allPlayers = foundPlayers
                    .OrderBy(p => p.playerIndex)
                    .ToList();

                Debug.Log($"✅ Atrasti visi {allPlayers.Count} spēlētāji (InitializeGame)");
                FindAndLinkDiceAndPlayers();

                gameStarted = true;
                currentPlayerIndex = 0;

                Debug.Log("🚀 Spēle SĀKTA – Player 0 (InitializeGame)");
                StartTurn(0);
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    void FindAndLinkDiceAndPlayers()
    {
        // ✅ Pārliecinamies, ka UI elementi ir atrasti
        if (turnInfoText == null || diceObject == null)
        {
            FindUIElements();
        }
        
        DiceRollScript dice = FindFirstObjectByType<DiceRollScript>();

        if (dice == null)
        {
            Debug.LogError("❌ GameTurnManager: DiceRollScript NAV atrasts!");
            return;
        }

        Debug.Log($"✅ GameTurnManager atrasti {allPlayers.Count} spēlētāji (Link)");

        foreach (var player in allPlayers)
        {
            player.SetTurnManager(this);
            player.SetDice(dice);

            Debug.Log($"🔗 Player {player.playerIndex} pieslēgts TurnManager + Dice");
        }
    }

    void StartTurn(int playerIndex)
    {
        if (gameEnded) return;
        if (allPlayers.Count == 0) return;

        currentPlayerIndex = playerIndex;

        Debug.Log($"🎯 Sākas gājiens: playerIndex={playerIndex}");

        if (playerIndex < allPlayers.Count)
        {
            allPlayers[playerIndex].ResetForNewTurn();

            // ✅ RESET kauliņu PIRMS gājiena sākuma
            DiceRollScript dice = FindFirstObjectByType<DiceRollScript>();
            if (dice != null)
            {
                dice.ResetDice();
            }

            if (allPlayers[playerIndex].isMainPlayer)
            {
                UpdateTurnUI("Tavs gājiens!");

                if (diceObject != null)
                    diceObject.SetActive(true);
            }
            else
            {
                UpdateTurnUI($"AI {playerIndex} gājiens...");

                if (diceObject != null)
                    diceObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError($"❌ playerIndex {playerIndex} ir ārpus robežām! allPlayers.Count={allPlayers.Count}");
        }
    }

    public void NextTurn()
    {
        if (gameEnded) return;
        if (allPlayers.Count == 0) return;

        Debug.Log($"➡️ NextTurn() izsaukts, pašreizējais index: {currentPlayerIndex}");

        int nextPlayer = currentPlayerIndex;
        int attempts = 0;

        do
        {
            nextPlayer = (nextPlayer + 1) % allPlayers.Count;
            attempts++;

            Debug.Log($"   Pārbauda player {nextPlayer}, attempts={attempts}");

            if (attempts > allPlayers.Count)
            {
                Debug.Log("   Visi spēlētāji ir beiguši!");
                EndGame();
                return;
            }
        }
        while (finishedPlayers.Contains(nextPlayer));

        Debug.Log($"✅ Nākamais spēlētājs: {nextPlayer}");
        StartTurn(nextPlayer);
    }

    public void PlayerFinished(int playerIndex)
    {
        if (!finishedPlayers.Contains(playerIndex))
        {
            finishedPlayers.Add(playerIndex);
            Debug.Log($"🏁 Spēlētājs {playerIndex} beidza! Pozīcija: {finishedPlayers.Count}");

            if (finishedPlayers.Count >= allPlayers.Count)
            {
                EndGame();
            }
        }
    }

    void EndGame()
    {
        gameEnded = true;
        UpdateTurnUI("🎉 Spēle beigusies!");

        if (diceObject != null)
        {
            diceObject.SetActive(false);
        }

        Debug.Log("=== Spēles rezultāti ===");
        for (int i = 0; i < finishedPlayers.Count; i++)
        {
            Debug.Log($"{i + 1}. vieta: Spēlētājs {finishedPlayers[i]}");
        }
    }

    void UpdateTurnUI(string message)
    {
        if (turnInfoText != null)
        {
            turnInfoText.text = message;
        }
        else
        {
            Debug.LogWarning("⚠️ turnInfoText ir null, nevaru atjaunot UI!");
        }
        Debug.Log($"📢 UI: {message}");
    }

    public void RestartGame()
    {
        gameEnded = false;
        finishedPlayers.Clear();
        currentPlayerIndex = 0;

        foreach (var player in allPlayers)
        {
            player.ResetPosition();
        }

        StartTurn(0);
    }

    public bool IsCurrentPlayerHuman()
    {
        if (!gameStarted) return false;
        if (allPlayers == null || allPlayers.Count == 0) return false;
        if (currentPlayerIndex < 0 || currentPlayerIndex >= allPlayers.Count) return false;

        return allPlayers[currentPlayerIndex].isMainPlayer;
    }

    public void OnPlayersCreated(List<PlayerMovement> players)
    {
        allPlayers = players.OrderBy(p => p.playerIndex).ToList();

        Debug.Log($"✅ OnPlayersCreated: saņemti {allPlayers.Count} spēlētāji");

        FindAndLinkDiceAndPlayers();

        gameStarted = true;
        currentPlayerIndex = 0;

        Debug.Log("🚀 Spēle SĀKTA – Player 0 (OnPlayersCreated)");
        StartTurn(0);
    }
}