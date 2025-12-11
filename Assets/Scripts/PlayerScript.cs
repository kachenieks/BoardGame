using UnityEngine;
using System.IO; 
using System.Linq;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    private const string textFileName = "PlayerNames";

    void Start()
    {
        int playerCount = PlayerPrefs.GetInt("PlayerCount", 2);
        int characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        string playerName = PlayerPrefs.GetString("PlayerName", "John Doe");
        
        string[] nameArray = ReadLinesFromFile(textFileName);
        
        // VIENMĒR atrod Floor1 un spawno tur!
        GameObject[] allFloors = GameObject.FindGameObjectsWithTag("Tile");
        GameObject floor1 = allFloors
            .OrderBy(f => ExtractFloorNumber(f.name))
            .FirstOrDefault();
        
        Vector3 basePosition;
        if (floor1 != null)
        {
            basePosition = floor1.transform.position;
            Debug.Log($"✅ Spawno uz: {floor1.name} @ {basePosition}");
        }
        else
        {
            basePosition = new Vector3(-2f, 0.5f, -3.2f); // Fallback
            Debug.LogWarning("⚠️ Nav atrasts Floor1! Izmanto default pozīciju");
        }
        
        Debug.Log($"🎮 Base spawn pozīcija: {basePosition}");

        // === GALVENAIS SPĒLĒTĀJS (Index 0) ===
        Vector3 mainPos = basePosition;
        
        GameObject mainCharacter = Instantiate(
            playerPrefabs[characterIndex], 
            mainPos, 
            Quaternion.identity);
        
        mainCharacter.name = "MainPlayer";
        mainCharacter.GetComponent<NameScript>().SetName(playerName);
        
        PlayerMovement mainMovement = mainCharacter.AddComponent<PlayerMovement>();
        mainMovement.isMainPlayer = true;
        mainMovement.playerIndex = 0;

        Debug.Log($"✅ Main: {playerName} @ {mainPos}, index=0, isMain=TRUE");

        // === AI SPĒLĒTĀJI (Index 1, 2, 3...) ===
        for(int i = 1; i < playerCount; i++)
        {
            Vector3 aiPos = basePosition + new Vector3(i * 0.12f, 0, i * 0.06f);
            
            int randomCharacterIndex = Random.Range(0, playerPrefabs.Length);
            
            GameObject aiPlayer = Instantiate(
                playerPrefabs[randomCharacterIndex], 
                aiPos, 
                Quaternion.identity);
            
            aiPlayer.name = $"AIPlayer_{i}";
            
            string aiName = nameArray[Random.Range(0, nameArray.Length)];
            aiPlayer.GetComponent<NameScript>().SetName(aiName);
            
            PlayerMovement aiMovement = aiPlayer.AddComponent<PlayerMovement>();
            aiMovement.isMainPlayer = false;
            aiMovement.playerIndex = i;
            
            Debug.Log($"✅ AI_{i}: {aiName} @ {aiPos}, index={i}, isMain=FALSE");
        }

        Debug.Log($"🎮 TOTAL: {playerCount} spēlētāji izveidoti");
    }

    private int ExtractFloorNumber(string name)
    {
        string numberPart = new string(name.Where(char.IsDigit).ToArray());
        if (int.TryParse(numberPart, out int number))
        {
            return number;
        }
        return 0;
    }

    string[] ReadLinesFromFile(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);
        
        if (textAsset != null)
        {
            return textAsset.text.Split(new[] { '\r', '\n' }, 
                System.StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            Debug.LogWarning("File not found: " + fileName);
            return new string[] { "Bot1", "Bot2", "Bot3", "Bot4" };
        }
    }
}