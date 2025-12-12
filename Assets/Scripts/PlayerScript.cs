using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    private const string textFileName = "PlayerNames";

    void Start()
    {
        // SVARĪGI: Izdzēš visus vecus spēlētājus no iepriekšējās scēnas UZREIZ
        CleanupOldPlayers();

        // Gaida 0.1s lai Destroy() pabeigtu darbu
        StartCoroutine(CreatePlayersAfterCleanup());
    }

    IEnumerator CreatePlayersAfterCleanup()
    {
        // Gaida lai vecais spēlētājs tiktu izdzēsts
        yield return new WaitForSeconds(0.2f);

        int playerCount = PlayerPrefs.GetInt("PlayerCount", 2);
        int characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");

        Debug.Log($"🎮 PlayerCount: {playerCount}, Character: {characterIndex}, Name: {playerName}");

        // Pārbauda vai playerPrefabs ir iestatīts
        if (playerPrefabs == null || playerPrefabs.Length == 0)
        {
            Debug.LogError("❌ KRITISKA KĻŪDA: playerPrefabs nav iestatīts Inspector! Pievieno player prefabus!");
            yield break;
        }

        Debug.Log($"✅ Atrasti {playerPrefabs.Length} player prefabi");

        // Pārbauda vai characterIndex ir derīgs
        if (characterIndex >= playerPrefabs.Length)
        {
            Debug.LogWarning($"⚠️ characterIndex {characterIndex} ir par lielu, lietosim 0");
            characterIndex = 0;
        }

        string[] nameArray = ReadLinesFromFile(textFileName);
        Debug.Log($"✅ Ielādēti {nameArray.Length} AI vārdi");

        // Atrod Floor1 pozīciju
        GameObject[] allFloors = GameObject.FindGameObjectsWithTag("Tile");

        if (allFloors == null || allFloors.Length == 0)
        {
            Debug.LogError("❌ Nav atrasts neviens objekts ar Tag 'Tile'!");
            yield break;
        }

        GameObject floor1 = allFloors
            .OrderBy(f => ExtractFloorNumber(f.name))
            .FirstOrDefault();

        Vector3 basePosition = floor1 != null
            ? floor1.transform.position
            : new Vector3(-2f, 0.5f, -3.2f);

        Debug.Log($"🎮 Base spawn pozīcija: {basePosition}");

        // ===== IZVEIDO GALVENO SPĒLĒTĀJU =====
        Debug.Log($"🎮 Izveidoju galveno spēlētāju (index 0)...");

        GameObject mainCharacter = Instantiate(
            playerPrefabs[characterIndex],
            basePosition,
            Quaternion.identity);

        if (mainCharacter == null)
        {
            Debug.LogError("❌ Neizdevās izveidot mainCharacter!");
            yield break;
        }

        mainCharacter.name = "MainPlayer";

        // Iestata vārdu
        NameScript mainNameScript = mainCharacter.GetComponent<NameScript>();
        if (mainNameScript != null)
        {
            mainNameScript.SetName(playerName);
        }

        // Iestata PlayerMovement
        PlayerMovement mainMovement = mainCharacter.GetComponent<PlayerMovement>();
        if (mainMovement != null)
        {
            mainMovement.isMainPlayer = true;
            mainMovement.playerIndex = 0;
            Debug.Log($"✅ Main Player: '{playerName}', index=0, isMainPlayer=TRUE");
        }
        else
        {
            Debug.LogError("❌ MainPlayer prefabam nav PlayerMovement komponente!");
        }

        // ===== IZVEIDO AI SPĒLĒTĀJUS =====
        for (int i = 1; i < playerCount; i++)
        {
            Debug.Log($"🤖 Izveidoju AI spēlētāju {i}...");

            Vector3 aiPos = basePosition + new Vector3(i * 0.12f, 0, i * 0.06f);
            int randomCharacterIndex = Random.Range(0, playerPrefabs.Length);

            GameObject aiPlayer = Instantiate(
                playerPrefabs[randomCharacterIndex],
                aiPos,
                Quaternion.identity);

            if (aiPlayer == null)
            {
                Debug.LogError($"❌ Neizdevās izveidot AI spēlētāju {i}!");
                continue;
            }

            aiPlayer.name = $"AIPlayer_{i}";

            // Iestata AI vārdu
            NameScript aiNameScript = aiPlayer.GetComponent<NameScript>();
            if (aiNameScript != null)
            {
                string aiName = nameArray[Random.Range(0, nameArray.Length)];
                aiNameScript.SetName(aiName);
            }

            // Iestata PlayerMovement
            PlayerMovement aiMovement = aiPlayer.GetComponent<PlayerMovement>();
            if (aiMovement != null)
            {
                aiMovement.isMainPlayer = false;
                aiMovement.playerIndex = i;
                Debug.Log($"✅ AI Player {i}: index={i}, isMainPlayer=FALSE");
            }
            else
            {
                Debug.LogError($"❌ AIPlayer_{i} prefabam nav PlayerMovement komponente!");
            }
        }

        Debug.Log($"🎮 ✅✅✅ PABEIGTS: Izveidoti {playerCount} spēlētāji! ✅✅✅");

        // ===== DROŠI PAZIŅO TURNMANAGER (NEVIS FindFirstObjectByType) =====
        // Pagaidām pāris frame, lai Instance noteikti iestātos
        yield return null;
        yield return null;

        GameTurnManager turnManager = GameTurnManager.Instance;

        if (turnManager != null)
        {
            List<PlayerMovement> players =
                FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None).ToList();

            Debug.Log("📣 PlayerScript paziņo GameTurnManager: spēlētāji gatavi!");
            turnManager.OnPlayersCreated(players);
        }
        else
        {
            Debug.LogError("❌ PlayerScript: GameTurnManager Instance nav atrasts! (Pārbaudi vai GameManager objektā ir GameTurnManager skripts un tas ir aktīvs)");
        }
    }

    void CleanupOldPlayers()
    {
        // Atrod visus vecus spēlētājus
        PlayerMovement[] oldPlayers = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);

        if (oldPlayers.Length > 0)
        {
            Debug.Log($"🧹 Izdzēšam {oldPlayers.Length} vecus spēlētājus...");
            foreach (var player in oldPlayers)
            {
                Destroy(player.gameObject);
            }
        }
    }

    int ExtractFloorNumber(string name)
    {
        string num = new string(name.Where(char.IsDigit).ToArray());
        return int.TryParse(num, out int n) ? n : 0;
    }

    string[] ReadLinesFromFile(string fileName)
    {
        TextAsset ta = Resources.Load<TextAsset>(fileName);

        if (ta == null)
        {
            Debug.LogWarning($"⚠️ Nav atrasts Resources/{fileName}.txt, lietosim default vārdus");
            return new string[] { "Bot1", "Bot2", "Bot3", "Bot4", "Bot5" };
        }

        string[] lines = ta.text.Split('\n');
        Debug.Log($"✅ Ielādēti {lines.Length} vārdi no {fileName}");
        return lines;
    }
}
