using UnityEngine;
using UnityEngine.UI;
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
    private List<PlayerMovement> allPlayers = new List<PlayerMovement>();
    private List<int> finishedPlayers = new List<int>();
    
    private bool gameEnded = false;

    void Start()
    {
        // Pagaida lai visi spēlētāji tiktu izveidoti
        StartCoroutine(InitializeGame());
    }

    IEnumerator InitializeGame()
    {
        // Pagaida 0.5s lai PlayerScript izveido spēlētājus
        yield return new WaitForSeconds(0.5f);
        
        // Atrod visus spēlētājus
        FindAllPlayers();
        
        // Sāk pirmo gājienu
        if (allPlayers.Count > 0)
        {
            StartTurn(0);
        }
        else
        {
            Debug.LogError("❌ Nav atrasts neviens spēlētājs!");
        }
    }

    void FindAllPlayers()
    {
        PlayerMovement[] foundPlayers = FindObjectsOfType<PlayerMovement>();
        
        // Sakārto pēc playerIndex
        allPlayers = foundPlayers.OrderBy(p => p.playerIndex).ToList();
        
        Debug.Log($"✅ GameTurnManager atrasti {allPlayers.Count} spēlētāji");
        
        foreach (var player in allPlayers)
        {
            Debug.Log($"   - Player {player.playerIndex}, isMain: {player.isMainPlayer}");
        }
    }

    void StartTurn(int playerIndex)
    {
        if (gameEnded) return;
        if (allPlayers.Count == 0) return;
        
        currentPlayerIndex = playerIndex;
        
        // Reset spēlētāja statusu
        if (playerIndex < allPlayers.Count)
        {
            allPlayers[playerIndex].ResetForNewTurn();
            
            // Parāda kura spēlētāja gājiens
            if (allPlayers[playerIndex].isMainPlayer)
            {
                UpdateTurnUI("Tavs gājiens! 🎲 Met kauliņu!");
                
                // Parāda kauliņu
                if (diceObject != null)
                {
                    diceObject.SetActive(true);
                }
            }
            else
            {
                UpdateTurnUI($"AI Spēlētājs {playerIndex} gājiens...");
                
                // Dice paliek redzams
                if (diceObject != null)
                {
                    diceObject.SetActive(true);
                }
            }
        }
    }

    public void NextTurn()
    {
        if (gameEnded) return;
        if (allPlayers.Count == 0) return;
        
        // Atrod nākamo spēlētāju kas vēl nav beidzis
        int nextPlayer = currentPlayerIndex;
        int attempts = 0;
        
        do
        {
            nextPlayer = (nextPlayer + 1) % allPlayers.Count;
            attempts++;
            
            // Ja visi spēlētāji ir beiguši
            if (attempts > allPlayers.Count)
            {
                EndGame();
                return;
            }
        }
        while (finishedPlayers.Contains(nextPlayer));
        
        StartTurn(nextPlayer);
    }

    public void PlayerFinished(int playerIndex)
    {
        if (!finishedPlayers.Contains(playerIndex))
        {
            finishedPlayers.Add(playerIndex);
            Debug.Log($"🏁 Spēlētājs {playerIndex} beidza! Pozīcija: {finishedPlayers.Count}");
            
            // Pārbauda vai visi ir beiguši
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
        Debug.Log(message);
    }

    // Funkcija lai restart spēli
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
}