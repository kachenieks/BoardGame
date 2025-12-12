using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameTurnManager : MonoBehaviour
{
    [Header("UI Elements")]
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
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("✅ GameTurnManager Instance iestatīts");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("🎮 GameTurnManager: Start() - gaida spēlētājus...");
        // Atstājam arī šo – ja kādreiz PlayerScript nepaziņo, TurnManager pats savāks
        StartCoroutine(InitializeGame());
    }

    IEnumerator InitializeGame()
    {
        int expectedPlayerCount = PlayerPrefs.GetInt("PlayerCount", 2);

        while (!gameStarted)
        {
            PlayerMovement[] foundPlayers =
                FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);

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

            yield return null;
        }
    }

    void FindAndLinkDiceAndPlayers()
    {
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

            if (allPlayers[playerIndex].isMainPlayer)
            {
                UpdateTurnUI("Tavs gājiens! 🎲 Met kauliņu!");

                if (diceObject != null)
                    diceObject.SetActive(true);
            }
            else
            {
                UpdateTurnUI($"AI Spēlētājs {playerIndex} gājiens...");

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
